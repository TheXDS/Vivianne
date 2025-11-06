using System.Collections.Generic;
using TheXDS.Ganymede.Models;
using TheXDS.Vivianne.Data;
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
    public static IEnumerable<FileFilterItem> VivFileFilter { get; } = FileTypes.GetInfo(KnownFileType.Viv).Filters;

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving FSH files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FshFileFilter { get; } = [FileTypes.GetInfo(KnownFileType.Shpi).Filters[0], FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving QFS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> QfsFileFilter { get; } = [FileTypes.GetInfo(KnownFileType.Shpi).Filters[1], FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving FCE files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FceFileFilter { get; } = FileTypes.GetInfo(KnownFileType.Fce).Filters;

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving BNK files.
    /// </summary>
    public static IEnumerable<FileFilterItem> BnkFileFilter { get; } = FileTypes.GetInfo(KnownFileType.Bnk).Filters;

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening
    /// either ASF or MUS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> AsfMusOpenFilter { get; } = [new FileFilterItem(St.AsfMusFile, ["*.mus", "*.asf"]), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for saving either
    /// ASF or MUS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> AsfMusSaveFilter { get; } = [
        new FileFilterItem(St.AsfFile, ["*.asf"]),
        new FileFilterItem(St.MusFile, ["*.mus"]),
        FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving FeData files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FeDataFileFilter { get; } = FileTypes.GetInfo(KnownFileType.FeData).Filters;

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening
    /// either FSH or QFS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FshQfsOpenFileFilter { get; } = FileTypes.GetInfo(KnownFileType.Shpi).Filters;

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for saving either
    /// FSH or QFS files.
    /// </summary>
    public static IEnumerable<FileFilterItem> FshQfsSaveFileFilter { get; } = FileTypes.GetInfo(KnownFileType.Shpi).SaveFilters;

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for importing any
    /// kind of file commonly hosted inside a VIV.
    /// </summary>
    public static IEnumerable<FileFilterItem> AnyVivContentFilter { get; } = [new FileFilterItem(St.CommonVIVContentFiles, ["*.fce" , "*.tga", "*.fsh", "*.qfs", "*.txt", "fedata.*", "*.bnk", "*.geo", "*.ctb", "*.ltb"]), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filters to be used on file dialogs for opening and
    /// saving audio files.
    /// </summary>
    public static IEnumerable<FileFilterItem> AudioFileFilter { get; } = [new FileFilterItem(St.WavFile, "*.wav"), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets a set of file filter to be used on file dialogs for saving files
    /// with common bitmap file formats.
    /// </summary>
    public static IEnumerable<FileFilterItem> CommonBitmapSaveFormats { get; } = FileTypes.GetInfo(KnownFileType.Picture).SaveFilters;

    /// <summary>
    /// Gets a set of file filter to be used on file dialogs for opening files
    /// with common bitmap file formats.
    /// </summary>
    public static IEnumerable<FileFilterItem> CommonBitmapOpenFormats { get; } = FileTypes.GetInfo(KnownFileType.Picture).Filters;
}
