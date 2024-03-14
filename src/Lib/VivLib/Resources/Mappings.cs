using System.Drawing.Imaging;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models;
using DC = System.Drawing.Color;

namespace TheXDS.Vivianne.Resources;

/// <summary>
/// Contains a set of resources to map GIMX pixel formats to different value
/// types as required.
/// </summary>
public static class Mappings
{
    private static readonly Rgb565ColorParser Rgb565 = new();
#if EnableFullFshFormat
    private static readonly Rgba16ColorParser Rgba1555 = new();
#endif

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding
    /// <see cref="PixelFormat"/> to use on drawing functions.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFormat, PixelFormat> FshBlobToPixelFormat { get; } = new Dictionary<FshBlobFormat, PixelFormat>()
    {
        { FshBlobFormat.Indexed8,   PixelFormat.Format8bppIndexed },
        { FshBlobFormat.Rgb565,     PixelFormat.Format16bppRgb565 },
        { FshBlobFormat.Argb32,     PixelFormat.Format32bppArgb },
#if EnableFullFshFormat
        { FshBlobFormat.Rgb24,      PixelFormat.Format24bppRgb },
        { FshBlobFormat.Argb1555,   PixelFormat.Format16bppArgb1555 },
#endif
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding label that
    /// describes the GIMX pixel format.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFormat, string> FshBlobToLabel { get; } = new Dictionary<FshBlobFormat, string>()
    {
        { FshBlobFormat.Palette32,      "32-bit color palette" },
        { FshBlobFormat.Indexed8,       "8-bit color (256 colors) with palette" },
        { FshBlobFormat.Rgb565,         "16-bit color (RGB565), no alpha" },
        { FshBlobFormat.Argb32,         "24-bit color with 8-bit alpha channel (ARGB32)" },
#if EnableFullFshFormat
        { FshBlobFormat.Palette24Dos,   "24-bit color palette, DOS variant" },
        { FshBlobFormat.Palette24,      "24-bit color palette" },
        { FshBlobFormat.Palette16Nfs5,  "16-bit color palette, NFS5 variant" },
        { FshBlobFormat.Palette16,      "16-bit color palette" },
        { FshBlobFormat.Rgb24,          "24-bit color without alpha (RGB24)" },
        { FshBlobFormat.Argb1555,       "16-bit Color with 1 bit alpha channel (ARGB1555)" },
        { FshBlobFormat.Dxt3,           "DXT3 compressed texture" },
        { FshBlobFormat.Dxt4,           "DXT4 compressed texture" },
#endif
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding integer value
    /// that indicates the number of bytes that conform a single pixel.
    /// </summary>
    public static IReadOnlyDictionary<FshBlobFormat, byte> FshBlobBytesPerPixel { get; } = new Dictionary<FshBlobFormat, byte>()
    {
        { FshBlobFormat.Palette32,     4 },
        { FshBlobFormat.Indexed8,      1 },
        { FshBlobFormat.Rgb565,        2 },
        { FshBlobFormat.Argb32,        4 },
#if EnableFullFshFormat
        { FshBlobFormat.Palette24Dos,  3 },
        { FshBlobFormat.Palette24,     3 },
        { FshBlobFormat.Palette16Nfs5, 2 },
        { FshBlobFormat.Palette16,     2 },
        { FshBlobFormat.Rgb24,         3 },
        { FshBlobFormat.Argb1555,      2 },
#endif
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="FshBlobFormat"/> value to a corresponding delegate that
    /// can be used to convert a pixel into a byte array.
    /// </summary>
    /// <remarks>
    /// This dictionary intentionally ommits <see cref="FshBlobFormat.Indexed8"/>
    /// because pixel values for this format will depend on a color palette
    /// which is not be available on the same data stream.
    /// </remarks>
    public static IReadOnlyDictionary<FshBlobFormat, Func<DC, byte[]>> FshBlobToPixelWriter { get; } = new Dictionary<FshBlobFormat, Func<DC, byte[]>>()
    {
        { FshBlobFormat.Palette32, c => [c.B, c.G, c.R, c.A] },
        { FshBlobFormat.Rgb565,    c => BitConverter.GetBytes(Rgb565.To(c)) },
        { FshBlobFormat.Argb32,    c => [c.B, c.G, c.R, c.A] },
#if EnableFullFshFormat
        { FshBlobFormat.Rgb24,     c => [c.B, c.G, c.R] },
        { FshBlobFormat.Argb1555,  c => BitConverter.GetBytes(Rgba1555.To(c)) },
#endif
    }.AsReadOnly();
}