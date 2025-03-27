using System.Collections.Generic;
using System.Linq;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Bnk;
using TheXDS.Vivianne.Serializers.Fce.Nfs3;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.ViewModels;
using TheXDS.Vivianne.ViewModels.Base;
using TheXDS.Vivianne.ViewModels.Bnk;
using TheXDS.Vivianne.ViewModels.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Fce.Nfs4;

namespace TheXDS.Vivianne.Data;

/// <summary>
/// Defines the signature for a method that can be used to create a ViewModel
/// for visualizing and optionally editing a file inside a VIV.
/// </summary>
/// <param name="data">Raw file data to be loaded into the viewer.</param>
/// <param name="vm">Reference to the current VIV editor.</param>
/// <param name="fileName">Filename for the file being opened.</param>
/// <returns>
/// A <see cref="IViewModel"/> that can be navigated to for previewing and
/// optionally edit the file.
/// </returns>
public delegate IViewModel? ContentVisualizerViewModelFactory(byte[] data, VivEditorViewModel vm, string fileName);

internal static class ContentVisualizerConfiguration
{
    public static IEnumerable<KeyValuePair<string, ContentVisualizerViewModelFactory>> Get()
    {
        yield return new(".tga", CreateTexturePreviewViewModel);
        yield return new(".jpg", CreateTexturePreviewViewModel);
        yield return new(".jpeg", CreateTexturePreviewViewModel);
        yield return new(".png", CreateTexturePreviewViewModel);
        yield return new(".bmp", CreateTexturePreviewViewModel);
        yield return new(".gif", CreateTexturePreviewViewModel);

        // FSH/QFS need to be properly decoded before displaying - do not use TexturePreviewViewModel.
        yield return new(".fsh", CreateFshEditorViewModel);
        yield return new(".qfs", CreateFshEditorViewModel);

        yield return new("fedata.bri", CreateFeDataEditorViewModel);
        yield return new("fedata.eng", CreateFeDataEditorViewModel);
        yield return new("fedata.fre", CreateFeDataEditorViewModel);
        yield return new("fedata.ger", CreateFeDataEditorViewModel);
        yield return new("fedata.ita", CreateFeDataEditorViewModel);
        yield return new("fedata.spa", CreateFeDataEditorViewModel);
        yield return new("fedata.swe", CreateFeDataEditorViewModel);

        yield return new(".txt", CreateCarpEditorViewModel);
        yield return new(".fce", CreateFceEditorViewModel);
        yield return new(".bnk", CreateBnkEditorViewModel);
    }

    public static IViewModel? CreateExternalEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return new ExternalFileViewModel(data, new VivBackingStore(vm), name);
    }

    private static IViewModel? CreateFeDataEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        if (data[0] == 4)
        {
            return CreateEditorViewModel<FeData4EditorViewModel, FeData4EditorState, Models.Fe.Nfs4.FeData, Serializers.Fe.Nfs4.FeDataSerializer>(data, vm, name);
        }
        else
        {
            return CreateEditorViewModel<FeData3EditorViewModel, FeData3EditorState, Models.Fe.Nfs3.FeData, Serializers.Fe.Nfs3.FeDataSerializer>(data, vm, name);
        }
    }

    private static IViewModel? CreateCarpEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        if (System.Text.Encoding.Latin1.GetString(data).Contains("understeer gradient(80)"))
        {
            return CreateEditorViewModel<ViewModels.Carp.Nfs4.CarpEditorViewModel, Models.Carp.Nfs4.CarpEditorState, Models.Carp.Nfs4.CarPerf, Serializers.Carp.Nfs4.CarpSerializer>(data, vm, name);
        }
        else
        {
            return CreateEditorViewModel<ViewModels.Carp.Nfs3.CarpEditorViewModel, Models.Carp.Nfs3.CarpEditorState, Models.Carp.Nfs3.CarPerf, Serializers.Carp.Nfs3.CarpSerializer>(data, vm, name);
        }
    }

    private static TexturePreviewViewModel CreateTexturePreviewViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return new(data) { Title = name };
    }

    private static readonly byte[][] knownFce4Headers =
    [
        [0x00, 0x10, 0x10, 0x14],
        [0x00, 0x10, 0x10, 0x15],
        [0x14, 0x10, 0x10, 0x00],
        [0x15, 0x10, 0x10, 0x00],
    ];

    private static IViewModel? CreateFceEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        if (knownFce4Headers.Any(data[0..4].SequenceEqual))
        {
            return new Fce4EditorViewModel(); //TODO: implement NFS4 FCE ViewModel
        }
        return CreateEditorViewModel<Fce3EditorViewModel, FceEditorState, FceFile, FceSerializer>(data, vm, name);
    }

    private static FshEditorViewModel? CreateFshEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return CreateEditorViewModel<FshEditorViewModel, FshEditorState, FshFile, FshSerializer>(data, vm, name);
    }

    private static BnkEditorViewModel? CreateBnkEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return CreateEditorViewModel<BnkEditorViewModel, BnkEditorState, BnkFile, BnkSerializer>(data, vm, name);
    }

    private static TViewModel? CreateEditorViewModel<TViewModel, TState, TFile, TSerializer>(byte[] data, VivEditorViewModel vm, string name)
        where TFile : notnull, new()
        where TViewModel : notnull, IFileEditorViewModel<TState, TFile>, new()
        where TState : notnull, IFileState<TFile>, new()
        where TSerializer : notnull, ISerializer<TFile>, new()
    {
        try
        {
            return new TViewModel()
            {
                Title = name,
                State = new TState() { File = new TSerializer().Deserialize(data) },
                BackingStore = new BackingStore<TFile, TSerializer>(new VivBackingStore(vm)) { FileName = name },
            };
        }
#if DEBUG
        catch (System.Exception ex)
        {
            vm.DialogService!.Error(ex);
#else
        catch
        {
            vm.DialogService!.Error($"Could not open {name}", "The file might be damaged or corrupt; or may use a format not currently understood by Vivianne.");
#endif
            return default;
        }
    }
}