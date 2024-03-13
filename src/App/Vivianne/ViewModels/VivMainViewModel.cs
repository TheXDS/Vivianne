using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.ViewModels;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Defines the signature for a method that can be used to create a ViewModel
/// for visualizing and optionally editing a file inside a VIV.
/// </summary>
/// <param name="data">Raw file data to be loaded into the viewer.</param>
/// <param name="saveCalback">
/// Callback to invoke when the ViewModel needs to request saving the file.
/// </param>
/// <returns>
/// A <see cref="IViewModel"/> that can be navigated to for previewing and
/// optionally edit the file.
/// </returns>
public delegate IViewModel ContentVisualizerViewModelFactory(byte[] data, Action<byte[]> saveCalback);

/// <summary>
/// ViewModel that serves as the main view for interacting with a VIV file.
/// </summary>
public class VivMainViewModel : HostViewModelBase, IStatefulViewModel<VivMainState>
{
    private static readonly Dictionary<string, ContentVisualizerViewModelFactory> ContentVisualizers = new()
    {
        { ".tga", CreateTexturePreviewViewModel },
        { ".jpg", CreateTexturePreviewViewModel },
        { ".jpeg", CreateTexturePreviewViewModel },
        { ".png", CreateTexturePreviewViewModel },
        { ".bmp", CreateTexturePreviewViewModel },
        { ".gif", CreateTexturePreviewViewModel },

        // FSH/QFS need to be properly decoded before displaying - do not use TexturePreviewViewModel.
        { ".fsh", CreateFshPreviewViewModel },
        { ".qfs", CreateQfsPreviewViewModel },

        { ".bri", CreateFeDataPreviewViewModel },
        { ".eng", CreateFeDataPreviewViewModel },
        { ".fre", CreateFeDataPreviewViewModel },
        { ".ger", CreateFeDataPreviewViewModel },
        { ".ita", CreateFeDataPreviewViewModel },
        { ".spa", CreateFeDataPreviewViewModel },
        { ".swe", CreateFeDataPreviewViewModel },
    };

    private static IViewModel CreateFeDataPreviewViewModel(byte[] data, Action<byte[]> saveCallback)
    {
        return new FeDataPreviewViewModel(data, saveCallback);
    }

    private static IViewModel CreateTexturePreviewViewModel(byte[] data, Action<byte[]> _)
    {
        return new TexturePreviewViewModel(data);
    }

    private static IViewModel CreateFshPreviewViewModel(byte[] data, Action<byte[]> _)
    {
        using var ms = new MemoryStream(data);
        return new FshPreviewViewModel(new FshSerializer().Deserialize(ms)) { Title = ""};
    }

    private static IViewModel CreateQfsPreviewViewModel(byte[] data, Action<byte[]> _)
    {
        var uncompressed = QfsCodec.Decompress(data);
        using var ms = new MemoryStream(uncompressed);
        return new FshPreviewViewModel(new FshSerializer().Deserialize(ms));
    }

    private VivMainState state = null!;

    /// <inheritdoc/>
    public VivMainState State
    {
        get => state;
        set => Change(ref state, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VivMainViewModel"/> class.
    /// </summary>
    public VivMainViewModel()
    {
        var cb = CommandBuilder.For(this);
        OpenFileCommand = cb.BuildSimple(OnOpenFile);
        ImportFileCommand = cb.BuildSimple(OnImportFile);
        ReplaceFileCommand = cb.BuildSimple(OnReplaceFile);
        ExportFileCommand = cb.BuildSimple(OnExportFile);
        RemoveFileCommand = cb.BuildSimple(OnRemoveFile);
    }

    /// <summary>
    /// Gets a reference to the command used to visualize the selected file
    /// from the VIV.
    /// </summary>
    public ICommand OpenFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to import a new file to the VIV
    /// directory.
    /// </summary>
    public ICommand ImportFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to export a selected file from the
    /// VIV directory.
    /// </summary>
    public ICommand ExportFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to replace a file on the VIV
    /// directory for a new one.
    /// </summary>
    public ICommand ReplaceFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to remove a file from the VIV
    /// directory.
    /// </summary>
    public ICommand RemoveFileCommand { get; }

    private async Task<bool> SaveVivAsync(IProgress<ProgressReport> progress)
    {
        if (State.FilePath is null)
        {
            var r = await DialogService!.GetFileSavePath("Save VIV", "", FileFilters.VivFileFilter);
            if (r.Success)
            {
                State.FilePath = r.Result;
            }
            else
            {
                return true;
            }
        }
        progress.Report("Saving...");
        await using var fs = File.Create(State.FilePath);
        await Task.Run(() => State.Viv.WriteTo(fs));
        return false;
    }

    private void OnOpenFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file, Value: { } rawData })
        {
            void Save(byte[] data)
            {
                State.Directory[file] = data;
                State.UnsavedChanges = true;
            }

            var ext = Path.GetExtension(file);
            IViewModel vm = ContentVisualizers.TryGetValue(ext, out var factory)
                ? factory.Invoke(rawData, Save)
                : new ExternalFileViewModel(rawData, Save);
            vm.Title = file;
            ChildNavService!.Navigate(vm);
        }
        else
        {
            ChildNavService!.NavigateAndReset<VivInfoViewModel, VivMainState>(State);
        }
    }

    private async Task OnImportFile()
    {
        var r = await DialogService!.GetFileOpenPath("Import file", "Select a file to import", FileFilters.AnyVivContentFilter);
        if (r.Success)
        {
            var keyName = Path.GetFileName(r.Result).ToLower();
            if (State.Directory.ContainsKey(keyName) && !await DialogService.Ask("Replace file", $"The file '{keyName}' already exist. Do you want to replace it?"))
            {
                return;
            }
            State.Directory[keyName] = await DialogService.RunOperation(p => File.ReadAllBytesAsync(r.Result));
        }
    }

    private async Task OnExportFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file, Value: { } rawData })
        {
            var ext = Path.GetExtension(file)[1..];
            var r = await DialogService!.GetFileSavePath(null, "", [FileFilterItem.Simple(ext), FileFilterItem.AllFiles], file);
            if (r.Success)
            {
                File.WriteAllBytes(r.Result, rawData);
            }
        }
    }

    private async Task OnReplaceFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file, Value: { } rawData })
        {
            var ext = Path.GetExtension(file)[1..];

            var r = await DialogService!.GetFileOpenPath($"Replace '{file}'", $"Select a file to repace '{file}' with", [FileFilterItem.Simple(ext), FileFilterItem.AllFiles]);
            if (r.Success)
            {
                State.Directory[Path.GetFileName(r.Result).ToLower()] = await DialogService.RunOperation(p => File.ReadAllBytesAsync(r.Result));
            }
        }
    }

    private async Task OnRemoveFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file })
        {
            if (await DialogService!.Ask($"Remove '{file}'", $"Are you sure you want to remove '{file}'?"))
            {
                State.Directory.Remove(file);
            }
        }
    }

    async Task IViewModel.OnNavigateBack(CancelFlag navigation)
    {
        if (State.UnsavedChanges)
        {
            bool ask = false;
            do
            {
                switch (await DialogService!.AskYnc("Unsaved changes", $"Do you want to save {State.FriendlyName}?"))
                {
                    case true: ask = await DialogService.RunOperation(SaveVivAsync); break;
                    case null: navigation.Cancel(); break;
                }
            } while (ask);
        }
    }

    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        ChildNavService!.NavigateAndReset<VivInfoViewModel, VivMainState>(State);
        return base.OnCreated();
    }
}
