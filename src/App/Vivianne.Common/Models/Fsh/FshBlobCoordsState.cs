using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.Models.Fsh;

/// <summary>
/// Represents the state of the <see cref="FshBlobCoordsEditorViewModel"/>.
/// </summary>
/// <param name="blob">Blob to be edited.</param>
public class FshBlobCoordsState(FshBlob blob) : EditorViewModelStateBase
{
    private int _XRotation = blob.XRotation;
    private int _YRotation = blob.YRotation;
    private int _XPosition = blob.XPosition;
    private int _YPosition = blob.YPosition;

    /// <summary>
    /// Gets a reference to the Blob being edited.
    /// </summary>
    public FshBlob Blob { get; } = blob;

    /// <summary>
    /// Gets or sets the X coord used for rotation operations.
    /// </summary>
    public int XRotation
    {
        get => _XRotation;
        set => Change(ref _XRotation, value);
    }

    /// <summary>
    /// Gets or sets the Y coord used for rotation operations.
    /// </summary>
    public int YRotation
    {
        get => _YRotation;
        set => Change(ref _YRotation, value);
    }

    /// <summary>
    /// Gets or sets the X coord used for traslation operations.
    /// </summary>
    public int XPosition
    {
        get => _XPosition;
        set => Change(ref _XPosition, value);
    }

    /// <summary>
    /// Gets or sets the Y coord used for traslation operations.
    /// </summary>
    public int YPosition
    {
        get => _YPosition;
        set => Change(ref _YPosition, value);
    }
}
