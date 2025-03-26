using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.Serializers.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to see and edit FeData car
/// information files for Need For Speed 3.
/// </summary>
public class FeData3EditorViewModel : FileEditorViewModelBase<FeData3EditorState, FeData>
{
    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        if (await (BackingStore?.Store.ReadAsync("car.fce") ?? Task.FromResult<byte[]?>(null)) is not byte[] fceBytes) return;
        FceSerializer serializer = new();
        if (serializer.TryGetFce(fceBytes) is { } fce)
        {
            State.PreviewFceColorTable = [.. ReadColorPairs(fce).Concat(Enumerable.Range(0, 10).Select(_ => (FceColor?)null)).Take(10)];
        }
    }

    private static FceColor?[] ReadColorPairs(FceFile fce)
    {
        static HsbColor WrapTable(IList<HsbColor> colors, int index) => colors[index % colors.Count];
        IEnumerable<(HsbColor, HsbColor)> colorPairs = fce.SecondaryColors.Count == 0
            ? [.. fce.PrimaryColors.Zip(fce.PrimaryColors)]
            : [.. fce.PrimaryColors.Zip(Enumerable.Range(0, fce.PrimaryColors.Count).Select((_, index) => WrapTable(fce.SecondaryColors, index)))];
        return [.. colorPairs.Select(p => new FceColor() { PrimaryColor = p.Item1, SecondaryColor = p.Item2 })];
    }

    //private void OnSave()
    //{
    //    saveCallback?.Invoke(serializer.Serialize(Data));
    //    if (LinkEdits) OnSyncChanges();
    //}

    //private void OnSyncChanges()
    //{
    //    if (viv is null || fedataName is null || Path.GetExtension(fedataName) is not { } ext) return;
    //    FeData3SyncTool.Sync(Data, ext, viv.Directory);
    //}
}