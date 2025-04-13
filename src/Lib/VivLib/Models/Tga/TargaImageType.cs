namespace TheXDS.Vivianne.Models.Tga;

/// <summary>
/// Enumerates the possible image formats for a TGA file.
/// </summary>
public enum TargaImageType : byte
{
    /// <summary>
    /// No image. Would go unused for any valid TGA file.
    /// </summary>
    NoImage = 0,

    /// <summary>
    /// Indexed image. The TGA should include a color map.
    /// </summary>
    Indexed = 1,

    /// <summary>
    /// True color image. The TGA contains RAW pixel data.
    /// </summary>
    TrueColor = 2,

    /// <summary>
    /// Grayscale image. The TGA contains RAW grayscale per-pixel intensity
    /// data.
    /// </summary>
    Grayscale = 3,

    /// <summary>
    /// Indexed image with RLE compression.
    /// </summary>
    RleIndexed = 9,

    /// <summary>
    /// True color image with RLE compression.
    /// </summary>
    RleTrueColor = 10,

    /// <summary>
    /// Grayscale image with RLE compression.
    /// </summary>
    RleGrayscale = 11,
}
