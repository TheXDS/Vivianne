using System.Collections.Generic;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio.Bnk;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.ViewModels;
using TheXDS.Vivianne.ViewModels.Asf;
using TheXDS.Vivianne.ViewModels.Base;
using TheXDS.Vivianne.ViewModels.Bnk;
using TheXDS.Vivianne.ViewModels.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Fce.Nfs4;
using TheXDS.Vivianne.ViewModels.Fe;
using TheXDS.Vivianne.ViewModels.Fsh;
using TheXDS.Vivianne.ViewModels.Viv;
using MFce3 = TheXDS.Vivianne.Models.Fce.Nfs3;
using MFce4 = TheXDS.Vivianne.Models.Fce.Nfs4;
using SNfs3 = TheXDS.Vivianne.Serializers.Fce.Nfs3.FceSerializer;
using SNfs4 = TheXDS.Vivianne.Serializers.Fce.Nfs4.FceSerializer;
using VmFce3 = TheXDS.Vivianne.ViewModels.Fce.Nfs3.Fce3EditorViewModel;
using VmFce4 = TheXDS.Vivianne.ViewModels.Fce.Nfs4.Fce4EditorViewModel;

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

        yield return new(".fsh", CreateFshEditorViewModel);
        yield return new(".qfs", CreateFshEditorViewModel);

        yield return new(".bri", CreateFeDataEditorViewModel);
        yield return new(".eng", CreateFeDataEditorViewModel);
        yield return new(".fre", CreateFeDataEditorViewModel);
        yield return new(".ger", CreateFeDataEditorViewModel);
        yield return new(".ita", CreateFeDataEditorViewModel);
        yield return new(".spa", CreateFeDataEditorViewModel);
        yield return new(".swe", CreateFeDataEditorViewModel);

        yield return new(".dat", CreateCarpEditorViewModel);
        yield return new(".qda", CreateCarpEditorViewModel);
        yield return new(".txt", CreateCarpEditorViewModel);
        yield return new(".fce", CreateFceEditorViewModel);
        yield return new(".bnk", CreateBnkEditorViewModel);

        yield return new(".asf", CreateMusPlayerViewModel);
        yield return new(".mus", CreateMusPlayerViewModel);

        yield return new(".md", CreateRawReadOnlyViewModel);
        yield return new(".nfo", CreateRawReadOnlyViewModel);
    }

    public static RawContentViewModel CreateRawReadOnlyViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return new RawContentViewModel(data);
    }

    public static ExternalFileViewModel CreateExternalEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return new ExternalFileViewModel(data, new VivBackingStore(vm), name);
    }

    private static IViewModel? CreateFeDataEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return VersionIdentifier.FeDataVersion(data) switch
        {
            NfsVersion.Nfs3 => CreateEditorViewModel<FeData3EditorViewModel, FeData3EditorState, Models.Fe.Nfs3.FeData, Serializers.Fe.Nfs3.FeDataSerializer>(data, vm, name),
            NfsVersion.Nfs4 => CreateEditorViewModel<FeData4EditorViewModel, FeData4EditorState, Models.Fe.Nfs4.FeData, Serializers.Fe.Nfs4.FeDataSerializer>(data, vm, name),
            _ => null
        };
    }

    private static IViewModel? CreateCarpEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return VersionIdentifier.CarpVersion(data) switch
        {
            NfsVersion.Nfs2 => CreateEditorViewModel<ViewModels.Carp.Nfs2.CarpEditorViewModel, Models.Carp.Nfs2.CarpEditorState, Models.Carp.Nfs2.CarPerf, Serializers.Carp.Nfs2.CarpSerializer>(data, vm, name),
            NfsVersion.Nfs3 => CreateEditorViewModel<ViewModels.Carp.Nfs3.CarpEditorViewModel, Models.Carp.Nfs3.CarpEditorState, Models.Carp.Nfs3.CarPerf, Serializers.Carp.Nfs3.CarpSerializer>(data, vm, name),
            NfsVersion.Nfs4 => CreateEditorViewModel<ViewModels.Carp.Nfs4.CarpEditorViewModel, Models.Carp.Nfs4.CarpEditorState, Models.Carp.Nfs4.CarPerf, Serializers.Carp.Nfs4.CarpSerializer>(data, vm, name),
            _ => null
        };
    }

    private static TexturePreviewViewModel? CreateTexturePreviewViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return CreateEditorViewModel<TexturePreviewViewModel, RawFileEditorState, RawFile, RawFileSerializer>(data, vm, name);
    }

    private static IViewModel? CreateFceEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return VersionIdentifier.FceVersion(data) switch
        {
            NfsVersion.Nfs3 => CreateEditorViewModel<VmFce3, Fce3EditorState, MFce3.FceFile, SNfs3>(data, vm, name),
            NfsVersion.Nfs4 or NfsVersion.Mco=> CreateEditorViewModel<VmFce4, Fce4EditorState, MFce4.FceFile, SNfs4>(data, vm, name),            
            _ => null
        };
    }

    private static FshEditorViewModel? CreateFshEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return CreateEditorViewModel<FshEditorViewModel, FshEditorState, FshFile, FshSerializer>(data, vm, name);
    }

    private static BnkEditorViewModel? CreateBnkEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return CreateEditorViewModel<BnkEditorViewModel, BnkEditorState, BnkFile, BnkSerializer>(data, vm, name);
    }
    private static MusPlayerViewModel? CreateMusPlayerViewModel(byte[] data, VivEditorViewModel vm, string fileName)
    {
        return new MusPlayerViewModel()
        {
            Title = fileName,
            Mus = ((IOutSerializer<MusFile>)new MusSerializer()).Deserialize(data),
            FileName = fileName,
            BackingStore = new VivBackingStore(vm)
        };
    }

    private static TViewModel? CreateEditorViewModel<TViewModel, TState, TFile, TSerializer>(byte[] data, VivEditorViewModel vm, string name)
        where TFile : notnull
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