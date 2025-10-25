using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using TheXDS.MCART.Math;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements an adorner that displays a large overlay text on top of visual
/// elements with a message.
/// </summary>
public class ImportantWatermarkMessage : Adorner
{
    private readonly FrameworkElement _control;
    private readonly string _message;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportantWatermarkMessage"/> class.
    /// </summary>
    /// <param name="control">Control to draw the adorner onto.</param>
    /// <param name="message">
    /// Message to be displayed as an overlay to inform about a beta-related
    /// topic.
    /// </param>
    public ImportantWatermarkMessage(FrameworkElement control, string message) : base(control)
    {
        _control = control;
        _message = message;
        IsHitTestVisible = false;
    }

    /// <inheritdoc/>
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (!_control.IsVisible) return;
        var textGeometry = new FormattedText(_message, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 48, new SolidColorBrush(new Color() { A = 255, R = 255, G = 0, B = 0 }), 1);
        var point = _control.TranslatePoint(new Point((_control.ActualWidth.OrIfInvalid(0) - textGeometry.Width) / 2, (_control.ActualHeight.OrIfInvalid(0) - textGeometry.Height) / 2), _control);
        drawingContext.DrawText(textGeometry, point);
    }
}