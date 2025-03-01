using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fe.Nfs4;

namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Represents the current state of the FeData4 editor ViewModel.
/// </summary>
public class FeData4EditorState : FileStateBase<FeData>
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
}
