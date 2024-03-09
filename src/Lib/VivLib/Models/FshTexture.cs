namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a FSH textures file.
/// </summary>
public class FshTexture
{
    /// <summary>
    /// Collection of images contained in this FSH.
    /// </summary>
    public Dictionary<string, Gimx> Images { get; } = [];
}
