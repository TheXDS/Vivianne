using System.Globalization;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models.Fce;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a <see cref="IHsbColor"/> to a <see cref="Brush"/>.
/// </summary>
public class FceColorToBrushConverter : IOneWayValueConverter<IHsbColor, Brush>
{
    /// <inheritdoc/>
    public Brush Convert(IHsbColor value, object? parameter, CultureInfo? culture)
    {
        var (r, g, b) = ColorConversion.ToRgb(value.Hue, value.Saturation, value.Brightness);
        return new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
    }
}