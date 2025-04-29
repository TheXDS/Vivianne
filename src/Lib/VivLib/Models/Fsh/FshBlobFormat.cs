namespace TheXDS.Vivianne.Models.Fsh;

/// <summary>
/// Enumerates the well-known FSH blob pixel formats.
/// </summary>
public enum FshBlobFormat : byte
{
    /// <summary>
    /// Indicates that the FSH blob represents a 32-bit color palette with
    /// 8-bit red, green, blue and alpha channels.
    /// </summary>
    Palette32 = 0x2A,

    /// <summary>
    /// Indicates that the FSH blob represents a 24-bit color palette with
    /// 8-bit red, green and blue channels.
    /// </summary>
    Palette24Dos = 0x22,

    /// <summary>
    /// Indicates that the FSH blob represents a 24-bit color palette with
    /// 8-bit red, green and blue channels.
    /// </summary>
    Palette24 = 0x24,

    /// <summary>
    /// Indicates that the FSH blob represents a 16-bit color palette
    /// </summary>
    Palette16Nfs5 = 0x29,

    /// <summary>
    /// Indicates that the FSH blob represents a 16-bit color palette
    /// </summary>
    Palette16 = 0x2D,

    /// <summary>
    /// Indicates that the FSH blob includes a DXT1 compressed texture.
    /// </summary>
    Dxt1 = 0x60,

    /// <summary>
    /// Indicates that the FSH blob includes a DXT3 compressed texture.
    /// </summary>
    Dxt3 = 0x61,

    /// <summary>
    /// Indicates that the FSH blob includes a DXT5 compressed texture.
    /// </summary>
    Dxt5 = 0x62,

    /// <summary>
    /// Indicates that the FSH blob includes a 16-bit color image in RGBA4444
    /// format, that is, with 4 bits per channel including alpha.
    /// </summary>
    Argb4444 = 0x6D,

    /// <summary>
    /// Indicates that the FSH blob includes a 16-bit color image in RGB565
    /// format, with no alpha channel.
    /// </summary>
    Rgb565 = 0x78,

    /// <summary>
    /// Indicates that the FSH blob includes a 256 color image. If a palette
    /// is not included in the blob's footer or as a companion blob in the
    /// parent FSH file, a common palette should be used (Checks should be
    /// performed in that order).
    /// </summary>
    Indexed8 = 0x7B,

    /// <summary>
    /// Indicates that the FSH blob includes a 32-bit color image in ARGB32
    /// format.
    /// </summary>
    Argb32 = 0x7D,

    /// <summary>
    /// Indicates that the FSH blob includes a 16-bit color image in ARGB1555
    /// format, that is, with a 1 bit alpha channel.
    /// </summary>
    Argb1555 = 0x7E,

    /// <summary>
    /// Indicates that the FSH blob includes a 24-bit color image in RGB24
    /// format with 8-bit red, green and blue channels.
    /// </summary>
    Rgb24 = 0x7F,

    /// <summary>
    /// Indicates that the FSH blob includes a compressed 16-bit color image in
    /// RGB565 format, with no alpha channel. After LZ decompression, the pixel
    /// data should be treated the same as <see cref="Rgb565"/>.
    /// </summary>
    LzRgb565 = 0xF8,

    /// <summary>
    /// Indicates that the FSH blob includes a compressed ARGB32 stream. After
    /// LZ decompression, the pixel data should be treated the same as
    /// <see cref="Argb32"/>.
    /// </summary>
    LzArgb32 = 0xFD,

    /// <summary>
    /// Indicates that the FSH blob includes a compressed ARGB1555 stream.
    /// After LZ decompression, the pixel data should be treated the same as
    /// <see cref="Argb1555"/>.
    /// </summary>
    LzArgb1555 = 0xFE,
}
