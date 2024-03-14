﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using MC = TheXDS.MCART.Types.Color;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows a user to preview a FSH texture file.
/// </summary>
/// <remarks>
/// QFS files can be decompressed and shown as FSH files with this ViewModel.
/// </remarks>
public class FshEditorViewModel : ViewModel, IViewModel
{
    private readonly FshFile _Fsh;
    private readonly Action<FshFile>? saveCallback;
    private BackgroundType _Background;
    private FshBlob? _CurrentImage;
    private bool _UnsavedChanges;
    private double _ZoomLevel = 1.0;
    private MC[]? _palette;

    /// <summary>
    /// Initializes a new instance of the <see cref="FshEditorViewModel"/>
    /// class.
    /// </summary>
    /// <param name="fsh">FSH file to preview.</param>
    /// <param name="saveCallback">Save callback to invoke when persisting changes. If set to <see langword="null"/>, this ViewModel will function in read-only mode.</param>
    public FshEditorViewModel(FshFile fsh, Action<FshFile>? saveCallback = null)
    {
        var cb = CommandBuilder.For(this);
        AddNewCommand = cb.BuildSimple(OnAddNew);
        ExportCommand = cb.BuildObserving(OnExport).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        ReplaceImageCommand = cb.BuildObserving(OnReplaceImage).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RemoveCurrentCommand = cb.BuildObserving(OnRemoveCurrent).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RenameCurrentCommand = cb.BuildObserving(OnRenameCurrent).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        SaveChangesCommand = cb.BuildObserving(OnSaveChanges).ListensToCanExecute(vm => vm.UnsavedChanges).Build();
        _Fsh = fsh;
        this.saveCallback = saveCallback;
        Images = new ObservableDictionaryWrap<string, FshBlob>(_Fsh.Entries);
        CurrentImage = _Fsh.Entries.Values.FirstOrDefault();
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
    public FshBlob? CurrentImage
    {
        get => _CurrentImage;
        set => Change(ref _CurrentImage, value);
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
    /// Gets a value that indicates if this FSH file contains a car dashboard.
    /// </summary>
    public bool IsDash => Images.Count > 2 && Images.TryGetValue("0000", out FshBlob? dashGimx) && dashGimx.Footer.Length == 104;

    /// <summary>
    /// Gets a color palette from the FSH file if one exists, or sets a global
    /// palette to use when previewing GIMX textures with a
    /// <see cref="FshBlobFormat.Indexed8"/> pixel format.
    /// </summary>
    /// <remarks>
    /// Changing this value will not write to the FSH file. The palette will be
    /// used only for previewing textures. If you want to write a palette
    /// locally, add a new GIMX with a pixel format that corresponds to a valid
    /// palette.
    /// </remarks>
    public MC[]? Palette
    { 
        get => _palette ?? _Fsh.GetPalette();
        set => Change(ref _palette, value);
    }

    /// <summary>
    /// Gets a value that indicates if this ViewModel was created in read-only mode.
    /// </summary>
    public bool IsReadOnly => saveCallback is null;

    /// <summary>
    /// Gets the ID of the GIMX texture being displayed.
    /// </summary>
    public string CurrentGimxId => Images.FirstOrDefault(p => ReferenceEquals(p.Value, CurrentImage)).Key;

    /// <summary>
    /// Gets a dictionary with the contents of the FSH file.
    /// </summary>
    public IDictionary<string, FshBlob> Images { get; }

    /// <summary>
    /// Gets a reference to the command used to add a new GIMX to the FSH file.
    /// </summary>
    public ICommand AddNewCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to export the current image.
    /// </summary>
    public ICommand ExportCommand { get; }

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

    /// <summary>
    /// Gets a reference to the command used to replace the current image.
    /// </summary>
    public ICommand ReplaceImageCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to save all pending changes on the
    /// FSH file.
    /// </summary>
    public ICommand SaveChangesCommand { get; }

    private async Task OnAddNew()
    {
        var data = await GetNewGimxData();
        if (data is null) return;
        try
        {
            var newGimx = new FshBlob() { Magic = Mappings.FshBlobToLabel.Keys.ToArray()[data.Value.formatIndex] };
            newGimx.ReplaceWith(Image.FromFile(data.Value.file), Palette ?? new MC[256]);
            Images.Add(data.Value.id, newGimx);
            CurrentImage = newGimx;
            UnsavedChanges = true;
        }
        catch (Exception ex)
        {
            await DialogService!.Error(ex);
        }
    }

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
            UnsavedChanges = true;
        }
    }

    private async Task OnRenameCurrent()
    {
        var id = await DialogService!.GetInputText("GIMX ID", $"Enter the new ID to use for the '{CurrentGimxId}' GIMX texture", CurrentGimxId);
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
        UnsavedChanges = true;
    }

    private async Task OnReplaceImage()
    {
        var r = await DialogService!.GetFileOpenPath($"Replace '{CurrentGimxId}'", $"Select a file to replace '{CurrentGimxId}' with", FileFilters.CommonBitmapFormats);
        if (r.Success)
        {
            try
            {
                CurrentImage!.ReplaceWith(Image.FromFile(r.Result), Palette ?? new MC[256]);
                UnsavedChanges = true;
                Notify(nameof(CurrentImage));
            }
            catch (Exception ex)
            {
                await DialogService!.Error(ex);
            }
        }
    }

    private void OnSaveChanges()
    {
        saveCallback?.Invoke(_Fsh);
        UnsavedChanges = false;
    }

    private async Task<(string file, string id, int formatIndex)?> GetNewGimxData()
    {
        bool IsGimxInvalid(string? id, [NotNullWhen(true)]out string? errorMessage) => FshExtensions.IsNewGimxIdInvalid(id, _Fsh, out errorMessage);
        IInputItemDescriptor[] inputs =
        [
            new InputItemDescriptor<string>(d => d.GetFileOpenPath("Add texture", "Select a file to add as a new texture")!),
            new InputItemDescriptor<string>(d => d.GetInputText("Blob ID", "Enter the ID to use for the new texture", InferNewGimxName()), IsGimxInvalid),
            new InputItemDescriptor<int>(d => d.SelectOption("Blob pixel format", "Select a pixel format for the new texture", Mappings.FshBlobToLabel.Values.ToArray()))
        ];
        return await DialogService!.AskSequentially(inputs) is { } data ? ((string)data[0], (string)data[1], (int)data[2]) : null;
    }

    private string InferNewGimxName()
    {
        foreach (var i in Enumerable.Range(0, 10000))
        {
            var n = i.ToString("0000");
            if (!_Fsh.Entries.ContainsKey(n)) return n;
        }
        return string.Empty;
    }

    async Task IViewModel.OnNavigateBack(CancelFlag navigation)
    {
        if (UnsavedChanges)
        {
            switch (await DialogService!.AskYnc("Unsaved changes", $"Do you want to save {Title}?"))
            {
                case true: OnSaveChanges(); break;
                case null: navigation.Cancel(); break;
            }
        }
    }
}
