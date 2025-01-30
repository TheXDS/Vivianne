
namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to preview a simple image, such
/// as TGA textures.
/// </summary>
/// <param name="rawFile">Raw file contents.</param>
public class TexturePreviewViewModel(byte[] rawFile) : RawContentViewModel(rawFile)
{
    private BackgroundType _background;
    private bool _unsavedChanges;
    private double _zoomLevel = 1.0;
    private bool _alpha = true;

    /// <summary>
    /// Gets or sets the desired background to use when previewing textures.
    /// </summary>
    public BackgroundType Background
    {
        get => _background;
        set => Change(ref _background, value);
    }

    /// <summary>
    /// Gets a value that indicates if there's unsaved changes on the underlying Texture file.
    /// </summary>
    public bool UnsavedChanges
    {
        get => _unsavedChanges;
        private set => Change(ref _unsavedChanges, value);
    }

    /// <summary>
    /// Gets or sets the desired zoom level.
    /// </summary>
    public double ZoomLevel
    {
        get => _zoomLevel;
        set => Change(ref _zoomLevel, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates if the alpha channel should be enabled.
    /// </summary>
    public bool Alpha
    { 
        get => _alpha;
        set => Change(ref _alpha, value);
    }

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        broadcastSetup.RegisterPropertyChangeTrigger(() => RawFile, () => Alpha);
    }
}
