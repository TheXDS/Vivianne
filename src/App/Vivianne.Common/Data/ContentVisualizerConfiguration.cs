﻿using System;
using System.Collections.Generic;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Carp.Nfs3;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Carp.Nfs3;
using TheXDS.Vivianne.Serializers.Fce.Nfs3;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.Tools;
using TheXDS.Vivianne.ViewModels;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.Data;

/// <summary>
/// Defines the signature for a method that can be used to create a ViewModel
/// for visualizing and optionally editing a file inside a VIV.
/// </summary>
/// <param name="data">Raw file data to be loaded into the viewer.</param>
/// <param name="saveCalback">
/// Callback to invoke when the ViewModel needs to request saving the file.
/// </param>
/// <param name="file">Reference to the current VIV file.</param>
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
    }

    private static IViewModel CreateFeDataEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
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

    private static Carp3EditorViewModel? CreateCarpEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return CreateEditorViewModel<Carp3EditorViewModel, CarpEditorState, CarPerf, CarpSerializer>(data, vm, name);

        /*
        byte[] carpMagic = [0x53, 0x65, 0x72, 0x69, 0x61, 0x6c, 0x20, 0x4e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x28, 0x30, 0x29];
        if (!data[0..16].SequenceEqual(carpMagic)) return null;
        void SaveCarp(string c) => vm.Invoke(System.Text.Encoding.Latin1.GetBytes(c));
        return new(CarpEditorState.From(System.Text.Encoding.Latin1.GetString(data)), SaveCarp, vm.State) { Title = name };*/
    }

    private static TexturePreviewViewModel CreateTexturePreviewViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return new(data) { Title = name };
    }

    private static Fce3EditorViewModel CreateFceEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return CreateEditorViewModel<Fce3EditorViewModel, Fce3EditorState, FceFile, FceSerializer>(data, vm, name);
    }

    private static FshEditorViewModel CreateFshEditorViewModel(byte[] data, VivEditorViewModel vm, string name)
    {
        return CreateEditorViewModel<FshEditorViewModel, FshEditorState, FshFile, FshSerializer>(data, vm, name);
    }

    private static TViewModel CreateEditorViewModel<TViewModel, TState, TFile, TSerializer>(byte[] data, VivEditorViewModel vm, string name)
        where TFile : new()
        where TViewModel : IFileEditorViewModel<TState, TFile>, new()
        where TState : IFileState<TFile>, new()
        where TSerializer : ISerializer<TFile>, new()
    {
        TSerializer serializer = new();
        return new TViewModel()
        {
            Title = name,
            State = new TState() { File = serializer.Deserialize(data) },
            BackingStore = new BackingStore<TFile, TSerializer>(new VivStateBackingStore(vm)),
        };
    }
}