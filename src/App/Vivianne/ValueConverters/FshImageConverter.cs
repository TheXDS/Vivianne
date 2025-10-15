using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts the data in a <see cref="FshBlob"/> into a
/// <see cref="BitmapSource"/>.
/// </summary>
public class FshImageConverter : IMultiValueConverter, IOneWayValueConverter<FshBlob, BitmapSource?>
{
    /// <inheritdoc/>
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 1 || values[0] is not FshBlob blob) return null;
        
        var p = (values.ElementAtOrDefault(1) as IEnumerable<SixLabors.ImageSharp.Color>)?.ToArray() ?? blob.ReadLocalPalette() ?? (parameter as FshFile)?.GetPalette();
        var image = blob.ToImage(p);
        var alpha = values.ElementAtOrDefault(2) as bool? ?? true;

        return image switch
        {
            Image<Abgr32>   i => GetImage(i, FshBlobFormat.Argb32, alpha),
            Image<Bgra32>   i => GetImage(i, FshBlobFormat.Argb32, alpha),
            Image<Rgba32>   i => GetImage(i, FshBlobFormat.Argb32, alpha),
            Image<Bgr24>    i => ConvertImageToBitmapSource(FshBlobFormat.Rgb24, i),
            Image<Rgb24>    i => ConvertImageToBitmapSource(FshBlobFormat.Rgb24, i),
            Image<Bgr565>   i => ConvertImageToBitmapSource(FshBlobFormat.Rgb565, i),
            Image<Bgra4444> i => ConvertImageToBitmapSource(FshBlobFormat.Argb4444, i),
            _ => null
        };
    }

    private static BitmapSource GetImage<T>(Image<T> image, FshBlobFormat format, bool alpha) where T : unmanaged, IPixel, IPixel<T>
    {
        return alpha ? ConvertImageToBitmapSource(format, image) : ConvertImageToBitmapSource(FshBlobFormat.Rgb24, RemoveAlpha(image));
    }

    private static Image<Bgr24> RemoveAlpha<T>(Image<T> image) where T : unmanaged, IPixel, IPixel<T>
    {
        var output = new Image<Bgr24>(image.Width, image.Height);
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                Rgba32 sourcePixel = default;
                image[x, y].ToRgba32(ref sourcePixel);
                output[x, y] = new Bgr24(
                    sourcePixel.R,
                    sourcePixel.G,
                    sourcePixel.B
                );
            }
        }
        return output;
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static BitmapSource ConvertImageToBitmapSource<T>(FshBlobFormat format, Image<T> image) where T : unmanaged, IPixel<T>
    {
        var width = image.Width;
        var height = image.Height;
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        var writer = Mappings.FshBlobToPixelWriter[format];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var offset = ((y * width) + x) * 4;
                var pixel = image[x, y];
                bw.Write(writer.Invoke(pixel));
            }
        }
        return BitmapSource.Create(width, height, 96, 96, GetFormat(format), null, ms.ToArray(), width * (image.PixelType.BitsPerPixel / 8));
    }

    private static PixelFormat GetFormat(FshBlobFormat format)
    {
        return format switch
        {
            FshBlobFormat.Argb32 => PixelFormats.Bgra32,
            FshBlobFormat.Rgb24 => PixelFormats.Bgr24,
            FshBlobFormat.Rgb565 => PixelFormats.Bgr565,
            //FshBlobFormat.Indexed8 => PixelFormats.Indexed8,

            /* Formats that do not have a pixel format directly supported by
             * WPF need to be converted to BGRA32 */
            _ => PixelFormats.Bgra32 
        };
    }

    /// <inheritdoc/>
    public BitmapSource? Convert(FshBlob value, object? parameter, CultureInfo? culture)
    {
        return Convert([value], typeof(BitmapSource), parameter!, culture! ) as BitmapSource;
    }
}