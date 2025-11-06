using System.Linq;
using TheXDS.Ganymede.Models;

namespace TheXDS.Vivianne.Data;

/// <summary>
/// Describes a file format supported by Vivianne.
/// </summary>
/// <param name="FileExtensions">File extensions to associate.</param>
/// <param name="ContentVisualizerFactory">
/// Factory used to generate a ViewModel to visualize or edit the file.
/// </param>
/// <param name="ProgId">Program ID to use for file type registrations.</param>
/// <param name="FileDescription">File type description.</param>
/// <param name="IsPrimary">
/// Determines whether to register Vivianne as the primary application to open
/// the file type.
/// </param>
/// <param name="IconIndex">
/// Resource index of the icon to use for the file type association.
/// </param>
public readonly record struct FileTypeInfo(
    string[] FileExtensions,
    ContentVisualizerViewModelFactory ContentVisualizerFactory,
    string ProgId,
    string FileDescription,
    bool IsPrimary,
    int IconIndex)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileTypeInfo"/> struct.
    /// </summary>
    /// <param name="fileExtension">File extension to associate.</param>
    /// <param name="contentVisualizerFactory">
    /// Factory used to generate a ViewModel to visualize or edit the file.
    /// </param>
    /// <param name="fileDescription">File type description.</param>
    /// <param name="isPrimary">
    /// Determines whether to register Vivianne as the primary application to open
    /// the file type.
    /// </param>
    /// <param name="iconIndex">
    /// Resource index of the icon to use for the file type association.
    /// </param>
    public FileTypeInfo(
        string fileExtension,
        ContentVisualizerViewModelFactory contentVisualizerFactory,
        string fileDescription,
        bool isPrimary = true,
        int iconIndex = 0)
        : this([fileExtension], contentVisualizerFactory, $"TheXDS.Vivianne{fileExtension}", fileDescription, isPrimary, iconIndex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTypeInfo"/> struct.
    /// </summary>
    /// <param name="fileExtensions">File extensions to associate.</param>
    /// <param name="contentVisualizerFactory">
    /// Factory used to generate a ViewModel to visualize or edit the file.
    /// </param>
    /// <param name="progId">Program ID to use for file type registrations.</param>
    /// <param name="fileDescription">File type description.</param>
    /// <param name="isPrimary">
    /// Determines whether to register Vivianne as the primary application to open
    /// the file type.
    /// </param>
    public FileTypeInfo(
        string[] fileExtensions,
        ContentVisualizerViewModelFactory contentVisualizerFactory,
        string progId,
        string fileDescription,
        bool isPrimary = true)
        : this(fileExtensions, contentVisualizerFactory, progId, fileDescription, isPrimary, 5)        
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FileTypeInfo"/> struct.
    /// </summary>
    /// <param name="fileExtensions">File extensions to associate.</param>
    /// <param name="contentVisualizerFactory">
    /// Factory used to generate a ViewModel to visualize or edit the file.
    /// </param>
    /// <param name="progId">Program ID to use for file type registrations.</param>
    /// <param name="fileDescription">File type description.</param>
    /// <param name="iconIndex">
    /// Resource index of the icon to use for the file type association.
    /// </param>
    public FileTypeInfo(
        string[] fileExtensions,
        ContentVisualizerViewModelFactory contentVisualizerFactory,
        string progId,
        string fileDescription,
        int iconIndex)
        : this(fileExtensions, contentVisualizerFactory, progId, fileDescription, true, iconIndex)
    {
    }

    /// <summary>
    /// Gets an array of <see cref="FileFilterItem"/> objects that describe the
    /// file filters to use for opening or saving a file represented by this
    /// value.
    /// </summary>
    public FileFilterItem[] Filters { get; } = [..FileExtensions.Select(p => new FileFilterItem(FileDescription, $"*{p}")), FileFilterItem.AllFiles];

    /// <summary>
    /// Gets an array of <see cref="FileFilterItem"/> objects that describe the
    /// file filters to use for saving a file represented by this value.
    /// </summary>
    public FileFilterItem[] SaveFilters { get; init; } = [.. FileExtensions.Select(p => new FileFilterItem(FileDescription, $"*{p}")), FileFilterItem.AllFiles];
}
