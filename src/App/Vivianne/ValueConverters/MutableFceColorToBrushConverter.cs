using System.Globalization;
using System.Windows.Media;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a <see cref="MutableFceColor"/> to a <see cref="Brush"/>.
/// </summary>
public class MutableFceColorToBrushConverter : IOneWayValueConverter<MutableFceColor, Brush>
{
    /// <inheritdoc/>
    public Brush Convert(MutableFceColor value, object? parameter, CultureInfo? culture)
    {
        var (r, g, b) = value.ToColor().ToRgb();
        return new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
    }
}
