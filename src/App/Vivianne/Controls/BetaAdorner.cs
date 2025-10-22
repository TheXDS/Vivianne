using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using TheXDS.MCART.Math;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements an adorner that renders warning bars on top of a control to
/// indicate its fragile or beta status.
/// </summary>
/// <param name="adornedElement">
/// Element to draw the warning bars on top of.
/// </param>
public class BetaAdorner(UIElement adornedElement) : Adorner(adornedElement)
{
    private static Brush? WarningBarsBrush = Application.Current.FindResource("WarningBars") as Brush;
    private static Typeface Typeface = (Application.Current.FindResource("nfsFont") as FontFamily)?.GetTypefaces().FirstOrDefault() ?? new Typeface("Segoe UI");

    /// <inheritdoc/>
    protected override void OnRender(DrawingContext c)
    {
        if (AdornedElement is not { IsVisible: true }) return;
        c.DrawRectangle(WarningBarsBrush, null, new Rect(0, 0, AdornedElement.RenderSize.Width, 48));
        var textGeometry = new FormattedText("BETA", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 24, new SolidColorBrush(new Color() { A = 48, R = 127, G = 127, B = 127 }), 1);
        var point = new Point(AdornedElement.RenderSize.Width - (textGeometry.Width + 10), 8);
        c.DrawText(textGeometry, point);
    }
}