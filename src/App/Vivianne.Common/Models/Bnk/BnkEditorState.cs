using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models.Bnk;

public class BnkEditorState : FileStateBase<BnkFile>
{
    private ObservableListWrap<BnkBlob> _streams;
    private BnkBlob? selectedSample;
    private bool loop = true;

    public ObservableListWrap<BnkBlob> Streams => _streams ??= GetObservable(File.Streams);

    public BnkBlob? SelectedStream
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
