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
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Base class for all ViewModels that can be used to launch file editors from
/// the startup page.
/// </summary>
/// <typeparam name="TState"></typeparam>
/// <typeparam name="TFile"></typeparam>
/// <typeparam name="TSerializer"></typeparam>
/// <typeparam name="TEditor"></typeparam>
public abstract class FileEditorViewModelLauncher<TState, TFile, TSerializer, TEditor> : ViewModel, IFileEditorViewModelLauncher
    where TFile : new()
    where TState : IFileState<TFile>, new()
    where TSerializer : ISerializer<TFile>, new()
    where TEditor : IFileEditorViewModel<TState, TFile>, new()
{
    private static readonly TSerializer serializer = new();
    private readonly IEnumerable<FileFilterItem> openFilter;
    private readonly IEnumerable<FileFilterItem> saveFilter;

    /// <inheritdoc/>
    public string PageName { get; }

    /// <inheritdoc/>
    public ICommand NewFileCommand { get; }

    /// <inheritdoc/>
    public ICommand OpenFileCommand { get; }

    /// <inheritdoc/>
    public abstract RecentFileInfo[] RecentFiles { get; set; }

    /// <inheritdoc/>
    public virtual IEnumerable<ButtonInteraction> AdditionalInteractions => [];

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="FileEditorViewModelLauncher{TState, TFile, TSerializer, TEditor}"/>
    /// class.
    /// </summary>
    /// <param name="pageName">Display name for the page.</param>
    /// <param name="openFilter">File filter to use when opening files.</param>
    /// <param name="saveFilter">File filter to use when saving files.</param>
    protected FileEditorViewModelLauncher(string pageName, IEnumerable<FileFilterItem> openFilter, IEnumerable<FileFilterItem> saveFilter)
    {
        this.openFilter = openFilter;
        this.saveFilter = saveFilter;
        PageName = pageName;
        NewFileCommand = new SimpleCommand(OnNew);
        OpenFileCommand = new SimpleCommand(p => DialogService.RunOperation(q => OnOpen(p)));
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="FileEditorViewModelLauncher{TState, TFile, TSerializer, TEditor}"/>
    /// class.
    /// </summary>
    /// <param name="pageName">Display name for the page.</param>
    /// <param name="filter">
    /// File filter to use when opening or saving files.
    /// </param>
    protected FileEditorViewModelLauncher(string pageName, IEnumerable<FileFilterItem> filter) : this(pageName, filter, filter)
    {
    }

    /// <inheritdoc/>
    public bool CanOpen(string fileExtension)
    {
        return openFilter.Any(p => p.Extensions.Contains(fileExtension));
    }

    /// <inheritdoc/>
    public async Task OnOpen(object? parameter)
    {
        if (await GetFilePath(parameter, [], openFilter) is not string filePath) return;
        var file = await serializer.DeserializeAsync(File.OpenRead(filePath));
        var state = new TState { File = file, FilePath = filePath };
        var recentFile = CreateRecentFileInfo(filePath, file);
        RecentFiles = [recentFile, .. RecentFiles.Take(9)];
        Notify(nameof(RecentFiles));
        await Settings.Save();
        var vm = new TEditor()
        {
            Title = recentFile.FriendlyName,
            State = state,
        };
        WireUpFullSaveCommands(vm, file, filePath);
        NavigationService!.Navigate(vm);
    }

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

    /// <summary>
    /// When overriden in a derived class, allows for final adjustments to be
    /// performed on the file to be saved right before commiting the changes to
    /// disk.
    /// </summary>
    /// <param name="file">File to be saved.</param>
    /// <param name="filePath">Path in which the file will be saved.</param>
    protected virtual void BeforeSave(TFile file, string filePath)
    {
        if (Settings.Current.AutoBackup)
        {
            FileBackup.Create(filePath);
        }
    }

    private void WireUpFullSaveCommands(TEditor vm, TFile file, string filePath)
    {
        vm.SaveCommand = ObservingCommandBuilder.Create(vm.State, () => OnSave(file, filePath)).ListensToCanExecute(s => s.UnsavedChanges).Build();
        vm.SaveAsCommand = GetSaveAsCommand(vm, file, filePath);
    }

    private ObservingCommand GetSaveAsCommand(TEditor vm, TFile file, string? filePath = null)
    {
        return ObservingCommandBuilder.Create(vm.State, () => OnSaveAs(vm, file, saveFilter, filePath)).ListensToCanExecute(s => s.UnsavedChanges).Build();
    }

    private void OnNew(object? parameter)
    {
        var file = new TFile();
        var state = new TState { File = file };
        var vm = new TEditor()
        {
            Title = "New file",
            State = state,
        };
        vm.SaveCommand = GetSaveAsCommand(vm, file);
        NavigationService!.Navigate(vm);
    }

    private static byte[] GetSerializedFile(TFile file)
    {
        return serializer.Serialize(file);
    }

    private async Task OnSaveAs(TEditor vm, TFile file, IEnumerable<FileFilterItem> saveFilter, string? fileName)
    {
        if (await DialogService!.GetFileSavePath(saveFilter, fileName) is { Success: true, Result: { } filePath })
        {
            await OnSave(file, filePath);
            if (vm.SaveAsCommand is null)
            {
                WireUpFullSaveCommands(vm, file, filePath);
            }
        }
    }

    private Task OnSave(TFile file, string filePath)
    {
        BeforeSave(file, filePath);
        return File.WriteAllBytesAsync(filePath, GetSerializedFile(file));
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
        if (!System.IO.File.Exists(file.FilePath))
        {
            await (DialogService?.Error("File not found.", "The file you selected does not exist or it's inaccessible.") ?? Task.CompletedTask);
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
