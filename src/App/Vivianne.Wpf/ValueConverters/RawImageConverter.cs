using FreeImageAPI;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Loads a raw image file into a <see cref="BitmapSource"/>, determining the
/// codec to use automatically.
/// </summary>
public class RawImageConverter : IOneWayValueConverter<byte[], BitmapSource>
{
    /// <inheritdoc/>
    public BitmapSource Convert(byte[] value, object? parameter, CultureInfo? culture)
    {
        using var ms = new MemoryStream(value);
        FIBITMAP dib = FreeImage.LoadFromStream(ms);
        var source = FreeImage.GetBitmap(dib).ToSource();
        FreeImage.Unload(dib);
        return source;
    }
}
