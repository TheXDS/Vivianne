namespace TheXDS.Vivianne.Info;

/// <summary>
/// Enumerates the possible game versions that a file could be intended for.
/// </summary>
public enum NfsVersion
{
    /// <summary>
    /// Unable to determine the target file version.
    /// </summary>
    Unknown,
    /// <summary>
    /// The file is intended for The Need For Speed (AKA, Need For Speed 1)
    /// </summary>
    Tnfs,
    /// <summary>
    /// The File is intended for Need For Speed 2.
    /// </summary>
    Nfs2,
    /// <summary>
    /// The file is intended for Need For Speed 3.
    /// </summary>
    Nfs3,
    /// <summary>
    /// The File is intended for Need For Speed 4.
    /// </summary>
    Nfs4,
    /// <summary>
    /// The file is intended for Motor City Online.
    /// </summary>
    Mco
}
