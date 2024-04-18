using System.Globalization;
using System.Windows.Media.Imaging;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Loads a raw image file into a <see cref="BitmapSource"/>, determining the
/// codec to use automatically.
/// </summary>
public class RawImageConverter : RawImageConverterBase, IOneWayValueConverter<byte[], BitmapSource>
{
    /// <inheritdoc/>
    public BitmapSource Convert(byte[] value, object? parameter, CultureInfo? culture)
    {
        return GetBitmap(value);
    }
}
