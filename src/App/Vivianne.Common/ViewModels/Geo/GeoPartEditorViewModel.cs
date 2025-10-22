using System.Linq;
using System.Threading.Tasks;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Geo;

/// <summary>
/// ViewModel that allows the user to edit a GEO part.
/// </summary>
/// <param name="state"></param>
public class GeoPartEditorViewModel(GeoPartEditorState state) : EditorViewModelBase<GeoPartEditorState>(state)
{
    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        State.Part.Unk_0x14 = State.Unk_0x14;
        State.Part.Unk_0x18 = State.Unk_0x18;
        State.Part.Unk_0x1C = State.Unk_0x1C;
        State.Part.Unk_0x24 = State.Unk_0x24;
        State.Part.Unk_0x2C = State.Unk_0x2C;
        foreach (var (first, second) in State.Part.Faces.Zip(State.Faces))
        {
            if (State.TextureSource is not null)
            {
                first.TextureName = State.TextureSource.Entries.First(p => p.Value == second.SelectedTexture).Key;
            }
            first.MaterialFlags = second.Flags;
        }

        return Task.CompletedTask;
    }
}