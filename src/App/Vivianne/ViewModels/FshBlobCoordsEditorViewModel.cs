using System.Threading.Tasks;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// ViewModel that allows the user to modify the FSH blob coordinates.
/// </summary>
/// <param name="state">State to associate with this ViewModel.</param>
public class FshBlobCoordsEditorViewModel(FshBlobCoordsState state) : EditorViewModelBase<FshBlobCoordsState>(state)
{
    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        State.Blob.XRotation = (ushort)State.XRotation;
        State.Blob.YRotation = (ushort)State.YRotation;
        State.Blob.XPosition = (ushort)State.XPosition;
        State.Blob.YPosition = (ushort)State.YPosition;
        return Task.CompletedTask;
    }
}