using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

public class CurveEditorDialogViewModel(CurveEditorState state) : EditorViewModelBase<CurveEditorState>(state)
{
    protected override Task OnSaveChanges()
    {
        State.TargetCollection.Clear();
        State.TargetCollection.AddRange(State.Collection);
        return Task.CompletedTask;
    }
}
