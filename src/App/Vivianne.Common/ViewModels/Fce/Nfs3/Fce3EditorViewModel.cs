using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Fce.Common;
using TheXDS.Vivianne.Extensions;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs3;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model for
/// NFS3.
/// </summary>
public class Fce3EditorViewModel : FceEditorViewModelBase<
    Fce3EditorState,
    FceFile,
    FceColor,
    HsbColor,
    FcePart,
    Fce3RenderStateBuilder>
{
    /// <summary>
    /// Gets a reference to the command used to open a dialog to edit the FCE
    /// color tables.
    /// </summary>
    public ICommand ColorEditorCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Fce3EditorViewModel"/> class.
    /// </summary>
    public Fce3EditorViewModel()
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
        var allParts = State.Parts.Select(p => p.Part).ToArray();
        var partsToShow = (preset switch
        {
            FceLodPreset.High => allParts.Take(5),
            FceLodPreset.Medium => allParts.SkipIfMore(5).Take(5),
            FceLodPreset.Low => allParts.SkipIfMore(10).Take(1),
            FceLodPreset.Tiny => allParts.SkipIfMore(11).Take(1),
            _ => []
        }).ToArray();

        foreach (var j in State.Parts)
        {
            j.IsVisible = partsToShow.Contains(j.Part);
        }
    }

    private async Task OnColorEditor()
    {
        var state = new Fce3ColorTableEditorState(State);
        var vm = new FceColorEditorViewModel(state);
        await DialogService!.Show(vm);
        await LoadColorNames();
        State.SelectedColor = State.Colors.FirstOrDefault();
        OnVisibleChanged();
    }
}
