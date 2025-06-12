using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Asf;

/// <summary>
/// Represents the view model for managing playback and export of MUS/ASF audio
/// streams.
/// </summary>
public class MusPlayerViewModel : ViewModel, IViewModel
{
    private readonly SoundPlayer _snd = new();
    private bool _isPlaying;
    private MapFile? linearMap;

    /// <summary>
    /// Gets a reference to the command used to start playing an audio stream.
    /// </summary>
    public ICommand PlaySampleCommand { get; }

    /// <summary>
    /// Gets a referenceto the command used to stop playback of the audio
    /// stream.
    /// </summary>
    public ICommand StopPlaybackCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to export the MUS/ASF
    /// audio stream to a .WAV file.
    /// </summary>
    public ICommand ExportAudioCommand { get; }

    /// <summary>
    /// Gets a reference to the MUS file being played by this view model.
    /// </summary>
    public required MusFile Mus { get; init; }

    /// <summary>
    /// Represents a file that contains a linear playback map for the MUS file.
    /// </summary>
    public MapFile? LinearMap
    {
        get => linearMap;
        set
        {
            if (Change(ref linearMap, value) && _isPlaying)
            {
                OnStopPlayback();
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusPlayerViewModel"/> class.
    /// </summary>
    public MusPlayerViewModel()
    {
        PlaySampleCommand = new SimpleCommand(OnPlaySample);
        StopPlaybackCommand = new SimpleCommand(OnStopPlayback);
        ExportAudioCommand = new SimpleCommand(OnExportAudio);
    }

    private void OnPlaySample()
    {
        SetSound(GetAudioStream());
        _snd.Play();
    }

    private void OnStopPlayback()
    {
        _isPlaying = false;
        _snd.Stop();
    }

    private async Task OnExportAudio()
    {
        if (await DialogService!.GetFileSavePath(FileFilters.AudioFileFilter) is { Success: true, Result: string path })
        {
            await File.WriteAllBytesAsync(path, GetAudioStream());
        }
    }

    private byte[] GetAudioStream()
    {
        (var audioProps, var rawStream) = AudioRender.JoinAllStreams(Mus);
        return AudioRender.RenderData(audioProps, rawStream);
    }

    private void SetSound(byte[] data)
    {
        _isPlaying = false;
        _snd.Stop();
        _snd.Stream?.Dispose();
        _snd.Stream = new MemoryStream(data);
    }

    Task IViewModel.OnNavigateAway(CancelFlag navigation)
    {
        OnStopPlayback();
        return Task.CompletedTask;
    }
}

public class MusPlayerViewModelLauncher : ViewModel, IFileEditorViewModelLauncher
{
    private static readonly ISerializer<MusFile> Serializer = new MusSerializer();

    private readonly IEnumerable<FileFilterItem> _openFilter;

    /// <inheritdoc/>
    public RecentFileInfo[] RecentFiles { get; set; }

    /// <inheritdoc/>
    public ICommand NewFileCommand { get; }

    /// <inheritdoc/>
    public ICommand OpenFileCommand { get; }

    /// <inheritdoc/>
    public bool CanCreateNew => false;

    /// <inheritdoc/>
    public string PageName => "ASF/MUS";

    /// <inheritdoc/>
    public IEnumerable<ButtonInteraction> AdditionalInteractions => throw new System.NotImplementedException();

    public MusPlayerViewModelLauncher()
    {
        RecentFiles = Settings.Current.RecentAsfFiles;
        NewFileCommand = new SimpleCommand(() => throw new TamperException(), false);
        OpenFileCommand = new SimpleCommand(p => DialogService?.RunOperation(q => OnOpen(p)) ?? Task.CompletedTask);
    }

    /// <inheritdoc/>
    public bool CanOpen(string fileExtension)
    {
        return ((string[])["asf", "mus"]).Contains(fileExtension.ToLowerInvariant());
    }

    /// <inheritdoc/>
    public async Task OnOpen(object parameter)
    {
        if (await GetFilePath(parameter, [], _openFilter) is not string filePath) return;
        var file = await Serializer.DeserializeAsync(File.OpenRead(filePath));
        var recentFile = CreateRecentFileInfo(filePath, file);
        RecentFiles = Settings.Current.RecentFilesCount > 0 ? [recentFile, .. (RecentFiles?.Where(p => p.FilePath != filePath) ?? []).Take(Settings.Current.RecentFilesCount - 1)] : [];
        Notify(nameof(RecentFiles));
        await Settings.Save();
        var vm = new MusPlayerViewModel()
        {
            Title = recentFile.FriendlyName,
            Mus = file,
        };
        await NavigationService!.Navigate(vm);
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
    protected virtual RecentFileInfo CreateRecentFileInfo(string path, MusFile file)
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
        IsBusy = true;
        try
        {
            if (!await Task.Run(() => File.Exists(file.FilePath)))
            {
                await (DialogService?.Error("St.FileNotFound", "St.FileNotFound2") ?? Task.CompletedTask);
                return null;
            }
            return file.FilePath;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<string?> TryOpenFile(IEnumerable<FileFilterItem> filters)
    {
        var f = await DialogService!.GetFileOpenPath(filters);
        return f.Result;
    }
}