using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.FshEditorViewModel;

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
    private readonly Func<FshFile, Task>? saveCallback;
    private BackgroundType _Background;
    private FshBlob? _CurrentImage;
    private bool _UnsavedChanges;
    private double _ZoomLevel = 1.0;
    private Color[]? _palette;

    /// <summary>
    /// Initializes a new instance of the <see cref="FshEditorViewModel"/>
    /// class.
    /// </summary>
    /// <param name="fsh">FSH file to preview.</param>
    /// <param name="saveCallback">
    /// Save callback to invoke when persisting changes. If set to
    /// <see langword="null"/>, this ViewModel will function in read-only mode.
    /// </param>
    public FshEditorViewModel(FshFile fsh, Func<FshFile, Task>? saveCallback = null)
    {
        var cb = CommandBuilder.For(this);
        AddNewCommand = cb.BuildSimple(OnAddNew);
        ExportCommand = cb.BuildObserving(OnExport).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        ReplaceImageCommand = cb.BuildObserving(OnReplaceImage).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RemoveCurrentCommand = cb.BuildObserving(OnRemoveCurrent).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RenameCurrentCommand = cb.BuildObserving(OnRenameCurrent).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        SaveChangesCommand = cb.BuildObserving(OnSaveChanges).ListensToCanExecute(vm => vm.UnsavedChanges).Build();
        ExportBlobFooterCommand = cb.BuildObserving(OnExportBlobFooter).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        ImportBlobFooterCommand = cb.BuildObserving(OnImportBlobFooter).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RemoveCurrentFooterCommand = cb.BuildObserving(OnRemoveBlobFooter).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        DashEditorCommand = cb.BuildObserving(OnDashEditor).ListensToCanExecute(p => p.IsDash).Build();
        CoordsEditorCommand = cb.BuildObserving(OnCoordsEditor).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        _Fsh = fsh;
        this.saveCallback = saveCallback;
        Images = new ObservableDictionaryWrap<string, FshBlob>(_Fsh.Entries);
        CurrentImage = _Fsh.Entries.Values.FirstOrDefault();
        RegisterPropertyChangeBroadcast(nameof(CurrentImage), nameof(Palette), nameof(CurrentFshBlobId));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FshEditorViewModel"/>
    /// class.
    /// </summary>
    /// <param name="fsh">FSH file to preview.</param>
    /// <param name="saveCallback">
    /// Save callback to invoke when persisting changes. If set to
    /// <see langword="null"/>, this ViewModel will function in read-only mode.
    /// </param>
    public FshEditorViewModel(FshFile fsh, Action<FshFile>? saveCallback = null) : this(fsh, async (p) => await (saveCallback is not null ? Task.Run(() => saveCallback.Invoke(p)) : Task.CompletedTask))
    { }

    /// <summary>
    /// Gets or sets the desired background to use when previewing textures.
    /// </summary>
    public BackgroundType Background
    {
        get => _Background;
        set => Change(ref _Background, value);
    }

    /// <summary>
    /// Gets or sets a reference to the FSH blob currently on display.
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
    public bool IsDash => Images.Count >= 2 && Images.TryGetValue("0000", out FshBlob? dashGimx) && dashGimx.GaugeData is not null;

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
    public Color[]? Palette
    {
        get => _palette ?? CurrentImage?.LocalPalette ?? _Fsh.GetPalette();
        set => Change(ref _palette, value);
    }

    /// <summary>
    /// Gets a value that indicates if this ViewModel was created in read-only mode.
    /// </summary>
    public bool IsReadOnly => saveCallback is null;

    /// <summary>
    /// Gets the ID of the GIMX texture being displayed.
    /// </summary>
    public string CurrentFshBlobId => Images.FirstOrDefault(p => ReferenceEquals(p.Value, CurrentImage)).Key;

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
    /// Gets a reference to the command used to export the current image's
    /// footer data.
    /// </summary>
    public ICommand ExportBlobFooterCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to import the current image's
    /// footer data.
    /// </summary>
    public ICommand ImportBlobFooterCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to remove the current image's
    /// footer data.
    /// </summary>
    public ICommand RemoveCurrentFooterCommand { get; }

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

    /// <summary>
    /// Gets a reference to the command used to open the dash editor dialog.
    /// </summary>
    public ICommand DashEditorCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to open a dialog to edit the FSH
    /// blob coordinates.
    /// </summary>
    public ICommand CoordsEditorCommand { get; }

    private async Task OnAddNew()
    {
        var data = await GetNewFshBlobData();
        if (data is null) return;
        try
        {
            var newBlob = new FshBlob() { Magic = Mappings.FshBlobToLabel.Keys.ToArray()[data.Value.formatIndex] };
            var img = Image.Load(data.Value.file);
            newBlob.ReplaceWith(img, Palette ?? new Color[256]);
            Images.Add(data.Value.id, newBlob);
            CurrentImage = newBlob;
            UnsavedChanges = true;
        }
        catch (Exception ex)
        {
            await DialogService!.Error(ex);
        }
    }

    private async Task OnExport()
    {
        var r = await DialogService!.GetFileSavePath(St.SaveTextureAs, FileFilters.CommonBitmapSaveFormats);
        if (!r.Success) return;
        CurrentImage!.ToImage(Palette)!.Save(r.Result, Mappings.ExportEnconder[Path.GetExtension(r.Result)]);
    }

    private async Task OnExportBlobFooter()
    {
        var r = await DialogService!.GetFileSavePath(St.SaveBlobFooterAs);
        if (!r.Success) return;
        System.IO.File.WriteAllBytes(r.Result, CurrentImage!.Footer);
    }

    private async Task OnImportBlobFooter()
    {
        if (CurrentImage?.Footer is not null && CurrentImage?.Footer.Length > 0)
        {
            switch (await DialogService!.AskYnc(St.ExportFooterConfirm))
            {
                case true: await OnExportBlobFooter(); break;
                case null: return;
            }
        }
        var r = await DialogService!.GetFileOpenPath(St.ImportBlobFooter);
        if (!r.Success) return;
        CurrentImage!.Footer = File.ReadAllBytes(r.Result);
        UnsavedChanges = true;
    }

    private async Task OnRemoveBlobFooter()
    {
        if (CurrentImage?.Footer is not null && CurrentImage?.Footer.Length > 0)
        {
            switch (await DialogService!.AskYnc(St.ExportFooterConfirm))
            {
                case true: await OnExportBlobFooter(); break;
                case null: return;
            }
        }
        CurrentImage!.Footer = [];
        UnsavedChanges = true;
    }

    private async Task OnRemoveCurrent()
    {
        if (Images.Count <= 1)
        {
            await DialogService!.Error(St.RemoveLastFshError, St.RemoveLastFshDetails);
            return;
        }
        var key = CurrentFshBlobId;
        if (key is not null)
        {
            if (!await DialogService!.Ask(string.Format(St.RemoveX, key), string.Format(St.RemoveXDetails, key))) return;
            Images.Remove(key);
            CurrentImage = Images.First().Value;
            UnsavedChanges = true;
        }
    }

    private async Task OnRenameCurrent()
    {
        var id = await DialogService!.GetInputText(St.FshId, string.Format(St.FshXNewId, CurrentFshBlobId), CurrentFshBlobId);
        if (!id.Success || id.Result.IsEmpty()) return;
        if (FshExtensions.IsNewGimxIdInvalid(id.Result, _Fsh, out var errorMsg))
        {
            await DialogService.Error(St.InvalidFshId, errorMsg);
            return;
        }
        var gimx = CurrentImage!;
        Images.Remove(CurrentFshBlobId);
        Images.Add(id.Result, gimx);
        CurrentImage = gimx;
        UnsavedChanges = true;
    }

    private async Task OnReplaceImage()
    {
        var r = await DialogService!.GetFileOpenPath(string.Format(St.ReplaceFsh, CurrentFshBlobId), string.Format(St.ReplaceFshPrompt, CurrentFshBlobId), FileFilters.CommonBitmapOpenFormats);
        if (r.Success)
        {
            try
            {
                var img = Image.Load(File.OpenRead(r.Result));
                CurrentImage!.ReplaceWith(img, _Fsh.GetPalette());
                UnsavedChanges = true;
                Notify(nameof(CurrentImage));
            }
            catch (Exception ex)
            {
                await DialogService!.Error(ex);
            }
        }
    }

    private async Task OnSaveChanges()
    {
        await (saveCallback?.Invoke(_Fsh) ?? Task.CompletedTask);
    }

    private async Task OnDashEditor()
    {
        var state = new GaugeDataState(_Fsh);
        var vm = new DashEditorViewModel(state) { Title = St.DashEditor };
        vm.StateSaved += Vm_StateSaved;
        await DialogService!.CustomDialog(vm);
        vm.StateSaved -= Vm_StateSaved;
    }

    private void Vm_StateSaved(object? sender, EventArgs e)
    {
        UnsavedChanges = true;
    }

    private async Task OnCoordsEditor()
    {
        var state = new FshBlobCoordsState(CurrentImage!);
        var vm = new FshBlobCoordsEditorViewModel(state) { Title = St.CoordsEditor };
        vm.StateSaved += Vm_StateSaved;
        await DialogService!.CustomDialog(vm);
        vm.StateSaved -= Vm_StateSaved;
    }

    private async Task<(string file, string id, int formatIndex)?> GetNewFshBlobData()
    {
        bool IsGimxInvalid(string? id, [NotNullWhen(true)] out string? errorMessage) => FshExtensions.IsNewGimxIdInvalid(id, _Fsh, out errorMessage);
        IInputItemDescriptor[] inputs =
        [
            new InputItemDescriptor<string>(d => d.GetFileOpenPath(St.AddTexture, St.AddTexturePrompt, FileFilters.CommonBitmapOpenFormats)!),
            new InputItemDescriptor<string>(d => d.GetInputText(St.FshId, St.FshNewId, InferNewFshBlobName()), IsGimxInvalid),
            new InputItemDescriptor<int>(d => d.SelectOption(St.FshPixelFormat, St.FshPixelFormatPrompt, Mappings.FshBlobToLabel.Values.ToArray()))
        ];
        return (await DialogService!.AskSequentially(inputs) is { } data && (int)data[2] != -1) ? ((string)data[0], (string)data[1], (int)data[2]) : null;
    }

    private string InferNewFshBlobName()
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
            switch (await DialogService!.AskYnc(St.Unsaved, string.Format(St.SaveConfirm, Title)))
            {
                case true: await OnSaveChanges(); break;
                case null: navigation.Cancel(); break;
            }
        }
    }
}
