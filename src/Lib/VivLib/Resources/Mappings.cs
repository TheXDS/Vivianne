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

    /// <summary>
    /// Maps a <see cref="GimxFormat"/> value to a corresponding
    /// <see cref="PixelFormat"/> to use on drawing functions.
    /// </summary>
    public static IReadOnlyDictionary<GimxFormat, PixelFormat> GimxToPixelFormat { get; } = new Dictionary<GimxFormat, PixelFormat>()
    {
        { GimxFormat.Indexed8,  PixelFormat.Format8bppIndexed },
        { GimxFormat.Rgb565,    PixelFormat.Format16bppRgb565 },
        { GimxFormat.Argb32,    PixelFormat.Format32bppArgb },
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="GimxFormat"/> value to a corresponding label that
    /// describes the GIMX pixel format.
    /// </summary>
    public static IReadOnlyDictionary<GimxFormat, string> GimxToLabel { get; } = new Dictionary<GimxFormat, string>()
    {
        { GimxFormat.Indexed8,  "8-bit color (256 colors) with palette" },
        { GimxFormat.Rgb565,    "16-bit color (RGB565), no alpha" },
        { GimxFormat.Argb32,    "24-bit color with 8-bit alpha channel (ARGB32)" },
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="GimxFormat"/> value to a corresponding integer value
    /// that indicates the number of bytes that conform a single pixel.
    /// </summary>
    public static IReadOnlyDictionary<GimxFormat, byte> GimxBytesPerPixel { get; } = new Dictionary<GimxFormat, byte>()
    {
        {GimxFormat.Palette,    4}, // 32-bit 256 Color palette
        {GimxFormat.Indexed8,   1}, // 1 byte per pixel (256 colors)
        {GimxFormat.Rgb565,     2}, // 2 bytes per pixel (16 bit color).
        {GimxFormat.Argb32,     4}, // 4 bytes per pixel (RGBA32)
    }.AsReadOnly();

    /// <summary>
    /// Maps a <see cref="GimxFormat"/> value to a corresponding delegate that
    /// can be used to convert a pixel into a byte array.
    /// </summary>
    public static IReadOnlyDictionary<GimxFormat, Func<DC, byte[]>> GimxToPixelWriter { get; } = new Dictionary<GimxFormat, Func<DC, byte[]>>()
    {
        { GimxFormat.Palette,   c => [c.B, c.G, c.R, c.A] },
        { GimxFormat.Rgb565,    c => BitConverter.GetBytes(Rgb565.To(c)) },
        { GimxFormat.Argb32,    c => [c.B, c.G, c.R, c.A] },
    }.AsReadOnly();
}