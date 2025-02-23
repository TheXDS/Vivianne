namespace TheXDS.Vivianne.Models.Tga;

/// <summary>
/// Enumerates the existing color map types for a TGA file.
/// </summary>
public enum ColorMapType : byte
{
    /// <summary>
    /// Indicates that the image contains no color map.
    /// </summary>
    NoColorMap,

    /// <summary>
    /// Indicates that the image contains a color map.
    /// </summary>
    ColorMap,
}
