using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Common;
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

    /// <summary>
    /// Gets a reference to the command used to open a dialog to edit the FCE
    /// color tables.
    /// </summary>
    public ICommand ColorEditorCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Fce4EditorViewModel"/> class.
    /// </summary>
    public Fce4EditorViewModel()
    {
        var cb = CommandBuilder.For(this);
        ColorEditorCommand = cb.BuildSimple(OnColorEditor);
    }

    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        State.SelectedColor = State.Colors.FirstOrDefault();
        return base.OnCreated();
    }

    /// <inheritdoc/>
    protected override void OnSwitchToLod(FceLodPreset preset)
    {
        if (!LodPartMapping.TryGetValue(preset, out var mapping)) return;
        foreach (var j in State.Parts)
        {
            j.IsVisible = mapping.Contains(j.Part.Name);
        }
    }
    private async Task OnColorEditor()
    {
        var state = new Fce4ColorTableEditorState(State);
        var vm = new Fce4ColorEditorViewModel(state) { Title = "Color editor" };
        await DialogService!.Show(vm);
        await LoadColorNames();
        State.SelectedColor = State.Colors.FirstOrDefault();
        OnVisibleChanged();
    }
}
