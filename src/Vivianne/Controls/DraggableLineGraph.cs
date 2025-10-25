using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Math;
using static TheXDS.MCART.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements a control that allows the user to edit collections of numeric
/// values with a line graph layout.
/// </summary>
[ContentProperty(nameof(ItemsSource))]
public class DraggableLineGraph : Control
{
    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty;

    /// <summary>
    /// Identifies the <see cref="MinValue"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinValueProperty;

    /// <summary>
    /// Identifies the <see cref="MaxValue"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxValueProperty;

    /// <summary>
    /// Identifies the <see cref="CornerRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty;

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty;

    /// <summary>
    /// Identifies the <see cref="PointsBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PointsBrushProperty;

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OpenEditorDialogCommandProperty;

    static DraggableLineGraph()
    {
        SetControlStyle<DraggableLineGraph>(DefaultStyleKeyProperty);
        ItemsSourceProperty = NewDp<IList<double>, DraggableLineGraph>(nameof(ItemsSource), FrameworkPropertyMetadataOptions.AffectsRender, [], OnDataChanged);
        PointsBrushProperty = NewDp<Brush, DraggableLineGraph>(nameof(PointsBrush), FrameworkPropertyMetadataOptions.AffectsRender);
        MinValueProperty = NewDp2Way<double, DraggableLineGraph>(nameof(MinValue), FrameworkPropertyMetadataOptions.AffectsRender, double.NaN, OnUpdateVisuals);
        MaxValueProperty = NewDp2Way<double, DraggableLineGraph>(nameof(MaxValue), FrameworkPropertyMetadataOptions.AffectsRender, double.NaN, OnUpdateVisuals);
        ForegroundProperty.OverrideMetadata<DraggableLineGraph>(Brushes.Red);
        CornerRadiusProperty = NewDp<CornerRadius, DraggableLineGraph>(nameof(CornerRadius), FrameworkPropertyMetadataOptions.AffectsRender);
        TitleProperty = NewDp<string?, DraggableLineGraph>(nameof(Title));
        OpenEditorDialogCommandProperty = NewDp<ICommand?, DraggableLineGraph>(nameof(OpenEditorDialogCommand));
    }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is INotifyCollectionChanged oldData)
        {
            oldData.CollectionChanged -= ((DraggableLineGraph)d).OnDataCollectionChanged;
        }
        if (e.NewValue is IEnumerable<double> c && d is DraggableLineGraph g)
        {
            if (!g.MinValue.IsValid()) g.MinValue = c.Min();
            if (!g.MaxValue.IsValid()) g.MaxValue = c.Max();
            if (c is INotifyCollectionChanged newData)
            {
                newData.CollectionChanged += ((DraggableLineGraph)d).OnDataCollectionChanged;
            }
        }
    }

    private static void OnUpdateVisuals(DependencyObject d, DependencyPropertyChangedEventArgs _)
    {
        ((DraggableLineGraph)d).UpdateVisuals();
    }

    private readonly List<Shape> pointTicks = [];
    private Canvas? canvas = null;
    private double canvasWidth;
    private double canvasHeight;
    private int? draggingIndex;
    private bool isDragging;
    private Point initialMouseDownPosition;
    private double initialValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="DraggableLineGraph"/>
    /// class.
    /// </summary>
    public DraggableLineGraph()
    {
        SizeChanged += OnSizeChanged;
        Loaded += DraggableLineGraph_Loaded;
    }

    /// <summary>
    /// Gets or sets the list to be displayed and updated through this control.
    /// </summary>
    public IList<double> ItemsSource
    {
        get => (IList<double>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum renderable value on this control.
    /// </summary>
    public double MinValue
    {
        get => (double)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum renderable value on this control.
    /// </summary>
    public double MaxValue
    {
        get => (double)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the round decorative border around the line
    /// graph area.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush to use when drawing the points on the graph.
    /// </summary>
    public Brush PointsBrush
    {
        get => (Brush)GetValue(PointsBrushProperty);
        set => SetValue(PointsBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the title of the control.
    /// </summary>
    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets a command that can be used to invoke a full-screen editor
    /// for the collection being presented on this control.
    /// </summary>
    public ICommand? OpenEditorDialogCommand
    {
        get => (ICommand?)GetValue(OpenEditorDialogCommandProperty);
        set => SetValue(OpenEditorDialogCommandProperty, value);
    }

    /// <inheritdoc/>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        canvas = GetTemplateChild("PART_Canvas") as Canvas;
        if (canvas != null)
        {
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            UpdateVisuals();
        }
    }

    private Point CalculatePosition(int index, double value)
    {
        double x = (canvasWidth * index / (ItemsSource.Count - 1)) + 5;
        double y = ScaleValueToCanvas(value);
        return new Point(x, y);
    }

    private double ScaleValueToCanvas(double value)
    {
        double minValue = MinValue.OrIfInvalid(ItemsSource.Min());
        double maxValue = MaxValue.OrIfInvalid(ItemsSource.Max());
        double scaledY = canvasHeight - ((value - minValue) / (maxValue - minValue) * canvasHeight) + 5;
        return scaledY;
    }

    private Ellipse CreatePointTick() => new()
    {
        Width = 10,
        Height = 10,
        Fill = PointsBrush,
        Effect = new DropShadowEffect()
        {
            Color = Colors.Black,
            BlurRadius = 3,
            ShadowDepth = 2
        }
    };

    private void UpdateVisuals()
    {
        if (canvas == null || ItemsSource == null || !ItemsSource.Any()) return;
        canvasWidth = canvas.ActualWidth - 10;
        canvasHeight = canvas.ActualHeight - 10;
        canvas.Children.Clear();
        foreach (var e in pointTicks)
        {
            e.MouseDown -= PointTick_MouseDown;
        }
        pointTicks.Clear();
        Point[] points = [.. ItemsSource.WithIndex().Select(p => CalculatePosition(p.index, p.element))];
        Polyline polyline = new()
        {
            Stroke = Brushes.Gray,
            StrokeThickness = 2,
            Points = [.. points]
        };
        canvas.Children.Add(polyline);
        foreach ((var position, var value) in points.Zip(ItemsSource))
        {
            var pointTick = CreatePointTick();
            pointTick.ToolTip = value;
            Canvas.SetLeft(pointTick, position.X - (pointTick.Width / 2));
            Canvas.SetTop(pointTick, position.Y - (pointTick.Height / 2));
            canvas.Children.Add(pointTick);
            pointTicks.Add(pointTick);
            pointTick.MouseDown += PointTick_MouseDown;
        }
    }

    private void OnDataCollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
    {
        UpdateVisuals();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateVisuals();
    }

    private void DraggableLineGraph_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateVisuals();
    }

    private void PointTick_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            draggingIndex = pointTicks.IndexOf((Ellipse)sender);
            if (draggingIndex.HasValue && draggingIndex.Value != -1)
            {
                initialMouseDownPosition = e.GetPosition(canvas);
                initialValue = ItemsSource[draggingIndex.Value];
                canvas?.CaptureMouse();
                isDragging = true;
            }
        }
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging && draggingIndex.HasValue && draggingIndex.Value != -1)
        {
            Point currentPos = e.GetPosition(canvas);
            double deltaY = currentPos.Y - initialMouseDownPosition.Y;
            double newValue = initialValue - (deltaY / canvasHeight * (MaxValue - MinValue));
            ItemsSource[draggingIndex.Value] = newValue;
            UpdateVisuals();
        }
    }

    private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && isDragging)
        {
            canvas?.ReleaseMouseCapture();
            isDragging = false;
            draggingIndex = null;
        }
    }
}