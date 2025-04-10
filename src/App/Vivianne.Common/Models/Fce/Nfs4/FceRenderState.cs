using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents the render tree state for an FCE editor.
/// </summary>
public class FceRenderState : FceRenderStateBase<FcePart, FceTriangle, FceColor, HsbColor, FceFile>
{
    /// <summary>
    /// Gets a value that indicates if the user selects the damaged version of
    /// the FCE model.
    /// </summary>
    public bool ViewDamaged { get; init; }
}