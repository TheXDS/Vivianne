using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Serializers;
using St = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Base class for ViewModels that can launch file viewers for a specific file type.
/// </summary>
/// <typeparam name="TFile">Type of file to be visualized in the generated ViewModel.</typeparam>
/// <typeparam name="TSerializer">Type of serializer that can be used to read file data.</typeparam>
/// <typeparam name="TViewModel">Type of ViewModel to launch upon invocation.</typeparam>
public abstract class FileViewModelLauncherBase<TFile, TSerializer, TViewModel> : ViewModel, IFileViewerViewModelLauncher
    where TFile : notnull, new()
    where TSerializer : IOutSerializer<TFile>, new()
    where TViewModel : IViewModel
{
    /// <summary>
    /// Gets a reference to the serializer used to serialize and deserialize files of type <typeparamref name="TFile"/>.
    /// </summary>
    protected static readonly TSerializer Serializer = new();

    private readonly IEnumerable<FileFilterItem> _openFilter;

    /// <inheritdoc/>
    public abstract RecentFileInfo[] RecentFiles { get; set; }

    /// <inheritdoc/>
    public bool IsActive { get; set; }

    /// <inheritdoc/>
    public ICommand OpenFileCommand { get; }

    /// <inheritdoc/>
    public string PageName { get; }

    /// <inheritdoc/>
    public virtual IEnumerable<ButtonInteraction> AdditionalInteractions { get { yield break; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileViewModelLauncherBase{TFile, TSerializer, TViewModel}"/> class.
    /// </summary>
    /// <param name="pageName">Display name for the page.</param>
    /// <param name="openFilter">File filter to use when opening files.</param>
    protected FileViewModelLauncherBase(string pageName, IEnumerable<FileFilterItem> openFilter)
    {
        _openFilter = openFilter;
        PageName = pageName;
        OpenFileCommand = new SimpleCommand(p => DialogService?.RunOperation(q => OnOpen(p, q)) ?? Task.CompletedTask);
    }

    /// <inheritdoc/>
    public bool CanOpen(string fileExtension)
    {
        return _openFilter.Any(p => p.Extensions.Contains(fileExtension));
    }

    /// <inheritdoc/>
    public Task OnOpen(object? parameter)
    {
        return OnOpen(parameter, new Progress<ProgressReport>());
    }

    /// <summary>
    /// Creates a ViewModel for the specified file.
    /// </summary>
    /// <param name="friendlyName">
    /// Friendly name to use as a title for the ViewModel to be created.
    /// </param>
    /// <param name="file">
    /// Reference to the file to be displayed on the ViewModel.
    /// </param>
    /// <param name="filePath">
    /// Reference to the full FilePath to the file to be displayed on the
    /// ViewModel.
    /// </param>
    /// <returns>
    /// A new ViewModel that can be used to display the specified file.
    /// </returns>
    protected abstract TViewModel CreateViewModel(string? friendlyName, TFile file, string filePath);

    /// <summary>
    /// When overriden in a derived class, allows for custom
    /// <see cref="RecentFileInfo"/> generation.
    /// </summary>
    /// <param name="path">Path from where the file is being opened.</param>
    /// <param name="file">Parsed file contents.</param>
    /// <returns>
    /// A new <see cref="RecentFileInfo"/> that can be later used to open the
    /// same file quickly.
    /// </returns>
    protected virtual RecentFileInfo CreateRecentFileInfo(string path, TFile file)
    {
        return new()
        {
            FilePath = path,
            FriendlyName = Path.GetFileName(path),
        };
    }

    private Task<(string?, string?)> GetFilePath(object? parameter, ICollection<RecentFileInfo> recentFiles, IEnumerable<FileFilterItem> filters)
    {
        return parameter switch
        {
            RecentFileInfo file => TryGetFile(file, recentFiles),
            string file => Task.FromResult<(string?, string?)>(((string?)file, Path.GetFileName(file))),
            _ => TryOpenFile(filters)
        };
    }

    private async Task<(string?, string?)> TryGetFile(RecentFileInfo file, ICollection<RecentFileInfo> recentFiles)
    {
        recentFiles.Remove(file);
        IsBusy = true;
        try
        {
            if (!await Task.Run(() => File.Exists(file.FilePath)))
            {
                await (DialogService?.Error(St.FileNotFound, St.FileNotFound2) ?? Task.CompletedTask);
                return (null, null);
            }
            return (file.FilePath, file.FriendlyName);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<(string?, string?)> TryOpenFile(IEnumerable<FileFilterItem> filters)
    {
        if (await DialogService!.GetFileOpenPath(filters) is { Success:true, Result: { } f })
        {
            return (f, Path.GetFileName(f));
        }
        return (null, null);
    }

    private async Task OnOpen(object? parameter, IProgress<ProgressReport> progress)
    {
        if (await GetFilePath(parameter, [], _openFilter) is not { Item1: string filePath, Item2: string friendlyName }) return;
        progress.Report(string.Format("Opening {0}...", friendlyName));
        TFile file;
        try
        {
            file = await Task.Run(() => Serializer.Deserialize(File.OpenRead(filePath)));
        }
        catch (Exception ex)
        {
            await DialogService!.Error(string.Format("Error opening {0}: {1}", friendlyName, ex.Message));
            return;
        }        
        var recentFile = CreateRecentFileInfo(filePath, file);
        RecentFiles = Settings.Current.RecentFilesCount > 0 ? [recentFile, .. (RecentFiles?.Where(p => p.FilePath != filePath) ?? []).Take(Settings.Current.RecentFilesCount - 1)] : [];
        Notify(nameof(RecentFiles));
        await Settings.Save();
        await NavigationService!.Navigate(CreateViewModel(recentFile.FriendlyName, file, filePath));
    }
}
