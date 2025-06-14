using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.ViewModels.Asf;

/// <summary>
/// Represents the view model for managing playback and export of MUS/ASF audio
/// streams.
/// </summary>
public class MusPlayerViewModel : ViewModel, IViewModel
{
    private WaveOutEvent? outputDevice;
    private WaveFileReader? audioFile;
    private MapFile? linearMap;
    private Timer? playbackTimer;

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
    /// Calculates the total duration of all audio streams in the MUS file.
    /// </summary>
    public TimeSpan Duration => GetSubStreams().Select(p => p.CalculatedDuration).Aggregate(TimeSpan.Zero, (accumulator, timeSpan) => accumulator + timeSpan);

    public TimeSpan CurrentPosition => audioFile?.CurrentTime ?? TimeSpan.Zero;

    /// <summary>
    /// Gets a reference to the MUS file being played by this view model.
    /// </summary>
    public required MusFile Mus { get; init; }

    /// <summary>
    /// Gets the file name of the MUS file being played.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets a reference to the backing store to use when reading files related
    /// to the MUS file being played.
    /// </summary>
    public required IBackingStore BackingStore { get; init; }

    /// <summary>
    /// Represents a file that contains a linear playback map for the MUS file.
    /// </summary>
    public MapFile? LinearMap
    {
        get => linearMap;
        set
        {
            if (Change(ref linearMap, value))
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

    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        if (await BackingStore.ReadAsync($"{Path.GetFileNameWithoutExtension(FileName)}.lin") is byte[] rawLin)
        {
            try
            {
                LinearMap = await ((ISerializer<MapFile>)new MapSerializer()).DeserializeAsync(rawLin);
            }
            catch
            {
                Debug.Print("Error loading linear map file for the current MUS file. Ignoring...");
            }
        }
    }

    private void OnPlaySample()
    {
        if (outputDevice is { PlaybackState: PlaybackState.Playing }) return;
        var rawWav = new MemoryStream(GetAudioStream());
        outputDevice = new WaveOutEvent();
        audioFile = new WaveFileReader(rawWav);
        outputDevice.PlaybackStopped += OnPlaybackEndReached;
        outputDevice.Init(audioFile);
        outputDevice.Play();
        playbackTimer = new Timer(OnUpdatePlaybackPosition, null, 0, 1000);
    }

    private void OnPlaybackEndReached(object? _, StoppedEventArgs __) => OnStopPlayback();

    private void OnUpdatePlaybackPosition(object? state)
    {
        Notify(nameof(CurrentPosition));
    }

    private void OnStopPlayback()
    {
        if (outputDevice is not null)
        {
            outputDevice.Stop();
            outputDevice.PlaybackStopped -= OnPlaybackEndReached;
        }
        playbackTimer?.Dispose();
        audioFile?.Dispose();
        outputDevice?.Dispose();
        playbackTimer = null;
        audioFile = null;
        outputDevice = null;
        GC.Collect();
        Notify(nameof(CurrentPosition));
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
        var jointStreams = AudioRender.JoinStreams(GetSubStreams());
        return AudioRender.RenderData(jointStreams, [.. jointStreams.AudioBlocks.SelectMany(p => p)]);
    }

    private IEnumerable<AsfFile> GetSubStreams()
    {
        if (LinearMap is not null)
        {
            throw new NotImplementedException("Linear playback is not yet implemented for MUS files.");
            //return Mus.AsfSubStreams.Values.Where(p => LinearMap.LinearPlaybackOffsets.Contains(p.StreamOffset));
        }
        return Mus.AsfSubStreams.Values;
    }

    Task IViewModel.OnNavigateAway(CancelFlag navigation)
    {
        OnStopPlayback();
        return Task.CompletedTask;
    }

    Task IViewModel.OnNavigateBack(CancelFlag navigation)
    {
        OnStopPlayback();
        return Task.CompletedTask;
    }
}
