using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fsh;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Base class for all converters that can load an image from raw bytes and
/// return a <see cref="BitmapSource"/>.
/// </summary>
public abstract class RawImageConverterBase
{
    /// <summary>
    /// Converts a byte array into a <see cref="BitmapSource"/>, optionally
    /// including a Car color to paint over the semi-transparent areas of the
    /// texture.
    /// </summary>
    /// <param name="value">
    /// Raw image content. Will be loaded using any supported codec (TGA, PNG,
    /// etc).
    /// </param>
    /// <param name="textureColor">
    /// Gets a color to pain over the semi-transparent areas of the texture. If
    /// set to <see langword="null"/>, the texture will not be painted over.
    /// </param>
    /// <param name="enableAlpha">If set to <see langword="false"/>, disables the alpha channel.</param>
    /// <returns>
    /// A <see cref="BitmapSource"/> that can be rendered as a texture or
    /// surface, or <see langword="null"/> if the byte array could not be
    /// parsed as an image in any known format.
    /// </returns>
    protected static BitmapSource? GetBitmap(byte[] value, Fce3Color? textureColor, bool enableAlpha = true)
    {
        if (value is null) return null;
        try
        {
            return Image.Load(value) switch
            {
                Image<Rgba32> i => ConvertImageToBitmapSource(FshBlobFormat.Argb32, i, textureColor, enableAlpha),
                Image<Rgb24> i => ConvertImageToBitmapSource(FshBlobFormat.Rgb24, i, textureColor, enableAlpha),
                Image<Bgr565> i => ConvertImageToBitmapSource(FshBlobFormat.Rgb565, i, textureColor, enableAlpha),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    private static BitmapSource ConvertImageToBitmapSource<T>(FshBlobFormat format, Image<T> image, Fce3Color? textureColor, bool enableAlpha) where T : unmanaged, IPixel<T>
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
                var offset = ((y * width) + x) * 4;
                var pixel = image[x, y];
                bw.Write(writer.Invoke(pixel, textureColor, enableAlpha));
            }
        }
        return BitmapSource.Create(width, height, 96, 96, GetFormat(format), null, ms.ToArray(), width * (image.PixelType.BitsPerPixel / 8));
    }

    private static ReadOnlyDictionary<FshBlobFormat, Func<object, Fce3Color?, bool, byte[]>> FshBlobToPixelWriter { get; } = new Dictionary<FshBlobFormat, Func<object, Fce3Color?, bool, byte[]>>()
    {
        { FshBlobFormat.Argb32,         GetColoredPixel  },
        { FshBlobFormat.Rgb24,          (c, f, _) => { var x = (Rgb24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Rgb565,         (c, f, _) => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Argb1555,       (c, f, a) => { var x = (Bgra5551)c; return BitConverter.GetBytes(x.PackedValue | (a ? 0 : 1)); }},
        { FshBlobFormat.Palette32,      (c, f, a) => { var x = (Rgba32)c; return [x.B, x.G, x.R, a ? x.A : (byte)255]; }},
        { FshBlobFormat.Palette24Dos,   (c, f, _) => { var x = (Rgb24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Palette24,      (c, f, _) => { var x = (Rgb24)c; return [x.B, x.G, x.R]; }},
        { FshBlobFormat.Palette16Nfs5,  (c, f, _) => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
        { FshBlobFormat.Palette16,      (c, f, _) => { var x = (Bgr565)c; return BitConverter.GetBytes(x.PackedValue); }},
    }.AsReadOnly();

    private static byte[] GetColoredPixel(object color, Fce3Color? carColor, bool enableAlpha)
    {
        var x = (Rgba32)color;
        if (carColor is not null)
        {
            var primary = carColor.PrimaryColor.ToRgba();
            var secondary = carColor.SecondaryColor.ToRgba();
            if (x.A.IsBetween<byte>(30, 120))
            {
                return [(byte)(x.B * (primary.B / 255.0)), (byte)(x.G * (primary.G / 255.0)), (byte)(x.R * (primary.R / 255.0)), 255];
            }
            else if (x.A.IsBetween<byte>(120, 220))
            {
                return [(byte)(x.B * (secondary.B / 255.0)), (byte)(x.G * (secondary.G / 255.0)), (byte)(x.R * (secondary.R / 255.0)), 255];
            }
        }
        return [x.B, x.G, x.R, enableAlpha ? x.A : (byte)255];
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
