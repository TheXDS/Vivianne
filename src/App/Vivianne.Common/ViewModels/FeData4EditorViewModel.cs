using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs4;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels;

public class FeData4EditorViewModel : FileEditorViewModelBase<FeData4EditorState, FeData>
{
    /// <summary>
    /// Gets a table of the colors defined in the FCE file.
    /// </summary>
    public Models.Fce.Nfs4.FceColor[]? PreviewFceColorTable { get; }
}
