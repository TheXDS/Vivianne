using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Tools.Audio;

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
