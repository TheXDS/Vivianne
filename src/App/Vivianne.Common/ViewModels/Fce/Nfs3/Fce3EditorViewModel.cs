using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs3;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model for
/// NFS3.
/// </summary>
public class Fce3EditorViewModel : FceEditorViewModelBase<
    FceEditorState,
    FceFile,
    FceColor,
    HsbColor,
    FcePart,
    FceTriangle,
    FceRenderState>;
