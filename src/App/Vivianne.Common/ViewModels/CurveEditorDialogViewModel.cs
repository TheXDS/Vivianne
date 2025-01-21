using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

public class CurveEditorDialogViewModel(CurveEditorState state) : EditorViewModelBase<CurveEditorState>(state)
{
    public CurveEditorDialogViewModel() : this(new(new ObservableCollection<double>([1, 2, 3, 4, 5])))
    {
        
    }



    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        State.TargetCollection.Clear();
        State.TargetCollection.AddRange(State.Collection);
        return Task.CompletedTask;
    }
}