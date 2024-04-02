﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.ViewModels;
using TheXDS.Vivianne.Extensions;
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
        { ".fsh", CreateFshEditorViewModel },
        { ".qfs", CreateQfsEditorViewModel },

        { "fedata.bri", CreateFeDataPreviewViewModel },
        { "fedata.eng", CreateFeDataPreviewViewModel },
        { "fedata.fre", CreateFeDataPreviewViewModel },
        { "fedata.ger", CreateFeDataPreviewViewModel },
        { "fedata.ita", CreateFeDataPreviewViewModel },
        { "fedata.spa", CreateFeDataPreviewViewModel },
        { "fedata.swe", CreateFeDataPreviewViewModel },

        { "carp.txt", CreateCarpEditorViewModel }
    };

    private static readonly Dictionary<string, Func<byte[]>> VivTemplates = new()
    {
        { "DASH.qfs", TemplateDashQfs}
    };

    private static byte[] TemplateDashQfs()
    {
        var cabinBlob = new FshBlob() { Magic = FshBlobFormat.Argb32, GaugeData = new() };
        cabinBlob.ReplaceWith(new Image<Rgba32>(640, 480));
        cabinBlob.Footer = Mappings.FshFooterWriter[FshBlobFooterType.CarDashboard].Invoke(cabinBlob);
        var steerBlob = new FshBlob() { Magic = FshBlobFormat.Argb32, XRotation=128, YRotation=128, XPosition=192, YPosition=352 };
        steerBlob.ReplaceWith(new Image<Rgba32>(256, 256));

        var fsh = new FshFile();
        fsh.Entries.Add("0000", cabinBlob);
        fsh.Entries.Add("0001", steerBlob);
        return QfsCodec.Compress(((ISerializer<FshFile>)new FshSerializer()).Serialize(fsh));
    }

    private static IViewModel CreateFeDataPreviewViewModel(byte[] data, Action<byte[]> saveCallback)
    {
        return new FeDataPreviewViewModel(data, saveCallback);
    }

    private static IViewModel CreateCarpEditorViewModel(byte[] data, Action<byte[]> saveCallback)
    {
        void SaveCarp(string c) => saveCallback.Invoke(System.Text.Encoding.Latin1.GetBytes(c));
        return new CarpEditorViewModel(CarpEditorState.From(System.Text.Encoding.Latin1.GetString(data)), SaveCarp);
    }

    private static IViewModel CreateTexturePreviewViewModel(byte[] data, Action<byte[]> _)
    {
        return new TexturePreviewViewModel(data);
    }

    private static IViewModel CreateFshEditorViewModel(byte[] data, Action<byte[]> saveCallback)
    {
        ISerializer<FshFile> serializer = new FshSerializer();
        void SaveFsh(FshFile fsh) => saveCallback.Invoke(serializer.Serialize(fsh));
        return new FshEditorViewModel(serializer.Deserialize(data), SaveFsh);
    }

    private static IViewModel CreateQfsEditorViewModel(byte[] data, Action<byte[]> saveCallback)
    {
        void CompressBack(byte[] data) => saveCallback.Invoke(QfsCodec.Compress(data));
        return CreateFshEditorViewModel(QfsCodec.Decompress(data), CompressBack);
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
        NewFromTemplateCommand = cb.BuildSimple(OnNewFromTemplate);
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
    /// Gets a reference to the command used to create a new file inside the
    /// VIV directory based on a template.
    /// </summary>
    public ICommand NewFromTemplateCommand { get; }

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
        var parser = new VivSerializer();

        await using var fs = File.Create(State.FilePath);
        await Task.Run(() => parser.SerializeTo(State.Viv, fs));
        return false;
    }

    private void OnOpenFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file, Value: { } rawData })
        {
            void Save(byte[] data)
            {
                UiThread.Invoke((Action)(() => State.Directory[file] = data));
                State.UnsavedChanges = true;
            }

            IViewModel vm = ContentVisualizers.FirstOrDefault(p => file.EndsWith(p.Key, StringComparison.InvariantCultureIgnoreCase)) is { Value: { } factory }
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

    private async Task OnNewFromTemplate()
    {
        var r = await DialogService!.SelectOption("New from template", "Select a template to create a new file in the VIV directory.", VivTemplates.Keys.ToArray());
        if (r.Success)
        {
            var template = VivTemplates.ToList()[r.Result];
            State.Directory[template.Key] = template.Value.Invoke();
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
