namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a FSH textures file.
/// </summary>
public class FshFile
{
    /// <summary>
    /// Collection of images contained in this FSH.
    /// </summary>
    public Dictionary<string, FshBlob> Entries { get; } = [];

    /// <summary>
    /// Gets or sets this FSH file Directory Id.
    /// </summary>
    /// <remarks>
    /// By default, this value is set to '<c>GIMX</c>', which is the default
    /// directory ID for Need For Speed 3/4.
    /// </remarks>
    public string DirectoryId { get; set; } = "GIMX";

    /// <summary>
    /// Gets or sets a value that indicates if the file stream is a compressed
    /// QFS file.
    /// </summary>
    public bool IsCompressed { get; set; }
}
