using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.Models.Fsh;

/// <summary>
/// Represents a FSH textures file.
/// </summary>
public class FshFile
{
    /// <summary>
    /// Collection of blobs contained in this FSH.
    /// </summary>
    public Dictionary<string, FshBlob> Entries { get; } = [];

    /// <summary>
    /// Gets or sets this FSH file Directory Id.
    /// </summary>
    /// <remarks>
    /// By default, this value is set to '<c>GIMX</c>', which is the default
    /// directory ID for Need For Speed 3/4.
    /// </remarks>
    public string DirectoryId { get; set; } = Mappings.FshDirectoryIds[0];

    /// <summary>
    /// Gets or sets a value that indicates if the underlying file storage
    /// should use compression.
    /// </summary>
    public bool IsCompressed { get; set; }

    /// <summary>
    /// Gets or sets the extra data that may exist after the FSH header.
    /// </summary>
    public byte[] ExtraData { get; set; } = [];
}
