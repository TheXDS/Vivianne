using System.Globalization;
using System.Windows.Media;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a <see cref="FceColor"/> to a <see cref="Brush"/>.
/// </summary>
public class FceColorToBrushConverter : IOneWayValueConverter<FceColor, Brush>
{
    /// <inheritdoc/>
    public Brush Convert(FceColor value, object? parameter, CultureInfo? culture)
    {
        var (r, g, b) = value.ToRgb();
        return new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
    }
}