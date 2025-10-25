using System.Globalization;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a <see cref="MutableFceColor"/> to a <see cref="Brush"/>.
/// </summary>
public class MutableFceColorToBrushConverter : IOneWayValueConverter<MutableFceColor, Brush>
{
    /// <inheritdoc/>
    public Brush Convert(MutableFceColor value, object? parameter, CultureInfo? culture)
    {
        var (r, g, b, _) = value.ToRgba();
        return new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
    }
}
