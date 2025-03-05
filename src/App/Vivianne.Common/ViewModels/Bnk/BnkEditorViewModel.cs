using System.Media;
using System.Windows.Input;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Bnk;

public class BnkEditorViewModel : FileEditorViewModelBase<BnkEditorState, BnkFile>
{
    public ICommand PlaySampleCommand { get; }

    public BnkEditorViewModel()
    {
        PlaySampleCommand = new SimpleCommand(OnPlaySample);
    }

    private void OnPlaySample()
    {
        if (State.SelectedSample is not { } sample) return;
        SoundPlayer snd = new SoundPlayer(BnkRender.RenderBnk(sample));
        snd.PlayLooping();
    }
}
