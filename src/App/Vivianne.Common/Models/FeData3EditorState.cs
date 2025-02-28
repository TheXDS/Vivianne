using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fe.Nfs3;

namespace TheXDS.Vivianne.Models;

public class FeData3EditorState : FileStateBase<FeData>
{
    private bool _LinkEdits;

    /// <summary>
    /// Gets or sets a value that indicates that changes made on the FeData
    /// editor should be synced up on other FeData files and the Carp.txt file.
    /// </summary>
    public bool LinkEdits
    {
        get => _LinkEdits;
        set => Change(ref _LinkEdits, value);
    }

    /// <summary>
    /// Gets a table of the colors defined in the FCE file.
    /// </summary>
    public FceColor[]? PreviewFceColorTable { get; set; }
}
