using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Component.Application;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.ViewModels;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.FileFilters;

namespace TheXDS.Vivianne.Data;

/// <summary>
/// Defines the signature for a method that can be used to create a ViewModel
/// for visualizing and optionally editing a file inside a VIV.
/// </summary>
/// <param name="backingStoreFactory">
/// Callback that generates a backing store for any editor ViewModels that require it.
/// </param>
/// <param name="fileName">Filename for the file being opened.</param>
/// <returns>
/// A <see cref="IViewModel"/> that can be navigated to for previewing and
/// optionally edit the file.
/// </returns>
public delegate Task<IViewModel?> ContentVisualizerViewModelFactory(Func<IBackingStore> backingStoreFactory, string fileName);

/// <summary>
/// Represents a file type configration registry of all file types supported by Vivianne.
/// </summary>
public static partial class FileTypes
{
    /// <summary>
    /// Contains a collection of all file types recognized by Vivianne.
    /// </summary>
    public static readonly FileTypeInfo[] KnownFileTypes =
    [
        new(".viv", CreateVivEditorViewModel, St.VivFile),
        new(".fce", CreateFceEditorViewModel, St.EA3DModel),
        new(".geo", CreateGeoEditorViewModel, St.EA3DModel),
        new(".bnk", CreateBnkEditorViewModel, St.BnkFile),
        new(".asf", CreateMusPlayerViewModel, St.AsfFile, false),

        new([".jpeg", ".png", ".jpg", ".bmp", ".gif", ".tga"],
            CreateTexturePreviewViewModel, "TheXDS.Vivianne.pict", St.PictureFile, false)
        {
            SaveFilters = [
                new FileFilterItem(St.PngFile, "*.png"),
                new FileFilterItem(St.GifFile, "*.gif"),
                new FileFilterItem(St.JpgFile, ["*.jpg", "*.jpeg"]),
                new FileFilterItem(St.BmpFile, "*.bmp"),
                new FileFilterItem(St.TgaFile, "*.tga"),
                FileFilterItem.AllFiles
            ]
        },

        new([".fsh", ".qfs"],
            CreateFshEditorViewModel, "TheXDS.Vivianne.shpi", St.FshFile)
        {
            SaveFilters = [
                new FileFilterItem(St.FshFile, "*.fsh"),
                new FileFilterItem(St.QfsFile, "*.qfs"),
                FileFilterItem.AllFiles
            ]
        },

        new([".eng", ".bri", ".fre", ".ger", ".ita", ".spa", ".swe"],
            CreateFeDataEditorViewModel, "TheXDS.Vivianne.fe", St.FeDataFile),

        new([".txt", ".dat"],
            CreateCarpEditorViewModel, "TheXDS.Vivianne.carp", St.CarpFile, false),

        new([".mus", ".lin", ".map"],
            CreateMusPlayerViewModel, "TheXDS.Vivianne.mus", St.MusFile)
        {
            SaveFilters = [
                new FileFilterItem(St.MusFile, "*.mus"),
                new FileFilterItem(St.LinFile, "*.lin"),
                new FileFilterItem(St.MapFile, "*.map"),
                FileFilterItem.AllFiles
            ]
        },

        new([".md", ".nfo", ".1st"],
            CreateRawReadOnlyViewModel, "TheXDS.Vivianne.md", St.MdFile, false),

        new([".qda"],
            CreateComingSoonViewModel, "TheXDS.Vivianne.generic", St.GenericFile),
    ];

    /// <summary>
    /// Gets the file format info for the specified known file format.
    /// </summary>
    /// <param name="type">Type for which to get the file format info.</param>
    /// <returns>
    /// A <see cref="FileTypeInfo"/> with the information of the specified file
    /// format.
    /// </returns>
    public static FileTypeInfo GetInfo(KnownFileType type)
    {
        return KnownFileTypes[(int)type];
    }

    private static NamedObject<ContentVisualizerViewModelFactory>[] SelectableVisualizers()
    {
        const int VisibleExtensionsCount = 2;
        return [.. KnownFileTypes[..^1].Select(p => new NamedObject<ContentVisualizerViewModelFactory>($"{p.FileDescription} ({string.Join(", ", ((string?[])[.. p.FileExtensions.Take(VisibleExtensionsCount), (p.FileExtensions.Length > VisibleExtensionsCount ? "etc." : null)]).NotNull())})", p.ContentVisualizerFactory))];
    }
    private static async Task<IViewModel?> FindContentVisualizer(string fileName, Func<IBackingStore> backingStoreFactory)
    {
        foreach (var j in KnownFileTypes.Where(p => p.FileExtensions.Contains(Path.GetExtension(fileName.ToLowerInvariant()))))
        {
            if (await j.ContentVisualizerFactory.Invoke(backingStoreFactory, fileName) is { } visualizer)
            {
                return visualizer;
            }
        }
        return null;
    }

