using System.Collections.Generic;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Models;

public class FceRenderState
{
    public IEnumerable<FcePart> VisibleParts { get; init; }
    public byte[]? Texture { get; init; }
    public FceColor? SelectedColor { get; init; }
}
