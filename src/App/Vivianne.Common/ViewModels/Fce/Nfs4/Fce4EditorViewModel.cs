using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs4;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model for
/// NFS4 / MCO.
/// </summary>
public class Fce4EditorViewModel : FceEditorViewModelBase<
    FceEditorState,
    FceFile,
    FceColor,
    HsbColor,
    FcePart,
    FceTriangle,
    FceRenderState>
{
    protected override FceRenderState GetRenderTree(IEnumerable<FcePart> visibleParts)
    {
        return new()
        {
            VisibleParts = Parts.Where(p => p.IsVisible).Select(p => p.Part),
            SelectedColor = SelectedColor,
            Texture = SelectedCarTexture,
            FceFile = RenderShadow ? State.File : null,
            ViewDamaged = State.ShowDamagedModel,
        };
    }
}
