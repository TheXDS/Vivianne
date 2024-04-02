namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a VIV file that can be edited in memory.
/// </summary>
public class VivFile
{
    /// <summary>
    /// Collection of files contained in this VIV.
    /// </summary>
    public Dictionary<string, byte[]> Directory { get; } = [];
}