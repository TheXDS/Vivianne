using System.Globalization;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.ViewModels;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a raw image blob into a <see cref="Brush"/>.
/// </summary>
public class RawImageToBrushConverter : RawImageConverterBase, IOneWayValueConverter<byte[], Brush?>
{
    /// <inheritdoc/>
    public Brush? Convert(byte[] value, object? parameter, CultureInfo? culture)
    {
        return GetBitmap(value, parameter as RenderColor[]) is { } bmp ? new ImageBrush(bmp) { ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.Tile } : (Brush?)null;
    }
}