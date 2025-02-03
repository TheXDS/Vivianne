using System.Collections.Generic;
using TheXDS.Ganymede.Models;
using St = TheXDS.Vivianne.Resources.Strings.FileFilters;

namespace TheXDS.Vivianne.Resources;

/// <summary>
/// Contains a set of common values and constants to be used by Vivianne.
/// </summary>
public static class FileFilters
{
    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving VIV files.
    /// </summary>
    public static IEnumerable<FileFilterItem> VivFileFilter { get; } = [new FileFilterItem(St.VivFile, "*.viv"), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving FSH files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FshFileFilter { get; } = [new FileFilterItem(St.FshFile, "*.fsh"), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving QFS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> QfsFileFilter { get; } = [new FileFilterItem(St.QfsFile, "*.qfs"), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving FCE files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FceFileFilter { get; } = [new FileFilterItem(St.FCE3DModel, "*.fce"), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving BNK files.
    /// </summary>
    public static IEnumerable<FileFilterItem> BnkFileFilter { get; } = [new FileFilterItem(St.BNKAudioFile, "*.bnk"), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving FeData files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FeDataFileFilter { get; } = [new FileFilterItem(St.LocalizedCarFeData, ["*.bri", "*.eng", "*.fre", "*.ger", "*.ita", "*.spa", "*.swe"]), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening
    /// either FSH or QFS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FshQfsOpenFileFilter { get; } = [new FileFilterItem(St.FSHQFSTexture, ["*.fsh", "*.qfs"]), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for saving either
    /// FSH or QFS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FshQfsSaveFileFilter { get; } = [new FileFilterItem(St.FshFile, "*.fsh"), new FileFilterItem(St.QfsFile, "*.qfs"), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for importing any
    /// kind of file commonly hosted inside a VIV.
    /// </summary>
    public static IEnumerable<FileFilterItem> AnyVivContentFilter { get; } = [new FileFilterItem(St.CommonVIVContentFiles, ["car*.fce", "car*.tga", "car*.fsh", "dash*.qfs", "carp*.txt", "fedata.*", "*.bnk"]), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filter to be used on file dialogs for saving files
    /// with common bitmap file formats.
    /// </summary>
    public static IEnumerable<FileFilterItem> CommonBitmapSaveFormats { get; } = [
        new FileFilterItem(St.PortableNetworkGraphics, "*.png"),
        new FileFilterItem(St.GIFImage, "*.gif"),
        new FileFilterItem(St.JPEGPicture, ["*.jpg", "*.jpeg"]),
        new FileFilterItem(St.BMPBitmapImage, "*.bmp"),
        FileFilterItem.AllFiles
        ];

    /// <summary>
    /// Gets a set of file filter to be used on file dialogs for opening files
    /// with common bitmap file formats.
    /// </summary>
    public static IEnumerable<FileFilterItem> CommonBitmapOpenFormats { get; } = [
        new FileFilterItem(St.CommonImageFiles, ["*.png", "*.gif", "*.jpg", "*.jpeg", "*.bmp"]),
        FileFilterItem.AllFiles
    ];
}
