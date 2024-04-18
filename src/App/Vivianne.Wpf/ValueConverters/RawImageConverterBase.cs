using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.ValueConverters;

public abstract class RawImageConverterBase
{
    protected BitmapSource? GetBitmap(byte[] value)
    {
        if (value is null) return null;
        try
        {
            return Image.Load(value) switch
            {
                Image<Rgba32> i => ConvertImageToBitmapSource(FshBlobFormat.Argb32, i),
                Image<Rgb24> i => ConvertImageToBitmapSource(FshBlobFormat.Rgb24, i),
                Image<Bgr565> i => ConvertImageToBitmapSource(FshBlobFormat.Rgb565, i),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    private static BitmapSource ConvertImageToBitmapSource<T>(FshBlobFormat format, Image<T> image) where T : unmanaged, IPixel<T>
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
                bw.Write(writer.Invoke(pixel));
            }
        }
        return BitmapSource.Create(width, height, 96, 96, GetFormat(format), null, ms.ToArray(), width * (image.PixelType.BitsPerPixel / 8));
    }

    private static IReadOnlyDictionary<FshBlobFormat, Func<object, byte[]>> FshBlobToPixelWriter { get; } = new Dictionary<FshBlobFormat, Func<object, byte[]>>()
    {
        { FshBlobFormat.Argb32,         c => { var x = (Rgba32)c; return [x.B, x.G, x.R, (byte)255]; }},
        { FshBlobFormat.Rgb24,          c => { var x = (Rgb24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Rgb565,         c => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Argb1555,       c => { var x = (Bgra5551)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Palette32,      c => { var x = (Rgba32)c; return [x.B, x.G, x.R, (byte)255]; }},
        { FshBlobFormat.Palette24Dos,   c => { var x = (Rgb24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Palette24,      c => { var x = (Rgb24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Palette16Nfs5,  c => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Palette16,      c => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
    }.AsReadOnly();

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
