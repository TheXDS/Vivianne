using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Serializers.Fce.Nfs3;
using TheXDS.Vivianne.Tools.Fe;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Fe;

/// <summary>
/// Implements a ViewModel that allows the user to see and edit FeData car
/// information files for Need For Speed 3.
/// </summary>
public class FeData3EditorViewModel : StatefulFileEditorViewModelBase<FeData3EditorState, FeData>
{
    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        if (await (BackingStore?.Store.ReadAsync("car.fce") ?? Task.FromResult<byte[]?>(null)) is not byte[] fceBytes) return;
        FceSerializer serializer = new();
        if (serializer.TryGetFce(fceBytes) is { } fce)
        {
            State.PreviewFceColorTable = [.. ReadColors(fce).Concat(Enumerable.Range(0, 10).Select(_ => (FceColor?)null)).Take(10)];
        }
    }

    /// <inheritdoc/>
    protected override bool BeforeSave()
    {
        if (Settings.Current.Fe_SyncChanges)
        {
            FeData3SyncTool.Sync(State.File, Path.GetExtension(BackingStore!.FileName)!, BackingStore.Store.AsDictionary());
        }
        return base.BeforeSave();
    }

    private static FceColor?[] ReadColors(FceFile fce)
    {
        static HsbColor WrapTable(IList<HsbColor> colors, int index) => colors[index % colors.Count];
        IEnumerable<(HsbColor, HsbColor)> colorPairs = fce.SecondaryColors.Count == 0
            ? [.. fce.PrimaryColors.Zip(fce.PrimaryColors)]
            : [.. fce.PrimaryColors.Zip(Enumerable.Range(0, fce.PrimaryColors.Count).Select((_, index) => WrapTable(fce.SecondaryColors, index)))];
        return [.. colorPairs.Select(p => new FceColor() { PrimaryColor = p.Item1, SecondaryColor = p.Item2 })];
    }
}