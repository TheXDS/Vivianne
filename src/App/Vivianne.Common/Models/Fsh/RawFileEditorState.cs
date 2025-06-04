using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models.Fsh;

/// <summary>
/// Represents the state of an editor for raw files.
/// </summary>
public class RawFileEditorState : FileStateBase<RawFile>
{
    /// <summary>
    /// Gets or sets the raw data of the file being edited.
    /// </summary>
    public byte[] Data
    {
        get => File.Data;
        set => Change(f => f.Data, value);
    }
}