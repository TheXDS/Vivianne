using System.Collections.Generic;
using TheXDS.Vivianne.Models.Carp.Nfs3;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Tools.Fe;

namespace TheXDS.Vivianne.ViewModels.Carp.Nfs3;

/// <summary>
/// Implements a ViewModel for NFS3 Carp files.
/// </summary>
public class CarpEditorViewModel : CarpEditorViewModel<CarpEditorState, CarPerf, CarClass>
{
    /// <inheritdoc/>
    protected override bool BeforeSave()
    {
        if (Settings.Current.Carp_SyncChanges)
        {
            FeData3SyncTool.Sync(State.File, BackingStore?.Store.AsDictionary() ?? new Dictionary<string, byte[]>());
        }
        return base.BeforeSave();
    }
}
