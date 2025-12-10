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
using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Geo;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio.Bnk;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.Serializers.Geo;
using TheXDS.Vivianne.Serializers.Viv;
using TheXDS.Vivianne.ViewModels;
using TheXDS.Vivianne.ViewModels.Asf;
using TheXDS.Vivianne.ViewModels.Base;
using TheXDS.Vivianne.ViewModels.Bnk;
using TheXDS.Vivianne.ViewModels.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Fce.Nfs4;
using TheXDS.Vivianne.ViewModels.Fe;
using TheXDS.Vivianne.ViewModels.Fsh;
using TheXDS.Vivianne.ViewModels.Geo;
using TheXDS.Vivianne.ViewModels.Viv;
using Cp2 = TheXDS.Vivianne.Models.Carp.Nfs2.CarPerf;
using Cp3 = TheXDS.Vivianne.Models.Carp.Nfs3.CarPerf;
using Cp4 = TheXDS.Vivianne.Models.Carp.Nfs4.CarPerf;
using Fe3 = TheXDS.Vivianne.Models.Fe.Nfs3.FeData;
using Fe4 = TheXDS.Vivianne.Models.Fe.Nfs4.FeData;
using MFce3 = TheXDS.Vivianne.Models.Fce.Nfs3;
using MFce4 = TheXDS.Vivianne.Models.Fce.Nfs4;
using SCp2 = TheXDS.Vivianne.Serializers.Carp.Nfs2.CarpSerializer;
using SCp3 = TheXDS.Vivianne.Serializers.Carp.Nfs3.CarpSerializer;
using SCp4 = TheXDS.Vivianne.Serializers.Carp.Nfs4.CarpSerializer;
using SFe3 = TheXDS.Vivianne.Serializers.Fe.Nfs3.FeDataSerializer;
using SFe4 = TheXDS.Vivianne.Serializers.Fe.Nfs4.FeDataSerializer;
using SNfs3 = TheXDS.Vivianne.Serializers.Fce.Nfs3.FceSerializer;
using SNfs4 = TheXDS.Vivianne.Serializers.Fce.Nfs4.FceSerializer;
using St = TheXDS.Vivianne.Resources.Strings.FileFilters;
using VCp2 = TheXDS.Vivianne.ViewModels.Carp.Nfs2.CarpEditorViewModel;
using VCp3 = TheXDS.Vivianne.ViewModels.Carp.Nfs3.CarpEditorViewModel;
using VCp4 = TheXDS.Vivianne.ViewModels.Carp.Nfs4.CarpEditorViewModel;
using VmFce3 = TheXDS.Vivianne.ViewModels.Fce.Nfs3.Fce3EditorViewModel;
using VmFce4 = TheXDS.Vivianne.ViewModels.Fce.Nfs4.Fce4EditorViewModel;
using VsCp2 = TheXDS.Vivianne.Models.Carp.Nfs2.CarpEditorState;
using VsCp3 = TheXDS.Vivianne.Models.Carp.Nfs3.CarpEditorState;
using VsCp4 = TheXDS.Vivianne.Models.Carp.Nfs4.CarpEditorState;

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
public static class FileTypes
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


    private static Task<IViewModel?> CreateVivEditorViewModel(Func<IBackingStore> backingStoreFactory, string fileName)
    {
        return CreateEditorViewModel<VivEditorViewModel, VivEditorState, VivFile, VivSerializer>(backingStoreFactory, fileName);
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

    private static Task<IViewModel?> CreateFeDataEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return TryCreateViewModel(backingStoreFactory, name, (data, store) => VersionIdentifier.FeDataVersion(data) switch
        {
            NfsVersion.Nfs3 => CreateEditorViewModel<FeData3EditorViewModel, FeData3EditorState, Fe3, SFe3>(data, store, name),
            NfsVersion.Nfs4 => CreateEditorViewModel<FeData4EditorViewModel, FeData4EditorState, Fe4, SFe4>(data, store, name),
            _ => Task.FromResult<IViewModel?>(null)
        });
    }

    private static Task<IViewModel?> CreateCarpEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return TryCreateViewModel(backingStoreFactory, name, (data, store) => VersionIdentifier.CarpVersion(data) switch
        {
            NfsVersion.Nfs2 => CreateEditorViewModel<VCp2, VsCp2, Cp2, SCp2>(data, store, name),
            NfsVersion.Nfs3 => CreateEditorViewModel<VCp3, VsCp3, Cp3, SCp3>(data, store, name),
            NfsVersion.Nfs4 => CreateEditorViewModel<VCp4, VsCp4, Cp4, SCp4>(data, store, name),
            _ => Task.FromResult<IViewModel?>(null)
        });
    }

    private static Task<IViewModel?> CreateTexturePreviewViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return CreateEditorViewModel<TexturePreviewViewModel, RawFileEditorState, RawFile, RawFileSerializer>(backingStoreFactory, name);
    }

    private static Task<IViewModel?> CreateFceEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return TryCreateViewModel(backingStoreFactory, name, (data, store) =>  VersionIdentifier.FceVersion(data) switch
        {
            NfsVersion.Nfs3 => CreateEditorViewModel<VmFce3, Fce3EditorState, MFce3.FceFile, SNfs3>(data, store, name),
            NfsVersion.Nfs4 or NfsVersion.Mco => CreateEditorViewModel<VmFce4, Fce4EditorState, MFce4.FceFile, SNfs4>(data, store, name),
            _ => Task.FromResult<IViewModel?>(null)
        });
    }

    private static Task<IViewModel?> CreateGeoEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return CreateEditorViewModel<GeoEditorViewModel, GeoEditorState, GeoFile, GeoSerializer>(backingStoreFactory, name);
    }

    private static Task<IViewModel?> CreateFshEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return CreateEditorViewModel<FshEditorViewModel, FshEditorState, FshFile, FshSerializer>(backingStoreFactory, name);
    }

    private static Task<IViewModel?> CreateBnkEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return CreateEditorViewModel<BnkEditorViewModel, BnkEditorState, BnkFile, BnkSerializer>(backingStoreFactory, name);
    }

    private static Task<IViewModel?> CreateMusPlayerViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return TryCreateViewModel(backingStoreFactory, name, async (data, store) => new MusPlayerViewModel()
        {
            Title = name,
            Mus = await ((ISerializer<MusFile>)new MusSerializer()).DeserializeAsync(data),
            FileName = name,
            BackingStore = store
        });
    }

    private static Task<IViewModel?> CreateEditorViewModel<TViewModel, TState, TFile, TSerializer>(Func<IBackingStore> backingStoreFactory, string name)
        where TFile : notnull
        where TViewModel : notnull, IFileEditorViewModel<TState, TFile>, new()
        where TState : notnull, IFileState<TFile>, new()
        where TSerializer : notnull, ISerializer<TFile>, new()
    {
        return TryCreateViewModel(backingStoreFactory, name, (data, store) => CreateEditorViewModel<TViewModel, TState, TFile, TSerializer>(data, store, name));
    }

    private static async Task<IViewModel?> CreateEditorViewModel<TViewModel, TState, TFile, TSerializer>(byte[] data, IBackingStore store, string name)
        where TFile : notnull
        where TViewModel : notnull, IFileEditorViewModel<TState, TFile>, new()
        where TState : notnull, IFileState<TFile>, new()
        where TSerializer : notnull, ISerializer<TFile>, new() => new TViewModel()
        {
            Title = name,
            State = new TState() { File = await new TSerializer().DeserializeAsync(data) },
            BackingStore = new BackingStore<TFile, TSerializer>(store) { FileName = name },
        };

    private static async Task<IViewModel?> TryCreateViewModel(Func<IBackingStore> backingStoreFactory, string name, Func<byte[], IBackingStore, Task<IViewModel?>> viewModelFactory)
    {
        IBackingStore store = backingStoreFactory.Invoke();
        return await store.ReadAsync(name) is byte[] data
            ? await viewModelFactory.Invoke(data, store)
            : FileErrorViewModel.FileNotFound;
    }
}