using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static TheXDS.MCART.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Represents a control that displays an image with interactive zooming and
/// coordinate tracking capabilities.
/// </summary>
/// <remarks>
/// The control allows users to zoom in and out of the displayed image and view
/// the current mouse coordinates over the image. It supports various image
/// source types and provides dependency properties for data binding. The zoom
/// level is constrained to a specific range and is applied to the image
/// content. Coordinate tracking is updated as the user moves the mouse over
/// the image. This control is suitable for scenarios where users need to
/// inspect image details or select specific regions interactively.
/// </remarks>
public class ZoomableImageControl : Control
{
    private static readonly DependencyPropertyKey CoordinateTextPropertyKey;

    /// <summary>
    /// Identifies the <see cref="CoordinateText"/> readonly dependency property.
    /// </summary>
    public static readonly DependencyProperty CoordinateTextProperty;

    /// <summary>
    /// Identifies the <see cref="Source"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty;

    /// <summary>
    /// Identifies the <see cref="Zoom"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomProperty;

    static ZoomableImageControl()
    {
        SetControlStyle<ZoomableImageControl>(DefaultStyleKeyProperty);
        (CoordinateTextPropertyKey, CoordinateTextProperty) = NewDpRo<string, ZoomableImageControl>(nameof(CoordinateText), string.Empty!);
        SourceProperty = NewDp<ImageSource, ZoomableImageControl>(nameof(Source));
        ZoomProperty = NewDp<double, ZoomableImageControl>(nameof(Zoom), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 1.0, OnZoomChanged, CoerceZoom);
    }

    private Image? _image;
    private Border? _container;
    private Point _lastDragPoint;
    private bool _isDragging;
    private readonly ScaleTransform _scale = new(1, 1);
    private readonly TranslateTransform _translate = new();

    /// <summary>
    /// Gets a text representation of the current mouse coordinates over the image.
    /// </summary>
    public string CoordinateText
    {
        get => (string)GetValue(CoordinateTextProperty);
        private set => SetValue(CoordinateTextPropertyKey, value);
    }

    /// <summary>
    /// Gets or sets the image source displayed by the control.
    /// </summary>
    /// <remarks>
    /// The value can be a bitmap, vector image, or other supported image
    /// source type. Setting this property updates the displayed image. If the
    /// value is <see langword="null"/>, no image is shown.
    /// </remarks>
    public ImageSource Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom level applied to the content.
    /// </summary>
    /// <remarks>
    /// The zoom level determines the scale at which the content is displayed.
    /// A value of <c>1.0</c> represents 100% (no zoom), values greater than
    /// <c>1.0</c> increase the size, and values less than <c>1.0</c> decrease
    /// the size. The valid range of values may depend on the specific
    /// implementation.
    /// </remarks>
    public double Zoom
    {
        get => (double)GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    /// <inheritdoc/>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _image = GetTemplateChild("PART_Image") as Image;
        _container = GetTemplateChild("PART_Container") as Border;

        if (_image is null || _container is null)
            return;

        var group = new TransformGroup();
        group.Children.Add(_scale);
        group.Children.Add(_translate);

        _image.RenderTransform = group;

        _container.MouseWheel += OnMouseWheel;
        _container.MouseLeftButtonDown += OnMouseDown;
        _container.MouseLeftButtonUp += OnMouseUp;
        _container.MouseMove += OnMouseMove;

        _container.Cursor = Cursors.Cross;
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (_image?.Source is not BitmapSource) return;
        double oldZoom = Zoom;
        Zoom += e.Delta > 0 ? 0.5 : -0.5;
        double scaleFactor = Zoom / oldZoom;
        Point mouse = e.GetPosition(_container);
        _translate.X = mouse.X - scaleFactor * (mouse.X - _translate.X);
        _translate.Y = mouse.Y - scaleFactor * (mouse.Y - _translate.Y);
        ApplyZoom();
    }

    private void ApplyZoom()
    {
        _scale.ScaleX = Zoom;
        _scale.ScaleY = Zoom;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _lastDragPoint = e.GetPosition(_container);
        _container!.CaptureMouse();

        Mouse.OverrideCursor = Cursors.SizeAll;
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        _container!.ReleaseMouseCapture();
        Mouse.OverrideCursor = Cursors.Cross;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        Point pos = e.GetPosition(_container);
        UpdateCoordinates(pos);
        if (!_isDragging) return;
        Vector delta = pos - _lastDragPoint;
        _translate.X += delta.X;
        _translate.Y += delta.Y;
        _lastDragPoint = pos;
    }

    private void UpdateCoordinates(Point containerPoint)
    {
        if (_image?.Source is not BitmapSource source) return;
        try
        {
            GeneralTransform transform = _container!.TransformToVisual(_image);
            Point p = transform.Transform(containerPoint);

            double x = p.X * source.PixelWidth / _image.ActualWidth;
            double y = p.Y * source.PixelHeight / _image.ActualHeight;

            if (x < 0 || y < 0 || x > source.PixelWidth || y > source.PixelHeight)
            {
                CoordinateText = string.Empty;
                return;
            }

            CoordinateText = $"{(int)x}, {(int)y}";
        }
        catch
        {
            CoordinateText = string.Empty;
        }
    }

    private static object CoerceZoom(DependencyObject d, object baseValue)
    {
        return double.Clamp(Math.Round((double)baseValue * 2.0) / 2.0, 1.0, 3.5);
    }

    private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ZoomableImageControl control && control._image != null)
        {
            control._image.LayoutTransform = new ScaleTransform(control.Zoom, control.Zoom);
        }
    }
}