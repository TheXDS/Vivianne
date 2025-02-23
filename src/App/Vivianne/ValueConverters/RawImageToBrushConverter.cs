using System.Globalization;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a raw image blob into a <see cref="Brush"/>.
/// </summary>
public class RawImageToBrushConverter : RawImageConverterBase, IOneWayValueConverter<byte[], Brush?>
{
    /// <inheritdoc/>
    public Brush? Convert(byte[] value, object? parameter, CultureInfo? culture)
    {
        return GetBitmap(value, parameter as Fce3Color) is { } bmp ? new ImageBrush(bmp) { ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.Tile } : (Brush?)null;
    }
}