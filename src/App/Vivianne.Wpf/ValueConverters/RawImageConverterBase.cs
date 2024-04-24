using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.ValueConverters;

public abstract class RawImageConverterBase
{
    protected BitmapSource? GetBitmap(byte[] value, CarColorItem? textureColor)
    {
        if (value is null) return null;
        try
        {
            return Image.Load(value) switch
            {
                Image<Rgba32> i => ConvertImageToBitmapSource(FshBlobFormat.Argb32, i, textureColor),
                Image<Rgb24> i => ConvertImageToBitmapSource(FshBlobFormat.Rgb24, i, textureColor),
                Image<Bgr565> i => ConvertImageToBitmapSource(FshBlobFormat.Rgb565, i, textureColor),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    private static BitmapSource ConvertImageToBitmapSource<T>(FshBlobFormat format, Image<T> image, CarColorItem? textureColor) where T : unmanaged, IPixel<T>
    {
        var width = image.Width;
        var height = image.Height;
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        var writer = FshBlobToPixelWriter[format];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var offset = (y * width + x) * 4;
                var pixel = image[x, y];
                bw.Write(writer.Invoke(pixel, textureColor));
            }
        }
        return BitmapSource.Create(width, height, 96, 96, GetFormat(format), null, ms.ToArray(), width * (image.PixelType.BitsPerPixel / 8));
    }

    private static IReadOnlyDictionary<FshBlobFormat, Func<object, CarColorItem?, byte[]>> FshBlobToPixelWriter { get; } = new Dictionary<FshBlobFormat, Func<object, CarColorItem?, byte[]>>()
    {
        { FshBlobFormat.Argb32,         GetColoredPixel  },
        { FshBlobFormat.Rgb24,          (c, f) => { var x = (Rgb24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Rgb565,         (c, f) => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Argb1555,       (c, f) => { var x = (Bgra5551)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Palette32,      (c, f) => { var x = (Rgba32)c; return [x.B, x.G, x.R, (byte)255]; }},
        { FshBlobFormat.Palette24Dos,   (c, f) => { var x = (Rgb24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Palette24,      (c, f) => { var x = (Rgb24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Palette16Nfs5,  (c, f) => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Palette16,      (c, f) => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
    }.AsReadOnly();

    private static byte[] GetColoredPixel(object color, CarColorItem? carColor)
    {
        var x = (Rgba32)color;
        if (carColor is not null)
        {
            if (x.A.IsBetween<byte>(30, 120))
            {
                return [(byte)(x.B * (carColor.Primary.B / 255.0)), (byte)(x.G * (carColor.Primary.G / 255.0)), (byte)(x.R * (carColor.Primary.R / 255.0)), (byte)255];
            }
            else if (x.A.IsBetween<byte>(120, 220))
            {
                return [(byte)(x.B * (carColor.Secondary.B / 255.0)), (byte)(x.G * (carColor.Secondary.G / 255.0)), (byte)(x.R * (carColor.Secondary.R / 255.0)), (byte)255];
            }
        }
        return [x.B, x.G, x.R, x.A];
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
