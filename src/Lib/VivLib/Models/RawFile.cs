namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a raw file, which is a file that the corresponding editor in
/// Vivianne can modify directly.
/// </summary>
public class RawFile
{
    /// <summary>
    /// Gets or sets the raw contents of the file.
    /// </summary>
    public byte[] Data { get; set; } = [];
}
