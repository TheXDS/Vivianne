using NAudio.Wave;
using System.Diagnostics;
using System.IO;
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
        var rawWav = new MemoryStream(GetAudioStream());
        outputDevice = new WaveOutEvent();
        audioFile = new WaveFileReader(rawWav) ;
        outputDevice.Init(audioFile);
        outputDevice.Play();
    }

    private void OnStopPlayback()
    {
        outputDevice?.Stop();
        audioFile?.Dispose();
        outputDevice?.Dispose();
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

    Task IViewModel.OnNavigateAway(CancelFlag navigation)
    {
        OnStopPlayback();
        return Task.CompletedTask;
    }
}
