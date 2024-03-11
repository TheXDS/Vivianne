using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts the data in a <see cref="Gimx"/> into a
/// <see cref="BitmapSource"/>.
/// </summary>
public class FshImageConverter : IOneWayValueConverter<Gimx?, BitmapSource?>
{
    private readonly record struct FshFormat(PixelFormat Format, BitmapPalette? Palette);

    private static readonly Dictionary<GimxFormat, PixelFormat> Formats = new()
    {
        {GimxFormat.Indexed8, PixelFormats.Indexed8},
        {GimxFormat.Bgr565, PixelFormats.Bgr565},
        {GimxFormat.Bgra32, PixelFormats.Bgra32},
    };

    /// <inheritdoc/>
    public BitmapSource? Convert(Gimx? value, object? parameter, CultureInfo? culture)
    {
        if (value is not Gimx gimx || !Formats.TryGetValue(gimx.Magic, out var fshFormat)) return null;
        GC.Collect();        
        var pal = gimx.Magic == GimxFormat.Indexed8 ? (Create(parameter) ?? BitmapPalettes.Halftone256) : null;
        var surface = new WriteableBitmap(gimx.Width, gimx.Height, 96, 96, fshFormat, pal);
        var stride = (gimx.Width * fshFormat.BitsPerPixel + 7) / 8;
        surface.WritePixels(new Int32Rect(0, 0, gimx.Width, gimx.Height), gimx.PixelData, stride, 0);
        surface.Freeze();
        return surface;
    }

    private static BitmapPalette? Create(object? value)
    {
        return value is IList<Color> colors ? new BitmapPalette(colors) : null;
    }
}