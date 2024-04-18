using System.Globalization;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

public class RawImageToBrushConverter : RawImageConverterBase, IOneWayValueConverter<byte[], Brush?>
{
    public Brush? Convert(byte[] value, object? parameter, CultureInfo? culture)
    {
        return GetBitmap(value) is { } bmp ? new ImageBrush(bmp) { TileMode = TileMode.Tile} : (Brush?)null;
    }
}