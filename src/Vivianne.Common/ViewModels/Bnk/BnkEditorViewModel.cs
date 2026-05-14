using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Tools.Audio;
using TheXDS.Vivianne.Tools.Audio.Bnk;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.BnkEditorViewModel;

namespace TheXDS.Vivianne.ViewModels.Bnk;

/// <summary>
/// ViewModel for the BNK file editor.
/// </summary>
public class BnkEditorViewModel : StatefulFileEditorViewModelBase<BnkEditorState, BnkFile>, IViewModel
{
    private WaveOutEvent? _outputDevice;
    private BnkWaveStream? _audioFile;
    private bool _isPlaying;

    /// <summary>
    /// Gets a reference to the command used to start playing an audio stream.
    /// </summary>
    public ICommand PlaySampleCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to start playing the looping
    /// section of an audio stream.
    /// </summary>
    public ICommand PlayLoopingSampleCommand { get; }

    /// <summary>
    /// Gets a referenceto the command used to stop playback of the audio
    /// stream.
    /// </summary>
    public ICommand StopPlaybackCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to export the currently selected
    /// audio stream to a .WAV file.
    /// </summary>
    public ICommand ExportSampleCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to export the looping section of
    /// the audio stream to a .WAV file.
    /// </summary>
    public ICommand ExportLoopCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to import a .WAV file to the
    /// current audio stream, replacing its contents.
    /// </summary>
    public ICommand ImportWavCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to import a .WAV file as the
    /// alternate stream for the selected BNK stream.
    /// </summary>
    public ICommand ImportAsAltStreamCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to remove an alt. stream from the
    /// selected BNK stream.
    /// </summary>
    public ICommand RemoveAltStreamCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to normalize the selected stream's
    /// volume.
    /// </summary>
    public ICommand NormalizeVolumeCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BnkEditorViewModel"/>
    /// class.
    /// </summary>
    public BnkEditorViewModel()
    {
        PlaySampleCommand = new SimpleCommand(OnPlaySample);
        PlayLoopingSampleCommand = new SimpleCommand(OnPlayLoopingSample);
        StopPlaybackCommand = new SimpleCommand(OnStopPlayback);
        ExportSampleCommand = new SimpleCommand(OnExportSample);
        ExportLoopCommand = new SimpleCommand(OnExportLoop);
        ImportWavCommand = new SimpleCommand(OnImportWav);
        ImportAsAltStreamCommand = new SimpleCommand(OnImportAsAltStream);
        NormalizeVolumeCommand = new SimpleCommand(OnNormalizeVolume);
        RemoveAltStreamCommand = new SimpleCommand(OnRemoveAltStream);
    }

    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        State.SelectedStream = State.Streams.NotNull().FirstOrDefault();
        State.ShowInfo = Settings.Current.Bnk_InfoOpenByDefault;
        State.Subscribe(OnStateChanged);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override bool BeforeSave()
    {
        if (!Settings.Current.Bnk_KeepTrash)
        {
            foreach (var stream in State.AllStreams)
            {
                stream.PostAudioStreamData = [];
            }
        }
        return base.BeforeSave();
    }

    private async void OnStateChanged(object instance, PropertyInfo property, PropertyChangeNotificationType notificationType)
    {
        if (_isPlaying)
        {
            await OnPlayLoopingSample();
        }
    }

    private void OnPlaySample()
    {
        if (State.SelectedStream is not { } sample) return;
        InitAudioStream(sample, false);
    }

    private async Task OnPlayLoopingSample()
    {
        if (State.SelectedStream is not { } sample) return;
        if (State.SelectedStream.LoopEnd == 0)
        {
            OnStopPlayback();
            await DialogService!.Message(St.BNKStream, St.NoLoop);
            return;
        }
        InitAudioStream(sample, true);
        _isPlaying = true;
    }

    private void OnStopPlayback()
    {
        _isPlaying = false;
        if (_outputDevice is not null)
        {
            _outputDevice.Stop();
            _outputDevice.Dispose();
            _outputDevice = null;
        }
        _audioFile = null;
    }

    private void InitAudioStream(BnkStream sample, bool looping)
    {
        OnStopPlayback();
        _audioFile = new BnkWaveStream(sample, State.PlaybackSpeed) { PlayLooping = looping };
        if (looping)
        {
            _audioFile.ResetToLoopStart();
        }
        _outputDevice = new WaveOutEvent();
        _outputDevice.Init(_audioFile);
        _outputDevice.Play();
    }

    private async Task OnImportWav()
    {
        try
        {
            if (State.SelectedStream is null) return;
            if (await DialogService!.GetFileOpenPath(FileFilters.AudioFileFilter) is { Success: true, Result: string path })
            {
                var stream = AudioRender.BnkFromWav(await File.ReadAllBytesAsync(path));
                State.SelectedStream.Channels = stream.Channels;
                State.SelectedStream.Compression = stream.Compression;
                State.SelectedStream.SampleRate = stream.SampleRate;
                State.SelectedStream.BytesPerSample = stream.BytesPerSample;
                State.SelectedStream.SampleData = stream.SampleData;
                State.LoopStart = stream.LoopStart;
                State.LoopEnd = stream.LoopEnd;
                State.Refresh();
            }
        }
#if DEBUG
        catch (Exception ex)
        {
            await DialogService!.Error(ex);
#else
        catch
        {
            await DialogService!.Error("Cannot import .WAV", "The file you selected does not appear to be a valid .WAV file.");
#endif
            return;
        }
    }

    private async Task OnImportAsAltStream()
    {
        if (State.SelectedStream is null) return;
        if (State.SelectedStream.IsAltStream)
        {
            await DialogService!.Error(St.ImportAltStream, St.NestedAltStreamsError);
            return;
        }
        if (await DialogService!.GetFileOpenPath(FileFilters.AudioFileFilter) is { Success: true, Result: string path })
        {
            State.SelectedStream.AltStream = AudioRender.BnkFromWav(await File.ReadAllBytesAsync(path));
            State.Refresh();
        }
    }

    private async Task OnExportSample()
    {
        if (State.SelectedStream is not { } sample) return;
        if (await DialogService!.GetFileSavePath(FileFilters.AudioFileFilter) is { Success: true, Result: string path })
        {
            await File.WriteAllBytesAsync(path, AudioRender.RenderBnk(sample));
        }
    }

    private async Task OnExportLoop()
    {
        if (State.SelectedStream is not { } sample) return;
        if (await DialogService!.GetFileSavePath(FileFilters.AudioFileFilter) is { Success: true, Result: string path })
        {
            await File.WriteAllBytesAsync(path, AudioRender.RenderBnkLoop(sample));
        }
    }

    private async Task OnNormalizeVolume()
    {
        var result = await DialogService!.GetInputValue(
            CommonDialogTemplates.Input with
            {
                Icon = "🔊",
                Title = St.Normalize,
                Text = St.NormalizeText
            }, 0.0, 1.0, Settings.Current.Bnk_DefaultNormalization);
        if (!result.Success) return;
        OnStopPlayback();
        UiThread.Invoke(() => State.SelectedStream!.SampleData = BnkNormalizer.NormalizeVolume(State.SelectedStream, result.Result));
        State.Refresh();
    }

    private void OnRemoveAltStream()
    {
        if (State.SelectedStream?.AltStream is null) return;
        State.SelectedStream.AltStream = null;
        State.Refresh();
    }

    Task IViewModel.OnNavigateAway(CancelFlag navigation)
    {
        OnStopPlayback();
        return Task.CompletedTask;
    }
}
