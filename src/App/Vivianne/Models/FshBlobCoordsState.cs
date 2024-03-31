namespace TheXDS.Vivianne.Models;

public class FshBlobCoordsState : EditorViewModelStateBase
{
    private int _XRotation;
    private int _YRotation;
    private int _XPosition;
    private int _YPosition;

    public FshBlob Blob { get; }

    public FshBlobCoordsState(FshBlob blob)
    {
        Blob = blob;
        _XRotation = blob.XRotation;
        _YRotation = blob.YRotation;
        _XPosition = blob.XPosition;
        _YPosition = blob.YPosition;
    }

    public int XRotation
    {
        get => _XRotation;
        set => Change(ref _XRotation, value);
    }

    public int YRotation
    {
        get => _YRotation;
        set => Change(ref _YRotation, value);
    }

    public int XPosition
    {
        get => _XPosition;
        set => Change(ref _XPosition, value);
    }

    public int YPosition
    {
        get => _YPosition;
        set => Change(ref _YPosition, value);
    }
}
