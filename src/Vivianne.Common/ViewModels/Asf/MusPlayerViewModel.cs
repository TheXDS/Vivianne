using NAudio.Wave;
using System;
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
using TheXDS.Vivianne.Tools.Audio.Mus;

namespace TheXDS.Vivianne.ViewModels.Asf;

/// <summary>
/// Represents the view model for managing playback and export of MUS/ASF audio
/// streams.
/// </summary>
public class MusPlayerViewModel : ViewModel, IViewModel
{
    private WaveOutEvent? outputDevice;
    private MapFile? linearMap;
    private MapFile? interactiveMap;
    private Timer? playbackTimer;
    private bool _playLooping;
    private TimeSpan _duration;
    private EaAudioWaveStream? _audioFile;

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
    /// Gets a value that indicates whether the audio stream can be played
    /// in a looping manner based on the presence of a .LIN file.
    /// </summary>
    public bool CanPlayLooping => LinearMap is not null;

    /// <summary>
    /// Gets a value that indicates whether the audio stream can be played
    /// in an interactive manner based on the presence of a .MAP file.
    /// </summary>
    public bool CanPlayInteractive => InteractiveMap is not null;

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
            if (Change(ref linearMap, value))
            {
                Notify(nameof(CanPlayLooping));
                OnStopPlayback();
            }
        }
    }

    /// <summary>
    /// Represents a file that contains an interactive playback map for the MUS file.
    /// </summary>
    public MapFile? InteractiveMap
    {
        get => interactiveMap;
        set
        {
            if (Change(ref interactiveMap, value))
            {
                Notify(nameof(CanPlayLooping));
                OnStopPlayback();
            }
        }
    }

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
    /// Calculates the total duration of all audio streams in the MUS file.
    /// </summary>
    public TimeSpan Duration
    {
        get => _duration;
        private set => Change(ref _duration, value);
    }

    /// <summary>
    /// Gets the current playback position of the audio stream.
    /// </summary>
    public TimeSpan CurrentPosition => TimeSpan.FromSeconds(_audioFile?.CurrentPositionInSeconds ?? 0);

    /// <summary>
    /// Gets or sets a value that indicates whether the audio stream should
    /// be played in a looping manner based on its PT header properties.
    /// </summary>
    public bool PlayLooping
    {
        get => _playLooping;
        set => Change(ref _playLooping, value);
    }

    /// <summary>
    /// Gets a reference to the audio stream being played, which is represented
    /// as a <see cref="MusWaveStream"/> that wraps the MUS file and its
    /// associated map files.
    /// </summary>
    public EaAudioWaveStream? AudioFile
    {
        get => _audioFile;
        private set => Change(ref _audioFile, value);
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
        switch (Mus.AsfSubStreams.Count)
        {
            case 0:
                await DialogService!.Error("No audio streams found in the MUS file. Playback will be unavailable.");
                break;
            case 1:
                var asf = Mus.AsfSubStreams.Values.Single();
                Duration = asf.CalculatedDuration;
                break;
            default:
                await Task.WhenAll([
                    TryLoadMap("lin", map => LinearMap = map),
                    TryLoadMap("map", map => InteractiveMap = map)]);
                Duration = CalculateLinearDuration();
                break;
        }
    }

    private async Task TryLoadMap(string extension, Action<MapFile> setMap)
    {
        if (await BackingStore.ReadAsync($"{Path.GetFileNameWithoutExtension(FileName)}.{extension}") is byte[] rawData)
        {
            try
            {
                var map = await ((ISerializer<MapFile>)new MapSerializer()).DeserializeAsync(rawData);
                setMap(map);
            }
            catch
            {
                Debug.Print($"Error loading .{extension} file for the current .mus file. Ignoring...");
            }
        }
    }

    private TimeSpan CalculateLinearDuration()
    {
        TimeSpan duration = TimeSpan.Zero;
        if (LinearMap is not null)
        {
            (var indices, _) = MapStitcher.Stitch(LinearMap);
            foreach (var substream in indices.Select(p => Mus.AsfSubStreams.Values.ElementAt(p)))
            {
                duration += substream.CalculatedDuration;
            }
        }
        return duration;
    }

    private void OnPlaySample()
    {
        if (outputDevice is { PlaybackState: PlaybackState.Playing }) return;
        outputDevice = new WaveOutEvent();
        outputDevice.PlaybackStopped += OnPlaybackEndReached;
        outputDevice.Init(AudioFile = CreateWaveStream());
        outputDevice.Play();
        playbackTimer = new Timer(OnUpdatePlaybackPosition, null, 0, 1000);
    }

    private EaAudioWaveStream CreateWaveStream()
    {
        if (PlayLooping && LinearMap is not null)
        {
            return new MusWaveStream(Mus, LinearMap);
        }
        else if (InteractiveMap is not null)
        {
            return new MusWaveStream(Mus, InteractiveMap);
        }
        else
        {
            return new AsfWaveStream(Mus.AsfSubStreams.Single().Value) { PlayLooping = true };
        }
    }

    private void OnPlaybackEndReached(object? _, StoppedEventArgs __) => OnStopPlayback();

    private void OnUpdatePlaybackPosition(object? state)
    {
        Notify(nameof(CurrentPosition));
    }

    private void OnStopPlayback()
    {
        playbackTimer?.Dispose();
        if (outputDevice is not null)
        {
            outputDevice.PlaybackStopped -= OnPlaybackEndReached;
            outputDevice.Stop();
        }
        outputDevice?.Dispose();
        playbackTimer = null;
        AudioFile = null;
        outputDevice = null;
        GC.Collect();
        Notify(nameof(CurrentPosition));
    }

    private async Task OnExportAudio()
    {
        if (LinearMap is not null && await DialogService!.GetFileSavePath(FileFilters.AudioFileFilter) is { Success: true, Result: string path })
        {
            (var header, var data) = AudioRender.JoinAllStreams(Mus, LinearMap);
            await File.WriteAllBytesAsync(path, AudioRender.RenderData(header, data));
        }
    }

    //private AsfFile GetPreRenderStream()
    //{
    //    (var streams, var loopOffset) = GetSubStreams();


    //    int[] indices = GetIndices();

    //    var asf = AudioRender.JoinStreams(SelectIndices([.. streams], indices));
    //    asf.LoopStart = PlayLooping ? loopOffset: 0;
    //    asf.LoopEnd = PlayLooping ? asf.TotalSamples / asf.BytesPerSample : 0;
    //    return asf;
    //}

    //private int[] GetIndices()
    //{
    //    List<int> sequence = [];
    //    int current = LinearMap.FirstItem;
    //    while (!sequence.Contains(current))
    //    {
    //        sequence.Add(current);
    //        current = LinearMap.Items[current].Jumps.FirstOrDefault()?.NextItem ?? 0;
    //    }
    //    return [.. sequence];
    //}

    //private static IEnumerable<AsfFile> SelectIndices(AsfFile[] files, int[] indices)
    //{
    //    return indices.Select(i => files[i]);
    //}

    //private (AsfFile[] streams, int loopOffset) GetSubStreams()
    //{
    //    //if (LinearMap is not null)
    //    //{
    //    //    List<int> sequence = [];
    //    //    int current = LinearMap.FirstItem;
    //    //    while (!sequence.Contains(current))
    //    //    {
    //    //        sequence.Add(current);
    //    //        current = LinearMap.Items[current].Jumps.FirstOrDefault()?.NextItem ?? 0;
    //    //    }

    //    //    return (sequence.Select(p => Mus.AsfSubStreams.Values.ElementAt(p)).ToArray(), Mus.AsfSubStreams.Keys.ElementAt(current));
    //    //}
    //    return (Mus.AsfSubStreams.Values.ToArray(), 0);
    //}

    //private static byte[] GetRawWav(AsfFile jointStreams)
    //{
    //    return AudioRender.RenderData(jointStreams, [.. jointStreams.AudioBlocks.SelectMany(p => p)]);
    //}

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
