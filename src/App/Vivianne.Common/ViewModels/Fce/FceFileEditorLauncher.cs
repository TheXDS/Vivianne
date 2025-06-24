using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.ViewModels.Base;
using M3 = TheXDS.Vivianne.Models.Fce.Nfs3;
using M4 = TheXDS.Vivianne.Models.Fce.Nfs4;
using S3 = TheXDS.Vivianne.Serializers.Fce.Nfs3;
using S4 = TheXDS.Vivianne.Serializers.Fce.Nfs4;
using St = TheXDS.Vivianne.Resources.Strings.Common;
using Vm3 = TheXDS.Vivianne.ViewModels.Fce.Nfs3;
using Vm4 = TheXDS.Vivianne.ViewModels.Fce.Nfs4;

namespace TheXDS.Vivianne.ViewModels.Fce;

/// <summary>
/// Implements a launcher to create and/or edit FCE files for NFS3.
/// </summary>
public class FceFileEditorLauncher : ViewModel, IFileEditorViewModelLauncher
{
    private readonly Func<IDialogService> _dialogSvc;

    /// <inheritdoc/>
    public RecentFileInfo[] RecentFiles
    {
        get => Settings.Current.RecentFceFiles;
        set => Settings.Current.RecentFceFiles = value;
    }

    /// <inheritdoc/>
    public string PageName => "FCE";

    /// <inheritdoc/>
    public bool CanCreateNew => false;

    /// <inheritdoc/>
    public IEnumerable<ButtonInteraction> AdditionalInteractions => [];

    /// <inheritdoc/>
    public ICommand NewFileCommand { get; }

    /// <inheritdoc/>
    public ICommand OpenFileCommand { get; }

    /// <inheritdoc/>
    public bool IsActive { get; set; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="FceFileEditorLauncher"/>
    /// class.
    /// </summary>
    /// <param name="dialogSvc">Dialog service to consume for dialogs.</param>
    public FceFileEditorLauncher(Func<IDialogService> dialogSvc)
    {
        _dialogSvc = dialogSvc;
        NewFileCommand = new SimpleCommand(() => throw new TamperException(), false);
        OpenFileCommand = new SimpleCommand(p => DialogService?.RunOperation(q => OnOpen(p)) ?? Task.CompletedTask);
    }

    /// <inheritdoc/>
    public bool CanOpen(string fileExtension) => FileFilters.FceFileFilter.Any(p => p.Extensions.Contains(fileExtension));

    /// <inheritdoc/>
    public async Task OnOpen(object? parameter)
    {
        if (await GetFilePath(parameter, [], FileFilters.FceFileFilter) is not string filePath) return;

        var fileContents = await File.ReadAllBytesAsync(filePath);
        await (VersionIdentifier.FceVersion(fileContents) switch
        {
            NfsVersion.Nfs3 => OnOpen<M3.FceFile, FcePart, S3.FceSerializer, Vm3.Fce3EditorViewModel, Vm3.Fce3EditorState>(filePath, fileContents),
            NfsVersion.Nfs4 or NfsVersion.Mco => OnOpen<M4.FceFile, M4.Fce4Part, S4.FceSerializer, Vm4.Fce4EditorViewModel, Vm4.Fce4EditorState>(filePath, fileContents),
            _ => DialogService?.Error("Unsupported file format.") ?? Task.CompletedTask
        });
    }

    private async Task OnOpen<TFile, TPart, TSerializer, TEditor, TState>(string filePath, byte[] fileContents)
        where TPart : FcePart
        where TFile : IFceFile<TPart>, new()
        where TSerializer : ISerializer<TFile>, new()
        where TState : NotifyPropertyChanged, IFileState<TFile>, new()
        where TEditor : StatefulFileEditorViewModelBase<TState, TFile>, new()
    {
        var file = await new TSerializer().DeserializeAsync(fileContents);
        var recentFile = CreateRecentFileInfo(filePath);
        RecentFiles = Settings.Current.RecentFilesCount > 0 ? [recentFile, .. (RecentFiles?.Where(p => p.FilePath != filePath) ?? []).Take(Settings.Current.RecentFilesCount - 1)] : [];
        Notify(nameof(RecentFiles));
        await Settings.Save();
        var vm = new TEditor()
        {
            Title = recentFile.FriendlyName,
            State = new TState { File = file },
            BackingStore = new BackingStore<TFile, TSerializer>(new FileSystemBackingStore(_dialogSvc.Invoke(), FileFilters.FceFileFilter, Path.GetDirectoryName(filePath) ?? Environment.CurrentDirectory)) { FileName = filePath },
        };
        await NavigationService!.Navigate(vm);
    }

    private static RecentFileInfo CreateRecentFileInfo(string path)
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
        if (!File.Exists(file.FilePath))
        {
            await (DialogService?.Error(St.FileNotFound, St.FileNotFound2) ?? Task.CompletedTask);
            return null;
        }
        return file.FilePath;
    }

    private async Task<string?> TryOpenFile(IEnumerable<FileFilterItem> filters)
    {
        var f = await DialogService!.GetFileOpenPath(filters);
        return f.Result;
    }
}