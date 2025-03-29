using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Tools.Bnk;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.BnkEditorViewModel;
namespace TheXDS.Vivianne.ViewModels.Bnk;

/// <summary>
/// ViewModel for the BNK file editor.
/// </summary>
public class BnkEditorViewModel : FileEditorViewModelBase<BnkEditorState, BnkFile>, IViewModel
{
    private readonly SoundPlayer _snd = new();
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
    /// Gets a reference to the command used to remove all unused data from the
    /// BNK file.
    /// </summary>
    public ICommand RemoveUnusedDataCommand { get; }

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
        RemoveUnusedDataCommand = new SimpleCommand(OnRemoveUnusedData);
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
        SetSound(BnkRender.RenderBnk(sample));
        _snd.Play();
    }

    private async Task OnPlayLoopingSample()
    {
        if (State.SelectedStream is not { } sample) return;
        if (State.SelectedStream.LoopEnd == 0)
        {
            _snd.Stop();
            await DialogService!.Message(St.BNKStream, St.NoLoop);
            return;
        }
        SetSound(BnkRender.RenderBnkLoop(sample));
        _isPlaying = true;
        _snd.PlayLooping();
    }

    private void OnStopPlayback()
    {
        _isPlaying = false;
        _snd.Stop();
    }

    private async Task OnImportWav()
    {
        if (State.SelectedStream is null) return;
        if (await DialogService!.GetFileOpenPath(FileFilters.AudioFileFilter) is { Success: true, Result: string path })
        {
            var stream = BnkRender.FromWav(await File.ReadAllBytesAsync(path));
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
            State.SelectedStream.AltStream = BnkRender.FromWav(await File.ReadAllBytesAsync(path));
            State.Refresh();
        }
    }

    private async Task OnExportSample()
    {
        if (State.SelectedStream is not { } sample) return;
        if (await DialogService!.GetFileSavePath(FileFilters.AudioFileFilter) is { Success:true, Result: string path })
        {
            await File.WriteAllBytesAsync(path, BnkRender.RenderBnk(sample));
        }
    }

    private async Task OnExportLoop()
    {
        if (State.SelectedStream is not { } sample) return;
        if (await DialogService!.GetFileSavePath(FileFilters.AudioFileFilter) is { Success: true, Result: string path })
        {
            await File.WriteAllBytesAsync(path, BnkRender.RenderBnkLoop(sample));
        }
    }

    private async Task OnRemoveUnusedData()
    {
        if (!await (DialogService?.AskYn(St.RemoveUnusedData, St.RemoveUnusedDataQuestion) ?? Task.FromResult(true))) return;
        OnStopPlayback();
        foreach (var stream in State.AllStreams)
        {
            if (stream.LoopEnd > stream.LoopStart)
            {
                stream.SampleData = [.. stream.SampleData
                    .Skip(stream.LoopStart * stream.BytesPerSample)
                    .Take((stream.LoopEnd - stream.LoopStart) * stream.BytesPerSample)];
                stream.LoopStart = 0;
                stream.LoopEnd = stream.TotalSamples;
            }
            stream.PostAudioStreamData = [];
        }
        State.Refresh();
    }
    
    private async Task OnNormalizeVolume()
    {
        var result = await DialogService!.GetInputValue(
            CommonDialogTemplates.Input with 
            {
                Icon = "🔊",
                Title = "Normalize",
                Text = "Please enter a desired volume level relative to 1.0"
            }, 0.0, 1.0, 0.85);
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
