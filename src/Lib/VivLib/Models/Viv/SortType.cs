using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Models.Viv;

/// <summary>
/// Enumerates the possible VIV sorting options in Vivianne.
/// </summary>
public enum SortType
{
    /// <summary>
    /// No sorting. Files should be laid out as declared on the VIV directory.
    /// </summary>
    [Name("No sorting (as in VIV directory)")]Directory,

    /// <summary>
    /// Sort by file name.
    /// </summary>
    [Name("Sort by file name")]FileName,

    /// <summary>
    /// Sort by file type, then by file name.
    /// </summary>
    [Name("Sort by type, then by file name")] FileType,

    /// <summary>
    /// Sort by file kind, then by file name.
    /// </summary>
    [Name("Sort by kind, then by file name")] FileKind,

    /// <summary>
    /// Sort by file size.
    /// </summary>
    [Name("Sort by file size, desc.")]FileSize,

    /// <summary>
    /// Sort by file offset.
    /// </summary>
    [Name("Sort by file offset")]FileOffset
}

