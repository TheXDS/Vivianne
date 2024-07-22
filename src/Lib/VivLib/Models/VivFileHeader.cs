namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the full VIV file header after the directory has been parsed.
/// </summary>
/// <param name="header">Essential VIV file header</param>
/// <param name="entries">Directory information of the viv file.</param>
public class VivFileHeader(VivHeader header, Dictionary<string, VivDirectoryEntry> entries)
{
    /// <summary>
    /// Gets or sets the essential VIV file header. this does not include the directory.
    /// </summary>
    public VivHeader Header { get; } = header;

    /// <summary>
    /// Gets or sets the directory information of the viv file.
    /// </summary>
    public Dictionary<string, VivDirectoryEntry> Entries { get; } = entries;
}
