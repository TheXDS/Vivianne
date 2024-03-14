using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using MC = TheXDS.MCART.Types.Color;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts the data in a <see cref="FshBlob"/> into a
/// <see cref="BitmapSource"/>.
/// </summary>
public class FshImageConverter : IMultiValueConverter
{
    private readonly record struct FshFormat(PixelFormat Format, BitmapPalette? Palette);

    private static readonly Dictionary<FshBlobFormat, PixelFormat> Formats = new()
    {
        {FshBlobFormat.Indexed8, PixelFormats.Indexed8},
        {FshBlobFormat.Rgb565, PixelFormats.Bgr565},
        {FshBlobFormat.Argb32, PixelFormats.Bgra32},
    };

    /// <inheritdoc/>
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is not FshBlob gimx || !Formats.TryGetValue(gimx.Magic, out var fshFormat)) return null;
        var pal = gimx.Magic == FshBlobFormat.Indexed8 && values.Length >= 2 ? (Create(values[1]) ?? BitmapPalettes.Halftone256) : null;
        var surface = new WriteableBitmap(gimx.Width, gimx.Height, 96, 96, fshFormat, pal);
        var stride = ((gimx.Width * fshFormat.BitsPerPixel) + 7) / 8;
        surface.WritePixels(new Int32Rect(0, 0, gimx.Width, gimx.Height), gimx.PixelData, stride, 0);
        surface.Freeze();
        return surface;
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static BitmapPalette? Create(object? value)
    {
        return value is IEnumerable<MC> colors
            ? new BitmapPalette(colors.Select(WpfColorExtensions.Color).ToList())
            : null;
    }
}