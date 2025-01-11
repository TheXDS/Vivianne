using System.Collections.Generic;
using TheXDS.Ganymede.Models;
using St = TheXDS.Vivianne.Resources.Strings.Views.StartupView;

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
    public static IEnumerable<FileFilterItem> FceFileFilter { get; } = [new FileFilterItem("FCE 3D model", "*.fce"), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening
    /// either FSH or QFS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FshQfsOpenFileFilter { get; } = [new FileFilterItem("FSH/QFS texture", [ "*.fsh", "*.qfs"]), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for saving either
    /// FSH or QFS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FshQfsSaveFileFilter { get; } = [new FileFilterItem(St.FshFile, "*.fsh"), new FileFilterItem(St.QfsFile, "*.qfs"), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for importing any
    /// kind of file commonly hosted inside a VIV.
    /// </summary>
    public static IEnumerable<FileFilterItem> AnyVivContentFilter { get; } = [new FileFilterItem("Common VIV content files", ["car*.fce", "car*.tga", "car*.fsh", "dash*.qfs", "carp*.txt", "fedata.*", "*.bnk"]), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filter to be used on file dialogs for saving files
    /// with common bitmap file formats.
    /// </summary>
    public static IEnumerable<FileFilterItem> CommonBitmapSaveFormats { get; } = [
        new FileFilterItem("Portable Network Graphics", "*.png"),
        new FileFilterItem("GIF image", "*.gif"),
        new FileFilterItem("JPEG picture", ["*.jpg", "*.jpeg"]),
        new FileFilterItem("BMP bitmap image", "*.bmp"),
        FileFilterItem.AllFiles
        ];

    /// <summary>
    /// Gets a set of file filter to be used on file dialogs for opening files
    /// with common bitmap file formats.
    /// </summary>
    public static IEnumerable<FileFilterItem> CommonBitmapOpenFormats { get; } = [
        new FileFilterItem("Common image files", ["*.png", "*.gif", "*.jpg", "*.jpeg", "*.bmp"]),
        FileFilterItem.AllFiles
    ];
}
