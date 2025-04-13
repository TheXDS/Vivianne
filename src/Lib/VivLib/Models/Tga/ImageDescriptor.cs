namespace TheXDS.Vivianne.Models.Tga;

/// <summary>
/// Enumerates the possible values for the pixel descriptor property in the TGA header.
/// </summary>
[Flags]
public enum ImageDescriptor : byte
{
    /// <summary>
    /// Gets a mask that isolates the "PixelAttribute" value from the descriptor.
    /// </summary>
    PixelAttrMask = 0x0f,

    /// <summary>
    /// Reserved bit. Should be set to 0.
    /// </summary>
    Rsvd = 0x10,

    /// <summary>
    /// Indicates that the origin is at the Top-Left corner of the image.
    /// </summary>
    TopLeftOrigin = 0x20,

    /// <summary>
    /// Gets a mask that isolates the "Interleaving" value from the descriptor.
    /// </summary>
    InterleavingMask = 0xc0,

    /// <summary>
    /// Indicates no interleaving.
    /// </summary>
    NonInterleaved = 0x00,

    /// <summary>
    /// Indicates two-way (even/odd) interleaving.
    /// </summary>
    TwoWayInterleaving = 0x40,

    /// <summary>
    /// Indicates four-way interleaving.
    /// </summary>
    FourWayInterleaving = 0x80,
}