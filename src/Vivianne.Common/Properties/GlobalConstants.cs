namespace TheXDS.Vivianne.Properties;

/// <summary>
/// Includes general constants that can be used across the entire application.
/// </summary>
public static class GlobalConstants
{
    /// <summary>
    /// Returns a list of all known process names for NFS2, including all the
    /// different versions and editions of the game.
    /// </summary>
    public static readonly string[] KnownNfs2ProcesNames = [
        "nfs2se-gl1",
        "nfs2se",
        "nfs2sea",
        "nfs2-gl1",
        "nfs2",
        "nfs2a",
    ];

    /// <summary>
    /// Returns a list of all known process names for Need For Speed games,
    /// from NFS2 to NFS4, including all of their variants.
    /// </summary>
    public static readonly string[] KnownNfsProcessNames = [
        .. KnownNfs2ProcesNames,
        "nfs3",
        "nfs4"];
}
