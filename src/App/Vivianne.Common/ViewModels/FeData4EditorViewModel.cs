using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels;

public class FeData4EditorViewModel : FileEditorViewModelBase<FeData4EditorState, FeData4>
{
    /// <summary>
    /// Gets a table of the colors defined in the FCE file.
    /// </summary>
    public Fce4ColorItem[]? PreviewFceColorTable { get; }
}
