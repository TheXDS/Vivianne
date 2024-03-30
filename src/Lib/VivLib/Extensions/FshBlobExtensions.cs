using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using static TheXDS.Vivianne.Resources.Mappings;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Contains a series of extension methods for the <see cref="FshBlob"/> class.
/// </summary>
public static class FshBlobExtensions
{
    /// <summary>
    /// Converts a <see cref="FshBlob"/> into an <see cref="Image"/>.
    /// </summary>
    /// <param name="blob">GIMX to export.</param>
    /// <returns>
    /// A new <see cref="Image"/> instance.
    /// </returns>
    public static Image? ToImage(this FshBlob blob, Color[]? palette = null)
    {
        if (FshBlobPixelReader.TryGetValue(blob.Magic, out var callback))
        {
            return callback.Invoke(blob.PixelData, blob.Width, blob.Height);
        }
        else if (blob.Magic == FshBlobFormat.Indexed8)
        {
            palette ??= LoadPalette(blob.Footer) ?? CreatePalette();
            return LoadFromIndexedBlob(blob, palette, p => p.ToPixel<Rgba32>());
        }
        else return null;
    }

    private static Color[]? LoadPalette(byte[] footer)
    {
        var s = new FshBlobSerializer();
        using var ms = new MemoryStream(footer);
        var blob = s.Deserialize(ms);
        return FshBlobToPalette[blob.Magic].Invoke(blob);
    }

    /// <summary>
    /// Replaces the GIMX data from an image.
    /// </summary>
    /// <param name="blob">GIMX to replace the data in.</param>
    /// <param name="image">Image from which to load the new data.</param>
    /// <param name="palette">
    /// Specifies the Color palette to use in case the FSH blob uses the
    /// <see cref="FshBlobFormat.Indexed8"/> pixel format.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no data encoding for the specified FSH blob format has been
    /// implemented.
    /// </exception>
    public static void ReplaceWith(this FshBlob blob, Image image, Color[]? palette)
    {
        if (blob.Magic == FshBlobFormat.Indexed8)
        {

            var quantizerFactory = new OctreeQuantizer(new QuantizerOptions() { MaxColors = 256 });
            var quantizer = quantizerFactory.CreatePixelSpecificQuantizer<Rgba32>(Configuration.Default);

            quantizer.BuildPalette(new ExtensivePixelSamplingStrategy(), (Image<Rgba32>)image);
            blob.LocalPalette = quantizer.Palette.ToArray().Select(p => new Color(p)).ToArray();

            using var q = quantizer.QuantizeFrame((ImageFrame<Rgba32>)image.Frames[0], new Rectangle(0, 0, image.Width, image.Height));
            var p = new List<byte>();
            for (int y = 0; y < image.Height; y++)
            {
                p.AddRange(q.DangerousGetRowSpan(y));
            }
            blob.PixelData = [.. p];
            blob.Footer = FshFooterWriter[FshBlobFooterType.ColorPalette].Invoke(blob);
        }
        else
        {
            blob.PixelData = ConvertToRaw(image);
            blob.Magic = ImageToFshBlobMagic(image);
        }
        blob.Width = (ushort)image.Width;
        blob.Height = (ushort)image.Height;
    }

    private static byte[] ConvertToRaw(Image img)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                bw.Write(GetPixelBytes(img, x, y));
            }
        }
        return ms.ToArray();
    }

    private static Color[] CreatePalette()
    {
        static IEnumerable<Color> GetColors()
        {
            foreach (var b in (byte[])[0x00, 0x40, 0x80, 0xbf, 0xff])
                foreach (var g in (byte[])[0x00, 0x24, 0x49, 0x6d, 0x92, 0xb6, 0xdb, 0xff])
                    foreach (var r in (byte[])[0x00, 0x33, 0x66, 0x99, 0xcc, 0xff])
                    {
                        yield return Color.FromRgb(r, g, b);
                    }
        }
        static IEnumerable<Color> GetGrayscale()
        {
            return Enumerable
                .Range(0, 16)
                .Select(p => (byte)(p * 16))
                .Select(p => Color.FromRgb(p, p, p));
        }

        return [.. GetColors().ToArray(), .. GetGrayscale()];
    }
}
