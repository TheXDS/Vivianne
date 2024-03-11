namespace TheXDS.Vivianne.Models;

/// <summary>
/// Enumerates the well-known GIMX blob pixel formats.
/// </summary>
public enum GimxFormat : byte
{
    /// <summary>
    /// Indicates that the GIMX blob represents a 32-bit, 256 color palette
    /// </summary>
    Palette = 0x2A,
    /// <summary>
    /// Indicates that the GIMX blob includes a 256 color image. If a palette
    /// is not included as a companion blob in the parent FSH file, a common
    /// palette should be used.
    /// </summary>
    Indexed8 = 0x7B,
    /// <summary>
    /// Indicates that the GIMX blob includes a 16-bit color image in BGR565
    /// format.
    /// </summary>
    Bgr565 = 0x78,
    /// <summary>
    /// Indicates that the GIMX blob includes a 32-bit color image in RGBA32
    /// format.
    /// </summary>
    Bgra32 = 0x7D,
}