    /// <summary>
    /// Gets and initializes a ViewModel that can be used to view or edit the
    /// specified file.
    /// </summary>
    /// <param name="file">File to be displayed or edited.</param>
    /// <param name="backingStoreFactory">
    /// Factory that gets the backing store from which to read and optionally
    /// write the requested file.
    /// </param>
    /// <param name="dialogSvc">
    /// Dialog service used to provide interactivity.
    /// </param>
    /// <returns>
    /// A <see cref="Task{T}"/> that returns a ViewModel to display or edit the
    /// requested file upon completion.
    /// </returns>
    public static async Task<IViewModel> GetViewModel(string file, Func<IBackingStore> backingStoreFactory, IDialogService? dialogSvc)
    {
        IViewModel vm;
        try
        {
            Task<IViewModel?>? task = PlatformServices.ModifierKey switch
            {
                ModifierKey.Alt => CreateExternalEditorViewModel(backingStoreFactory, file),
                ModifierKey.Ctrl when dialogSvc is not null => (await dialogSvc.SelectOption(
                    Dialogs.OpenAs, SelectableVisualizers())) is { Success: true, Result: { } factory }
                    ? factory.Invoke(backingStoreFactory, file)
                    : null,
                _ => FindContentVisualizer(file, backingStoreFactory)
            };
            vm = task is not null && await task is { } x ? x : FileErrorViewModel.UnknownFileFormat;
        }
        catch (Exception ex)
        {
#if DEBUG
            await (dialogSvc?.Error(ex) ?? Task.CompletedTask);
#else
            if (dialogSvc is not null) (await dialogSvc.Show(Dialogs.CorruptFileError(file), DialogOptions.CopyExToClipboard(ex))).Invoke();
#endif
            vm = new FileErrorViewModel(ex);
        }
        vm.Title = file;
        return vm;
    }

    private static Task<IViewModel?>  CreateComingSoonViewModel(Func<IBackingStore> _, string __) => Task.FromResult<IViewModel?>(new ComingSoonViewModel());

    private static Task<IViewModel?> CreateRawReadOnlyViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return TryCreateViewModel(backingStoreFactory, name, (data, _) => Task.FromResult<IViewModel?>(new RawContentViewModel(data)));
    }

    private static Task<IViewModel?> CreateExternalEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return Task.FromResult<IViewModel?>(new ExternalFileViewModel(backingStoreFactory.Invoke(), name));
    }

    private static Task<IViewModel?> CreateEditorViewModel<TViewModel, TState, TFile, TSerializer>(Func<IBackingStore> backingStoreFactory, string name, Action<TSerializer>? serializerConfigCallback = null)
        where TFile : notnull
        where TViewModel : notnull, IFileEditorViewModel<TState, TFile>, new()
        where TState : notnull, IFileState<TFile>, new()
        where TSerializer : notnull, ISerializer<TFile>, new()
    {
        return TryCreateViewModel(backingStoreFactory, name, (data, store) => CreateEditorViewModel<TViewModel, TState, TFile, TSerializer>(data, store, name, serializerConfigCallback));
    }

    private static async Task<IViewModel?> CreateEditorViewModel<TViewModel, TState, TFile, TSerializer>(byte[] data, IBackingStore store, string name, Action<TSerializer>? serializerConfigCallback = null)
        where TFile : notnull
        where TViewModel : notnull, IFileEditorViewModel<TState, TFile>, new()
        where TState : notnull, IFileState<TFile>, new()
        where TSerializer : notnull, ISerializer<TFile>, new() => new TViewModel()
        {
            Title = name,
            State = new TState() { File = await CreateSerializer<TSerializer, TFile>(serializerConfigCallback).DeserializeAsync(data) },
            BackingStore = new BackingStore<TFile, TSerializer>(store, serializerConfigCallback) { FileName = name },
        };

    private static TSerializer CreateSerializer<TSerializer, TFile>(Action<TSerializer>? configCallback = null) where TSerializer : notnull, ISerializer<TFile>, new()
    {
        TSerializer serializer = new();
        configCallback?.Invoke(serializer);
        return serializer;
    }

    private static async Task<IViewModel?> TryCreateViewModel(Func<IBackingStore> backingStoreFactory, string name, Func<byte[], IBackingStore, Task<IViewModel?>> viewModelFactory)
    {
        IBackingStore store = backingStoreFactory.Invoke();
        return await store.ReadAsync(name) is byte[] data
            ? await viewModelFactory.Invoke(data, store)
            : FileErrorViewModel.FileNotFound;
    }
}