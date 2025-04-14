using System.Collections.Generic;
using TheXDS.Vivianne.Models.Carp.Nfs4;
using TheXDS.Vivianne.Models.Fe.Nfs4;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Tools.Fe;

namespace TheXDS.Vivianne.ViewModels.Carp.Nfs4;

/// <summary>
/// Implements a ViewModel for NFS4 Carp files.
/// </summary>
public class CarpEditorViewModel : CarpEditorViewModel<CarpEditorState, CarPerf, CarClass>
{     /// <inheritdoc/>
    protected override bool BeforeSave()
    {
        if (Settings.Current.Carp_SyncChanges)
        {
            FeData4SyncTool.Sync(State.File, BackingStore?.Store.AsDictionary() ?? new Dictionary<string, byte[]>());
        }
        return base.BeforeSave();
    }
}
