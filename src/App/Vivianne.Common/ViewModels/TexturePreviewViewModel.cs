using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.ViewModels.Base;
using SixLabors.ImageSharp.Formats.Tga;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to preview a simple image, such
/// as TGA textures.
/// </summary>
public class TexturePreviewViewModel : StatefulFileEditorViewModelBase<RawFileEditorState, RawFile>
{
    private BackgroundType _background;
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

    /// <summary>
    /// Gets the command that replaces the current texture with a new one.
    /// </summary>
    public ICommand ReplaceTextureCommand { get; }

    /// <summary>
    /// Gets the command that triggers the export operation.
    /// </summary>
    public ICommand ExportTextureCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TexturePreviewViewModel"/> class.
    /// </summary>
    public TexturePreviewViewModel()
    {
        var cb = CommandBuilder.For(this);
        ReplaceTextureCommand = cb.BuildSimple(OnReplaceTexture);
        ExportTextureCommand = cb.BuildSimple(OnExportTexture);
    }

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        broadcastSetup.RegisterPropertyChangeTrigger(() => State, () => Alpha);
    }

    private async Task OnReplaceTexture()
    {
        if (await DialogService!.GetFileOpenPath(FileFilters.CommonBitmapOpenFormats) is { Success: true, Result: { } textureFile })
        {
            try
            {
                IsBusy = true;
                var img = await Image.LoadAsync(File.OpenRead(textureFile));
                using var ms = new MemoryStream();
                await img.SaveAsTgaAsync(ms, new TgaEncoder() { BitsPerPixel = TgaBitsPerPixel.Pixel32, Compression = TgaCompression.None });
                State.Data = ms.ToArray();
            }
            catch (Exception ex)
            {
                await DialogService!.Error(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    private async Task OnExportTexture()
    {
        try
        {
            if (await DialogService!.GetFileSavePath(CommonDialogTemplates.FileSave with { Title = "St.SaveTextureAs" }, FileFilters.CommonBitmapSaveFormats) is { Success: true, Result: { } file })
            {
                await (await Image.LoadAsync(new MemoryStream(State.Data))).SaveAsync(file);
            }
        }
        catch (Exception ex)
        {
            await DialogService!.Error(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
