using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs4;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model for
/// NFS4 / MCO.
/// </summary>
public class Fce4EditorViewModel : FceEditorViewModelBase<
    Fce4EditorState,
    FceFile,
    FceColor,
    HsbColor,
    Fce4Part,
    Fce4RenderStateBuilder>
{
    private static readonly ReadOnlyDictionary<FceLodPreset, string[]> LodPartMapping = new Dictionary<FceLodPreset, string[]>()
    {
        { FceLodPreset.High, [":HB", ":HLFW", ":HRFW", ":HLMW", ":HRMW", ":HLRW", ":HRRW", ":OT", ":OS", ":OLB", ":ORB", ":OLM", ":ORM", ":OC", ":OH", ":OD", ":OND"] },
        { FceLodPreset.Medium, [":MB", ":MLFW", ":MRFW", ":MLMW", ":MRMW", ":MLRW", ":MRRW"] },
        { FceLodPreset.Low, [":LB"] },
        { FceLodPreset.Tiny, [":TB"] },
    }.AsReadOnly();

    /// <inheritdoc/>
    protected override void OnSwitchToLod(FceLodPreset preset)
    {
        if (!LodPartMapping.TryGetValue(preset, out var mapping)) return;
        foreach (var j in State.Parts)
        {
            j.IsVisible = mapping.Contains(j.Part.Name);
        }
    }
}
