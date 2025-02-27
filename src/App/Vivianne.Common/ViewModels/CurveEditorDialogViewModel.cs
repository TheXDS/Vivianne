using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ViewModels.Base;

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
        if (State.TargetCollection is IList<double> list)
        {
            foreach (var (index, element) in State.Collection.WithIndex())
            {
                list[index] = element;
            }
        }
        else
        {
            State.TargetCollection.Clear();
            State.TargetCollection.AddRange(State.Collection);
        }
        return Task.CompletedTask;
    }
}