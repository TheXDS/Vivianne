using System.Threading.Tasks;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// ViewModel that allows the user to modify Gauge data information.
/// </summary>
/// <param name="state">State to associate with this ViewModel.</param>
public class DashEditorViewModel(GaugeDataState state) : EditorViewModelBase<GaugeDataState>(state)
{
    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        State.Blob.GaugeData = State.BackingStore;
        return Task.CompletedTask;
    }
}
