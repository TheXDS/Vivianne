using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel for a dialog that allows the user to edit a
/// collection of <see cref="double"/> values.
/// </summary>
/// <param name="state">Current state of the dialog.</param>
public class CurveEditorDialogViewModel(CurveEditorState state) : EditorViewModelBase<CurveEditorState>(state)
{
    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        State.TargetCollection.Clear();
        State.TargetCollection.AddRange(State.Collection);
        return Task.CompletedTask;
    }
}