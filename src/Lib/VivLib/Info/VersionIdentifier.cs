using TheXDS.Vivianne.Codecs;

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
    public static NfsVersion FceVersion(byte[] file) => FceVersion(BitConverter.ToInt32(file.AsSpan()[0..4]));

    /// <summary>
    /// Infers the game file version for the specified FCE contents.
    /// </summary>
    /// <param name="stream">Stream to the file contents to check.</param>
    /// <returns>
    /// A value that indicates the game for which this file is intended.
    /// </returns>
    public static NfsVersion FceVersion(Stream stream)
    {
        if (!stream.CanSeek) return NfsVersion.Unknown;
        var data = new byte[4];
        stream.ReadExactly(data);
        stream.Seek(0, SeekOrigin.Begin);
        return FceVersion(data);
    }

    /// <summary>
    /// Infers the game file version for the specified FCE contents.
    /// </summary>
    /// <param name="magic">File magic signature to check.</param>
    /// <returns>
    /// A value that indicates the game for which this file is intended.
    /// </returns>
    public static NfsVersion FceVersion(int magic)
    {
        if (knownFce4Headers.Any(p => magic == BitConverter.ToInt32(p))) return NfsVersion.Nfs4;
        return knownFce4MHeaders.Any(p => magic == BitConverter.ToInt32(p)) ? NfsVersion.Mco : NfsVersion.Nfs3;
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
    public static NfsVersion CarpVersion(byte[] file)
    {
        if (RefPackCodec.IsCompressed(file)) file = RefPackCodec.Decompress(file);
        if (file.Length == 356) return NfsVersion.Nfs2;

        var rawString = System.Text.Encoding.Latin1.GetString(file);

        return rawString.Contains("understeer gradient(80)") ? NfsVersion.Nfs4 
            : rawString.StartsWith("Serial") ? NfsVersion.Nfs3 : NfsVersion.Unknown;
    }
}
