using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.ViewModels;

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
public delegate IViewModel? ContentVisualizerViewModelFactory(byte[] data, Action<byte[]> saveCalback, VivEditorState file, string fileName);

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
        yield return new(".qfs", CreateQfsEditorViewModel);

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

    private static FcePreviewViewModel CreateFceEditorViewModel(byte[] data, Action<byte[]> _, VivEditorState viv, string __)
    {
        ISerializer<FceFile> s = new FceSerializer();
        return new(s.Deserialize(data), viv.Directory);
    }

    private static FeDataPreviewViewModel CreateFeDataEditorViewModel(byte[] data, Action<byte[]> saveCallback, VivEditorState viv, string name)
    {
        return new(data, saveCallback, viv, name) { Title = name };
    }

    private static CarpEditorViewModel? CreateCarpEditorViewModel(byte[] data, Action<byte[]> saveCallback, VivEditorState viv, string name)
    {
        byte[] carpMagic = [0x53, 0x65, 0x72, 0x69, 0x61, 0x6c, 0x20, 0x4e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x28, 0x30, 0x29];
        if (!data[0..16].SequenceEqual(carpMagic)) return null;
        void SaveCarp(string c) => saveCallback.Invoke(System.Text.Encoding.Latin1.GetBytes(c));
        return new(CarpEditorState.From(System.Text.Encoding.Latin1.GetString(data)), SaveCarp, viv) { Title = name };
    }

    private static TexturePreviewViewModel CreateTexturePreviewViewModel(byte[] data, Action<byte[]> _, VivEditorState __, string name)
    {
        return new(data) { Title = name };
    }

    private static FshEditorViewModel CreateFshEditorViewModel(byte[] data, Action<byte[]> saveCallback, VivEditorState _, string name)
    {
        ISerializer<FshFile> serializer = new FshSerializer();
        var file = serializer.Deserialize(data);
        var state = new FshEditorState() { File = file };
        void SaveFsh(FshFile fsh) => saveCallback.Invoke(serializer.Serialize(fsh));
        return new()
        {
            Title = name,
            State = state,
            SaveCommand = state.Create(() => SaveFsh(file)).ListensToCanExecute(s => s.UnsavedChanges).Build(),
        };
    }

    private static FshEditorViewModel CreateQfsEditorViewModel(byte[] data, Action<byte[]> saveCallback, VivEditorState viv, string name)
    {
        void CompressBack(byte[] data) => saveCallback.Invoke(QfsCodec.Compress(data));
        return CreateFshEditorViewModel(QfsCodec.Decompress(data), CompressBack, viv, name);
    }
}