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
        State.Cabin.GaugeData = State.BackingStore;
        if (State.Steering is not null)
        {
            State.Steering.XPosition = (ushort)State.SteeringXPosition;
            State.Steering.YPosition = (ushort)State.SteeringYPosition;
            State.Steering.XRotation = (ushort)State.SteeringXRotation;
            State.Steering.YRotation = (ushort)State.SteeringYRotation;
        }
        return Task.CompletedTask;
    }
}
