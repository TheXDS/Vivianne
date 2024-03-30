using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts the data in a <see cref="FshBlob"/> into a
/// <see cref="BitmapSource"/>.
/// </summary>
public class FshImageConverter : IMultiValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >=1 && values[0] is FshBlob blob)
        {
            var p = values.Length >=2 ? (values[1] as IEnumerable<SixLabors.ImageSharp.Color>)?.ToArray() : null;
            return blob.ToImage(p) switch {
                Image<Rgba32> i => ConvertImageToBitmapSource(FshBlobFormat.Argb32, i),
                Image<Rgb24> i => ConvertImageToBitmapSource(FshBlobFormat.Rgb24, i),
                Image<Bgr565> i => ConvertImageToBitmapSource(FshBlobFormat.Rgb565, i),
                _ => null
            };
        }
        return null;
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
                var offset = (y * width + x) * 4;
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
}