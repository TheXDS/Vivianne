namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to preview a simple image, such
/// as TGA textures.
/// </summary>
/// <param name="rawFile">Raw file contents.</param>
public class TexturePreviewViewModel(byte[] rawFile) : RawContentViewModel(rawFile)
{
    private BackgroundType _Background;
    private bool _UnsavedChanges;
    private double _ZoomLevel = 1.0;

    /// <summary>
    /// Gets or sets the desired background to use when previewing textures.
    /// </summary>
    public BackgroundType Background
    {
        get => _Background;
        set => Change(ref _Background, value);
    }

    /// <summary>
    /// Gets a value that indicates if there's unsaved changes on the unferlying FSH file.
    /// </summary>
    public bool UnsavedChanges
    {
        get => _UnsavedChanges;
        private set => Change(ref _UnsavedChanges, value);
    }

    /// <summary>
    /// Gets or sets the desired zoom level.
    /// </summary>
    public double ZoomLevel
    {
        get => _ZoomLevel;
        set => Change(ref _ZoomLevel, value);
    }
}
