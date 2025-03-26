using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.ValueConverters;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements an adorner that draws a small FCE color preview on the right of the adorned control.
/// </summary>
/// <param name="control">Control to draw the adorner onto.</param>
/// <param name="color">Reference to the FCE color to draw.</param>
public class FceColorPreviewAdorner(Control control, FceColor? color) : Adorner(control)
{
    private static readonly FceColorToBrushConverter _converter = new();
    private readonly Control _control = control;
    private readonly FceColor? _color = color;
    private readonly Brush? _primaryBrush = color is not null ? _converter.Convert(color.PrimaryColor, null, null) : null;
    private readonly Brush? _secondaryBrush = color is not null ? _converter.Convert(color.SecondaryColor, null, null) : null;

    /// <inheritdoc/>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (_color is null) return;
        drawingContext.DrawRectangle(_primaryBrush, null, new Rect(_control.ActualWidth - 20, (_control.ActualHeight / 2) - 5, 5, 10));
        drawingContext.DrawRectangle(_secondaryBrush, null, new Rect(_control.ActualWidth - 15, (_control.ActualHeight / 2) - 5, 5, 10));
    }
}
