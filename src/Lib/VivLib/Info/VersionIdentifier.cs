namespace TheXDS.Vivianne.Info;

/// <summary>
/// Contains functions that help identify the game version for which a file is
/// intended.
/// </summary>
public static class VersionIdentifier
{
    private static readonly byte[][] knownFce4Headers =
    [
        [0x00, 0x10, 0x10, 0x14],
        [0x14, 0x10, 0x10, 0x00],
    ];

    private static readonly byte[][] knownFce4MHeaders =
    [
        [0x00, 0x10, 0x10, 0x15],
        [0x15, 0x10, 0x10, 0x00],
    ];

    /// <summary>
    /// Infers the game file version for the specified FCE contents.
    /// </summary>
    /// <param name="file">File contents to check.</param>
    /// <returns>
    /// A value that indicates the game for which this file is intended.
    /// </returns>
    public static NfsVersion FceVersion(byte[] file)
    {
        if (knownFce4Headers.Any(file[0..4].SequenceEqual)) return NfsVersion.Nfs4;
        return knownFce4MHeaders.Any(file[0..4].SequenceEqual) ? NfsVersion.Mco : NfsVersion.Nfs3;
    }

    /// <summary>
    /// Infers the game file version for the specified FeData contents.
    /// </summary>
    /// <param name="file">File contents to check.</param>
    /// <returns>A value that indicates the game for which this file is intended.</returns>
    public static NfsVersion FeDataVersion(byte[] file) => file[0] == 4 ? NfsVersion.Nfs4 : NfsVersion.Nfs3;

    /// <summary>
    /// Infers the game file version for the specified Carp contents.
    /// </summary>
    /// <param name="file">File contents to check.</param>
    /// <returns>A value that indicates the game for which this file is intended.</returns>
    public static NfsVersion CarpVersion(byte[] file) => System.Text.Encoding.Latin1.GetString(file).Contains("understeer gradient(80)") ? NfsVersion.Nfs4 : NfsVersion.Nfs3;
}
