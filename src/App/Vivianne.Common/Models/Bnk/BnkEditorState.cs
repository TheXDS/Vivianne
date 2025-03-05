using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models.Bnk;

public class BnkEditorState : FileStateBase<BnkFile>
{
    private ObservableListWrap<BnkBlob> _samples;
    private BnkBlob? selectedSample;
    private bool loop;

    public ObservableListWrap<BnkBlob> Samples => _samples ??= GetObservable(File.Blobs);

    public BnkBlob? SelectedSample
    {
        get => selectedSample;
        set => Change(ref selectedSample, value);
    }

    public bool Loop
    {
        get => loop;
        set => Change(ref loop, value);
    }

    
}
