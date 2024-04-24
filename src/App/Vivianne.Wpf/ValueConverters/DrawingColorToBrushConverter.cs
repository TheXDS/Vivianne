using System.Globalization;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a <see cref="System.Drawing.Color"/> to a <see cref="Brush"/>.
/// </summary>
public class DrawingColorToBrushConverter : IOneWayValueConverter<System.Drawing.Color, Brush>
{
    /// <inheritdoc/>
    public Brush Convert(System.Drawing.Color value, object? parameter, CultureInfo? culture)
    {
        return new SolidColorBrush(Color.FromRgb(value.R, value.G, value.B));
    }
}
