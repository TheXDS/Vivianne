using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.ViewModels.Fe;

namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Represents the state of the <see cref="FeData3EditorViewModel"/>.
/// </summary>
public class FeData3EditorState : FileStateBase<FeData>
{
    /// <summary>
    /// Gets a table of the colors defined in the FCE file.
    /// </summary>
    public FceColor?[]? PreviewFceColorTable { get; set; }
}
