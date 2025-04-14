﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers.Fsh;
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
    /// <param name="blob">Image blob to export.</param>
    /// <param name="palette">
    /// Specifies the Color palette to use in case the FSH blob uses the
    /// <see cref="FshBlobFormat.Indexed8"/> pixel format.
    /// </param>
    /// <returns>
    /// A new <see cref="Image"/> instance.
    /// </returns>
    public static Image? ToImage(this FshBlob blob, Color[]? palette = null)
    {
        if (FshBlobPixelReader.TryGetValue(blob.Magic, out var callback))
        {
            return callback.Invoke(blob.PixelData, blob.Width, blob.Height);
        }
        if (blob.Magic != FshBlobFormat.Indexed8) return null;
        palette ??= ReadLocalPalette(blob) ?? CreatePalette();
        return LoadFromIndexedBlob(blob, palette, p => p.ToPixel<Bgra32>());
    }

    /// <summary>
    /// Replaces the blob's image data from an image.
    /// </summary>
    /// <param name="blob">Blob to replace the data in.</param>
    /// <param name="image">Image from which to load the new data.</param>
    /// <param name="palette">
    /// Specifies the Color palette to use in case the FSH blob uses the
    /// <see cref="FshBlobFormat.Indexed8"/> pixel format.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no data encoding for the specified FSH blob format has been
    /// implemented.
    /// </exception>
    public static void ReplaceWith(this FshBlob blob, Image image, Color[]? palette = null)
    {
        if (blob.Magic == FshBlobFormat.Indexed8)
        {
            var quantizerFactory = new OctreeQuantizer(new QuantizerOptions { MaxColors = 256 });
            var quantizer = quantizerFactory.CreatePixelSpecificQuantizer<Rgba32>(Configuration.Default);

            quantizer.BuildPalette(new ExtensivePixelSamplingStrategy(), (Image<Rgba32>)image);
            palette = [.. quantizer.Palette.ToArray().Select(p => new Color(p))];

            using var q = quantizer.QuantizeFrame((ImageFrame<Rgba32>)image.Frames[0], new Rectangle(0, 0, image.Width, image.Height));
            var p = new List<byte>();
            for (int y = 0; y < image.Height; y++)
            {
                p.AddRange(q.DangerousGetRowSpan(y));
            }
            blob.PixelData = [.. p];
            blob.WriteLocalPalette(palette);
        }
        else
        {
            blob.PixelData = ConvertToRaw(image);
            blob.Magic = ImageToFshBlobMagic(image);
        }
        blob.Width = (ushort)image.Width;
        blob.Height = (ushort)image.Height;
    }

    /// <summary>
    /// Loads a color palette from the specified blob.
    /// </summary>
    /// <param name="blob">
    /// Blob that contains a color palette
    /// for the image.
    /// </param>
    /// <returns>
    /// An array of <see cref="Color"/> with the color palette to use when
    /// rendering the texture.
    /// </returns>
    public static Color[]? ReadLocalPalette(this FshBlob blob)
    {
        if (blob.FooterType() != FshBlobFooterType.ColorPalette) return null;
        var s = new FshBlobSerializer();
        using var ms = new MemoryStream(blob.Footer);
        var paletteBlob = s.Deserialize(ms)!;
        return FshBlobToPalette[paletteBlob.Magic].Invoke(paletteBlob);
    }

    /// <summary>
    /// Writes a local color palette to the footer of a <see cref="FshBlob"/>.
    /// </summary>
    /// <param name="blob">
    /// FSH blob to write the local color palette to.
    /// </param>
    /// <param name="colors">
    /// Contents of the color table. Must have no more than 256 colors.
    /// Currently, parsers only support tables with exactly 256 colors.
    /// </param>
    public static void WriteLocalPalette(this FshBlob blob, IEnumerable<Color> colors)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        var colorTable = colors.ToArray();
        foreach (var j in colorTable)
        {
            Rgba32 c = j.ToPixel<Rgba32>();
            bw.Write([c.B, c.G, c.R, c.A]);
        }
        var b = new FshBlob()
        {
            Magic = FshBlobFormat.Palette32,
            Width = (ushort)colorTable.Length,
            Height = 1,
            PixelData = ms.ToArray()
        };
        using var ms2 = new MemoryStream();
        new FshBlobSerializer().SerializeTo(b, ms2);
        blob.Footer = ms2.ToArray();
    }

    /// <summary>
    /// Gets a value that indicates the inferred type of the footer data in the
    /// specified <see cref="FshBlob"/>.
    /// </summary>
    /// <param name="blob">FSH blob to read the footer data from.</param>
    /// <returns>
    /// A value that indicates the type of data that the footer in
    /// <paramref name="blob"/> seems to contain.
    /// </returns>
    public static FshBlobFooterType FooterType(this FshBlob blob)
    {
        return FshBlobFooterIdentifier.FirstOrDefault(p => p.Predicate.Invoke(blob.Footer))?.Value ?? FshBlobFooterType.Unknown;
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

    private static readonly byte[] paletteHeader = [0x2a, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00];

    private record class FooterIdentifierElement(FshBlobFooterType Value, Func<byte[], bool> Predicate);

    private static IEnumerable<FooterIdentifierElement> FshBlobFooterIdentifier { get; } = [
        new(FshBlobFooterType.None,            b => b is null || b.Length == 0),
        new(FshBlobFooterType.CarDashboard,    b => b.Length == 104),
        new(FshBlobFooterType.ColorPalette,    b => b.Length == 1040 && b[0..8].SequenceEqual(paletteHeader)),
        new(FshBlobFooterType.Padding,         b => b.All(p => p == 0)),
    ];
}
