using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TheXDS.MCART.Math;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements an adorner that draws the mouse coordinates at the bottom right
/// corner of the adorned control.
/// </summary>
public class MouseTrackingAdorner : Adorner
{
    private readonly FrameworkElement _control;
    private Point? _mousePosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseTrackingAdorner"/>
    /// class.
    /// </summary>
    /// <param name="control">Control to draw the mouse position into.</param>
    public MouseTrackingAdorner(FrameworkElement control) : base(control)
    {
        _control = control;
        control.MouseMove += OnMouseMove;
        control.MouseLeave += OnMouseLeave;
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        _mousePosition = null;
        InvalidateVisual();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        _mousePosition = e.GetPosition(_control);
        InvalidateVisual();
    }

    /// <inheritdoc/>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (!_control.IsVisible || _mousePosition is null) return;
        var textGeometry = new FormattedText($"X: {_mousePosition.Value.X} Y: {_mousePosition.Value.Y}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 12, Application.Current.TryFindResource("TextFillColorSecondaryBrush") as Brush ?? Brushes.Gray, 1);
        var point = _control.TranslatePoint(new Point(5, (((double[])[_control.Height.OrIfInvalid(0), _control.ActualHeight.OrIfInvalid(0), 24]).Max() - 14)), _control);
        drawingContext.DrawText(textGeometry, point);
    }
}
