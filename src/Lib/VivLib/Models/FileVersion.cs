namespace TheXDS.Vivianne.Models;

/// <summary>
/// Enumerates the possible NFS versions that a file could be for.
/// </summary>
[Flags]
public enum FileVersion : byte
{
    /// <summary>
    /// File is not supported.
    /// </summary>
    Unsupported = 0,
    /// <summary>
    /// File is for NFS3.
    /// </summary>
    Nfs3 = 1,
    /// <summary>
    /// File is for NFS4.
    /// </summary>
    Nfs4 = 2
}