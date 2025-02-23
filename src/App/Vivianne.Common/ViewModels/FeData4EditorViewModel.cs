using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fe.Nfs4;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels;

public class FeData4EditorViewModel : FileEditorViewModelBase<FeData4EditorState, FeData>
{
    /// <summary>
    /// Gets a table of the colors defined in the FCE file.
    /// </summary>
    public Fce4Color[]? PreviewFceColorTable { get; }
}
