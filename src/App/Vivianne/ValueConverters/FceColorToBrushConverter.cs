using System.Globalization;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a <see cref="HsbColor"/> to a <see cref="Brush"/>.
/// </summary>
public class FceColorToBrushConverter : IOneWayValueConverter<HsbColor, Brush>
{
    /// <inheritdoc/>
    public Brush Convert(HsbColor value, object? parameter, CultureInfo? culture)
    {
        var (r, g, b) = value.ToRgb();
        return new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
    }
}