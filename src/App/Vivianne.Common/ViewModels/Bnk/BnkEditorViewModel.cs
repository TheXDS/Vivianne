using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Bnk;

public class BnkEditorViewModel : FileEditorViewModelBase<BnkEditorState, BnkFile>
{
    public ICommand PlaySampleCommand { get; }
    public ICommand ExportSampleCommand { get; }

    public BnkEditorViewModel()
    {
        PlaySampleCommand = new SimpleCommand(OnPlaySample);
        ExportSampleCommand = new SimpleCommand(OnExportSample);
    }

    private void OnPlaySample()
    {
        if (State.SelectedStream is not { } sample) return;
        SoundPlayer snd = new(new MemoryStream(BnkRender.RenderBnkLoop(sample)));
        snd.PlayLooping();
    }

    private async Task OnExportSample()
    {
        if (State.SelectedStream is not { } sample) return;
        if (await DialogService!.GetFileSavePath([new FileFilterItem("WAV audio file",".wav")]) is { Success:true, Result: string path })
        {
            await File.WriteAllBytesAsync(path, BnkRender.RenderBnk(sample));
        }
    }
}
