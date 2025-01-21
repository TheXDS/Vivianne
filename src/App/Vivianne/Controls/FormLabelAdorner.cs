using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TheXDS.MCART.Math;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements an adorner that will attach a label on the left side of a control.
/// </summary>
public class FormLabelAdorner : Adorner
{
    private readonly Control _control;
    private readonly string _placeholderText;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormLabelAdorner"/>
    /// class.
    /// </summary>
    /// <param name="control">Control to attach a label to.</param>
    /// <param name="label">Label to be attached.</param>
    public FormLabelAdorner(Control control, string label) : base(control)
    {
        _control = control;
        control.ToolTip ??= label;
        _placeholderText = label;
        IsHitTestVisible = false;
    }

    /// <inheritdoc/>
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (!_control.IsVisible) return;
        var textGeometry = new FormattedText( _placeholderText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 12, Application.Current.TryFindResource("TextFillColorSecondaryBrush") as Brush ?? Brushes.Gray, 1);
        var point = _control.TranslatePoint(new Point(5, (((double[])[_control.Height.OrIfInvalid(0), _control.ActualHeight.OrIfInvalid(0), 24]).Max() - 14) / 2), _control);
        _control.Padding = new Thickness(textGeometry.Width + 10, _control.Padding.Top, _control.Padding.Right, _control.Padding.Bottom);
        drawingContext.DrawText(textGeometry, point);
    }
}
