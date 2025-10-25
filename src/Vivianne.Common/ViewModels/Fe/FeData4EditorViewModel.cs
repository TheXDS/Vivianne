using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs4;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fce.Nfs4;
using TheXDS.Vivianne.Tools.Fe;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Fe;

/// <summary>
/// Implements a ViewModel that allows the user to edit FeData files for NFS4.
/// </summary>
public class FeData4EditorViewModel : StatefulFileEditorViewModelBase<FeData4EditorState, FeData>
{
    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        if (await (BackingStore?.Store.ReadAsync("car.fce") ?? Task.FromResult<byte[]?>(null)) is not byte[] fceBytes) return;
        if (VersionIdentifier.FceVersion(fceBytes) == NfsVersion.Nfs4)
        {
            var fce = ((ISerializer<FceFile>)new FceSerializer()).Deserialize(fceBytes);
            State.PreviewFceColorTable = [.. ReadColors(fce).Concat(Enumerable.Range(0, 10).Select(_ => (FceColor?)null)).Take(10)];
        }
    }

    /// <inheritdoc/>
    protected override bool BeforeSave()
    {
        State.File.DefaultCompare = State.DefaultCompare.ToCompare();
        State.File.CompareUpg1 = State.CompareUpg1.ToCompare();
        State.File.CompareUpg2 = State.CompareUpg2.ToCompare();
        State.File.CompareUpg3 = State.CompareUpg3.ToCompare();

        if (Settings.Current.Fe_SyncChanges)
        {
            FeData4SyncTool.Sync(State.File, Path.GetExtension(BackingStore!.FileName)!, BackingStore.Store.AsDictionary());
        }
        return base.BeforeSave();
    }

    private static FceColor?[] ReadColors(FceFile fce)
    {
        return [.. fce.PrimaryColors.Zip(fce.InteriorColors, fce.SecondaryColors).Zip(fce.DriverHairColors).Select(p => new FceColor() {
            PrimaryColor = p.First.First,
            InteriorColor = p.First.Second,
            SecondaryColor = p.First.Third,
            DriverHairColor = p.Second
        })];
    }
}
