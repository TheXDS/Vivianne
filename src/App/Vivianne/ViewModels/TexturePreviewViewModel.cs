using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Vivianne.Resources;

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

    /// <summary>
    /// Gets a reference to the command used to export the current image.
    /// </summary>
    public ICommand ExportCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to replace the current image.
    /// </summary>
    public ICommand ReplaceImageCommand { get; }

    //private async Task OnExport()
    //{
    //    var r = await DialogService!.GetFileSavePath($"Save texture as", FileFilters.CommonBitmapSaveFormats);
    //    if (!r.Success) return;

    //    using var ms = new MemoryStream(value);
    //    FIBITMAP dib = FreeImage.LoadFromStream(ms);
    //    var bitmap = FreeImage.GetBitmap(dib)
    //    CurrentImage!.ToImage().Save(r.Result, ImageFormat.Png);
    //}
}
