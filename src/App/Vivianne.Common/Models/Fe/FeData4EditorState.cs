using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.Models.Fe.Nfs4;

namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Represents the current state of the FeData4 editor ViewModel.
/// </summary>
public class FeData4EditorState : FileStateBase<FeData>
{
    /// <summary>
    /// Gets a table of the colors defined in the FCE file.
    /// </summary>
    public FceColor?[]? PreviewFceColorTable { get; set; }
}
