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

public abstract class FileViewModelLauncherBase<TFile, TSerializer, TViewModel> : ViewModel, IFileViewerViewModelLauncher
    where TFile : notnull, new()
    where TSerializer : IOutSerializer<TFile>, new()
    where TViewModel : IFileEditorViewModel, new()

{
    /// <summary>
    /// Gets a reference to the serializer used to serialize and deserialize files of type <typeparamref name="TFile"/>.
    /// </summary>
    protected static readonly TSerializer Serializer = new();

    private readonly IEnumerable<FileFilterItem> _openFilter;

    /// <inheritdoc/>
    public abstract RecentFileInfo[] RecentFiles { get; set; }

    /// <inheritdoc/>
    public ICommand OpenFileCommand { get; }

    /// <inheritdoc/>
    public string PageName { get; }

    /// <inheritdoc/>
    public virtual IEnumerable<ButtonInteraction> AdditionalInteractions { get { yield break; } }

    protected FileViewModelLauncherBase(string pageName, IEnumerable<FileFilterItem> openFilter)
    {
        _openFilter = openFilter;
        PageName = pageName;
        OpenFileCommand = new SimpleCommand(p => DialogService?.RunOperation(q => OnOpen(p)) ?? Task.CompletedTask);

    }

    /// <inheritdoc/>
    public bool CanOpen(string fileExtension)
    {
        return _openFilter.Any(p => p.Extensions.Contains(fileExtension));
    }

    /// <inheritdoc/>
    public async Task OnOpen(object? parameter)
    {
        if (await GetFilePath(parameter, [], _openFilter) is not string filePath) return;
        var file = await Task.Run(() => Serializer.Deserialize(File.OpenRead(filePath)));
        var recentFile = CreateRecentFileInfo(filePath, file);
        RecentFiles = Settings.Current.RecentFilesCount > 0 ? [recentFile, .. (RecentFiles?.Where(p => p.FilePath != filePath) ?? []).Take(Settings.Current.RecentFilesCount - 1)] : [];
        Notify(nameof(RecentFiles));
        await Settings.Save();
        await NavigationService!.Navigate(CreateViewModel(recentFile.FriendlyName));
    }

    protected abstract TViewModel CreateViewModel(string? friendlyName);

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

    private Task<string?> GetFilePath(object? parameter, ICollection<RecentFileInfo> recentFiles, IEnumerable<FileFilterItem> filters)
    {
        return parameter switch
        {
            RecentFileInfo file => TryGetFile(file, recentFiles),
            string file => Task.FromResult((string?)file),
            _ => TryOpenFile(filters)
        };
    }

    private async Task<string?> TryGetFile(RecentFileInfo file, ICollection<RecentFileInfo> recentFiles)
    {
        recentFiles.Remove(file);
        IsBusy = true;
        try
        {
            if (!await Task.Run(() => File.Exists(file.FilePath)))
            {
                await (DialogService?.Error(St.FileNotFound, St.FileNotFound2) ?? Task.CompletedTask);
                return null;
            }
            return file.FilePath;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<string?> TryOpenFile(IEnumerable<FileFilterItem> filters)
    {
        var f = await DialogService!.GetFileOpenPath(filters);
        return f.Result;
    }
}
