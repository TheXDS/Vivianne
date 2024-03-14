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
    public string DirectoryId { get; set; } = "GIMX";
}