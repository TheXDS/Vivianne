using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows a user to preview a FSH texture file.
/// </summary>
/// <remarks>
/// QFS files can be decompressed and shown as FSH files with this ViewModel.
/// </remarks>
public class FshPreviewViewModel : ViewModel
{
    private readonly FshTexture _Fsh;
    private BackgroundType _Background;
    private Gimx? _CurrentImage;
    private double _ZoomLevel = 1.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="FshPreviewViewModel"/>
    /// class.
    /// </summary>
    /// <param name="fsh">FSH file to preview.</param>
    public FshPreviewViewModel(FshTexture fsh)
    {
        var cb = CommandBuilder.For(this);

        AddNewCommand = cb.BuildSimple(OnAddNew);
        ExportCommand = cb.BuildObserving(OnExport).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        ReplaceImageCommand = cb.BuildObserving(OnReplaceImage).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RemoveCurrentCommand = cb.BuildObserving(OnRemoveCurrent).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RenameCurrentCommand = cb.BuildObserving(OnRenameCurrent).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        _Fsh = fsh;
        Images = new ObservableDictionaryWrap<string, Gimx>(_Fsh.Images);
        CurrentImage = _Fsh.Images.Values.FirstOrDefault();
    }

    /// <summary>
    /// Gets or sets the desired background to use when previewing textures.
    /// </summary>
    public BackgroundType Background
    {
        get => _Background;
        set => Change(ref _Background, value);
    }

    /// <summary>
    /// Gets or sets a reference to the GIMX blob currently on display.
    /// </summary>
    public Gimx? CurrentImage
    {
        get => _CurrentImage;
        set => Change(ref _CurrentImage, value);
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
    /// Gets a value that indicates if this FSH file contains a car dashboard.
    /// </summary>
    public bool IsDash => Images.Count > 2 && Images.TryGetValue("0000", out Gimx? dashGimx) && dashGimx.Footer.Length == 104;
    
    /// <summary>
    /// Gets the ID of the GIMX texture being displayed.
    /// </summary>
    public string CurrentGimxId => Images.FirstOrDefault(p => ReferenceEquals(p.Value, CurrentImage)).Key;

    /// <summary>
    /// Gets a dictionary with the contents of the FSH file.
    /// </summary>
    public IDictionary<string, Gimx> Images { get; }

    /// <summary>
    /// Gets a reference to the command used to replace the current image.
    /// </summary>
    public ICommand ReplaceImageCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to export the current image.
    /// </summary>
    public ICommand ExportCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to add a new GIMX to the FSH file.
    /// </summary>
    public ICommand AddNewCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to remove the current GIMX texture
    /// from the FSH file.
    /// </summary>
    public ICommand RemoveCurrentCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to rename the current GIMX texture
    /// from the FSH file.
    /// </summary>
    public ICommand RenameCurrentCommand { get; }

    private async Task OnExport()
    {
        var r = await DialogService!.GetFileSavePath($"Save texture as", FileFilters.CommonBitmapFormats);
        if (!r.Success) return;
        CurrentImage!.ToImage().Save(r.Result, ImageFormat.Png);
    }

    private async Task OnRemoveCurrent()
    {
        if (Images.Count <= 1)
        {
            await DialogService!.Error("Cannot remove GIMX", "A FSH file must contain at least one GIMX texture.");
            return;
        }
        var key = CurrentGimxId;
        if (key is not null)
        {
            if (!await DialogService!.Ask($"Remove '{CurrentGimxId}'", $"Are you sure you want to remove '{key}' from the FSH?")) return;
            Images.Remove(key);
            CurrentImage = Images.First().Value;
        }
    }

    private async Task OnRenameCurrent()
    {
        var id = await DialogService!.GetInputText("GIMX ID", $"Enter the new ID to use for the '{CurrentGimxId}' GIMX texture");
        if (!id.Success || id.Result.IsEmpty()) return;
        if (FshExtensions.IsNewGimxIdInvalid(id.Result, _Fsh, out var errorMsg))
        {
            await DialogService.Error("Invalid GIMX ID", errorMsg);
            return;
        }
        var gimx = CurrentImage!;
        Images.Remove(CurrentGimxId);
        Images.Add(id.Result, gimx);
        CurrentImage = gimx;
    }

    private async Task OnAddNew()
    {
        var id = await DialogService!.GetInputText("GIMX ID", "Enter the ID to use for the new GIMX texture");        
        if (!id.Success || id.Result.IsEmpty()) return;
        if (FshExtensions.IsNewGimxIdInvalid(id.Result, _Fsh, out var errorMsg))
        {
            await DialogService.Error("Invalid GIMX ID", errorMsg);
            return;
        }
        var formatIndex = await DialogService.SelectOption("GIMX pixel formatIndex", "Select a pixel formatIndex for the new GIMX texture", Mappings.GimxToLabel.Values.ToArray());
        if (formatIndex < 0) return;
        var r = await DialogService!.GetFileOpenPath($"Add '{id.Result}'", $"Select a file to add as '{id.Result}'", FileFilters.CommonBitmapFormats);
        if (r.Success)
        {
            try
            {
                var newGimx = new Gimx() { Magic = Mappings.GimxToLabel.Keys.ToArray()[formatIndex] };
                newGimx.ReplaceWith(Image.FromFile(r.Result), _Fsh);
                Images.Add(id.Result, newGimx);
                CurrentImage = newGimx;
            }
            catch (Exception ex)
            {
                await DialogService!.Error(ex);
            }
        }
    }

    private async Task OnReplaceImage()
    {
        var r = await DialogService!.GetFileOpenPath($"Replace '{CurrentGimxId}'", $"Select a file to replace '{CurrentGimxId}' with", FileFilters.CommonBitmapFormats);
        if (r.Success)
        {
            try
            {
                CurrentImage!.ReplaceWith(Image.FromFile(r.Result), _Fsh);
                Notify(nameof(CurrentImage));
            }
            catch (Exception ex)
            {
                await DialogService!.Error(ex);
            }
        }
    }
}
