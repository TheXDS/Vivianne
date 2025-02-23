using System.Threading.Tasks;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fsh.Nfs3;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fsh.Blobs;

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
        State.Cabin.Footer = ((ISerializer<GaugeData>)new GaugeDataSerializer()).Serialize(State.BackingStore);
        if (State.Steering is not null)
        {
            State.Steering.XPosition = (ushort)State.SteeringXPosition;
            State.Steering.YPosition = (ushort)State.SteeringYPosition;
            State.Steering.XRotation = (ushort)State.SteeringXRotation;
            State.Steering.YRotation = (ushort)State.SteeringYRotation;
        }
        foreach (var j in State.Gears)
        {
            j.XPosition = (ushort)State.GearXPosition;
            j.YPosition = (ushort)State.GearYPosition;
        }
        return Task.CompletedTask;
    }
}
