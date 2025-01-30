using SixLabors.ImageSharp;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.FshEditorViewModel;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows a user to preview a FSH texture file.
/// </summary>
/// <remarks>
/// QFS files can be decompressed and shown as FSH files with this ViewModel.
/// </remarks>
public class FshEditorViewModel : ViewModel, IViewModel, IFileEditorViewModel<FshEditorState, FshFile>
{
    private BackgroundType _Background;
    private FshBlob? _CurrentImage;
    private double _ZoomLevel = 1.0;
    private bool _alpha = true;
    private Color[]? _palette;
    private ICommand saveCommand = null!;
    private ICommand? saveAsCommand = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="FshEditorViewModel"/>
    /// class.
    /// </summary>
    public FshEditorViewModel()
    {
        var cb = CommandBuilder.For(this);
        AddNewCommand = cb.BuildSimple(OnAddNew);
        ExportCommand = cb.BuildObserving(OnExport).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        ReplaceImageCommand = cb.BuildObserving(OnReplaceImage).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RemoveCurrentCommand = cb.BuildObserving(OnRemoveCurrent).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RenameCurrentCommand = cb.BuildObserving(OnRenameCurrent).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        ExportBlobFooterCommand = cb.BuildObserving(OnExportBlobFooter).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        ImportBlobFooterCommand = cb.BuildObserving(OnImportBlobFooter).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        RemoveCurrentFooterCommand = cb.BuildObserving(OnRemoveBlobFooter).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        DashEditorCommand = cb.BuildObserving(OnDashEditor).ListensToCanExecute(p => p.IsDash).Build();
        CoordsEditorCommand = cb.BuildObserving(OnCoordsEditor).CanExecuteIfNotNull(p => p.CurrentImage).Build();
        CloseCommand = cb.BuildSimple(OnClose);
    }

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        broadcastSetup.RegisterPropertyChangeBroadcast(() => CurrentImage, () => Palette, () => CurrentFshBlobId);
        broadcastSetup.RegisterPropertyChangeTrigger(() => CurrentImage, () => Alpha);
    }

    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        CurrentImage = State.Entries.Values.FirstOrDefault();
        return base.OnCreated();
    }

    /// <inheritdoc/>
    public FshEditorState State { get; set; } = null!;

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
    /// Gets or sets the desired zoom level.
    /// </summary>
    public double ZoomLevel
    {
        get => _ZoomLevel;
        set => Change(ref _ZoomLevel, value);
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
        get => _palette ?? CurrentImage?.LocalPalette ?? State.File.GetPalette();
        set => Change(ref _palette, value);
    }

    /// <summary>
    /// Gets a value that indicates if this FSH file contains a car dashboard.
    /// </summary>
    public bool IsDash => State.Entries.Count >= 2 && State.Entries.TryGetValue("0000", out FshBlob? dashGimx) && dashGimx.GaugeData is not null;

    /// <summary>
    /// Gets the ID of the GIMX texture being displayed.
    /// </summary>
    public string CurrentFshBlobId => State.Entries.FirstOrDefault(p => ReferenceEquals(p.Value, CurrentImage)).Key;

    /// <inheritdoc/>
    public ICommand SaveCommand
    {
        get => saveCommand;
        set => Change(ref saveCommand, value);
    }

    /// <inheritdoc/>
    public ICommand? SaveAsCommand
    {
        get => saveAsCommand;
        set => Change(ref saveAsCommand, value);
    }

    /// <inheritdoc/>
    public ICommand CloseCommand { get; }

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
            State.Entries.Add(data.Value.id, newBlob);
            CurrentImage = newBlob;
        }
        catch (Exception ex)
        {
            await DialogService!.Error(ex);
        }
    }

    private async Task OnExport()
    {
        var r = await DialogService!.GetFileSavePath(CommonDialogTemplates.FileSave with { Title = St.SaveTextureAs }, FileFilters.CommonBitmapSaveFormats);
        if (!r.Success) return;
        CurrentImage!.ToImage(Palette)!.Save(r.Result, Mappings.ExportEnconder[Path.GetExtension(r.Result)]);
    }

    private Task OnClose()
    {
        return NavigationService?.NavigateBack() ?? Task.CompletedTask;
    }

    private async Task OnExportBlobFooter()
    {
        var r = await DialogService!.GetFileSavePath(CommonDialogTemplates.FileSave with { Title = St.SaveBlobFooterAs }, [FileFilterItem.AllFiles]);
        if (!r.Success) return;
        File.WriteAllBytes(r.Result, CurrentImage!.Footer);
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
        var r = await DialogService!.GetFileOpenPath(CommonDialogTemplates.FileOpen with { Title = St.ImportBlobFooter }, [FileFilterItem.AllFiles]);
        if (!r.Success) return;
        CurrentImage!.Footer = File.ReadAllBytes(r.Result);
        State.Entries.RefreshItem(CurrentImage);
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
        State.Entries.RefreshItem(CurrentImage);
    }

    private async Task OnRemoveCurrent()
    {
        var key = CurrentFshBlobId;
        if (key is not null)
        {
            if (!await DialogService!.AskYn(string.Format(St.RemoveX, key), string.Format(St.RemoveXDetails, key))) return;
            if (State.Entries.Remove(key)) CurrentImage = State.Entries.Values.FirstOrDefault();
        }
    }

    private async Task OnRenameCurrent()
    {
        var id = await DialogService!.GetInputText(CommonDialogTemplates.Input with { Title = St.FshId, Text = string.Format(St.FshXNewId, CurrentFshBlobId) }, CurrentFshBlobId);
        if (!id.Success || id.Result.IsEmpty()) return;
        if (FshExtensions.IsNewGimxIdInvalid(id.Result, State.File, out var errorMsg))
        {
            await DialogService.Error(St.InvalidFshId, errorMsg);
            return;
        }
        var blob = CurrentImage!;
        State.Entries.Remove(CurrentFshBlobId);
        State.Entries.Add(id.Result, blob);
        CurrentImage = blob;
    }

    private async Task OnReplaceImage()
    {
        var r = await DialogService!.GetFileOpenPath(CommonDialogTemplates.FileOpen with
        {
            Title = string.Format(St.ReplaceFsh, CurrentFshBlobId),
            Text = string.Format(St.ReplaceFshPrompt, CurrentFshBlobId)
        }, FileFilters.CommonBitmapOpenFormats);
        if (r.Success)
        {
            try
            {
                var img = Image.Load(File.OpenRead(r.Result));
                CurrentImage!.ReplaceWith(img, State.File.GetPalette());
                State.Entries.RefreshItem(CurrentImage);
                Notify(nameof(CurrentImage));
            }
            catch (Exception ex)
            {
                await DialogService!.Error(ex);
            }
        }
    }

    private async Task OnDashEditor()
    {
        var state = new GaugeDataState(State.File);
        var vm = new DashEditorViewModel(state) { Title = St.DashEditor };
        vm.StateSaved += Vm_StateSaved;
        await DialogService!.Show(vm);
        vm.StateSaved -= Vm_StateSaved;
    }

    private async Task OnCoordsEditor()
    {
        var state = new FshBlobCoordsState(CurrentImage!);
        var vm = new FshBlobCoordsEditorViewModel(state) { Title = St.CoordsEditor };
        vm.StateSaved += Vm_StateSaved;
        await DialogService!.Show(vm);
        vm.StateSaved -= Vm_StateSaved;
    }

    private void Vm_StateSaved(object? sender, EventArgs e)
    {
        State.UnsavedChanges = true;
    }

    private async Task<(string file, string id, int formatIndex)?> GetNewFshBlobData()
    {
        bool IsGimxInvalid(string? id, [NotNullWhen(true)] out string? errorMessage) => FshExtensions.IsNewGimxIdInvalid(id, State.File, out errorMessage);
        IInputItemDescriptor[] inputs =
        [
            new InputItemDescriptor<string>(d => d.GetFileOpenPath(CommonDialogTemplates.FileOpen with
            {
                Title = St.AddTexture,
                Text = St.AddTexturePrompt
            }, FileFilters.CommonBitmapOpenFormats)!),
            new InputItemDescriptor<string>(d => d.GetInputText(CommonDialogTemplates.Input with
            {
                Title = St.FshId,
                Text = St.FshNewId
            }, InferNewFshBlobName()), IsGimxInvalid),
            new InputItemDescriptor<int>(d => d.SelectOption(CommonDialogTemplates.Input with
            {
                Title = St.FshPixelFormat,
                Text = St.FshPixelFormatPrompt
            }, Mappings.FshBlobToLabel.Values.ToArray()))
        ];
        return (await DialogService!.AskSequentially(inputs) is { } data && (int)data[2] != -1) ? ((string)data[0], (string)data[1], (int)data[2]) : null;
    }

    private string InferNewFshBlobName()
    {
        foreach (var i in Enumerable.Range(0, 10000))
        {
            var n = i.ToString("0000");
            if (!State.Entries.ContainsKey(n)) return n;
        }
        return string.Empty;
    }

    async Task IViewModel.OnNavigateBack(CancelFlag navigation)
    {
        if (State.UnsavedChanges)
        {
            switch (await DialogService!.AskYnc(St.Unsaved, string.Format(St.SaveConfirm, Title)))
            {
                case true: SaveCommand.Execute(State); break;
                case null: navigation.Cancel(); break;
            }
        }
    }
}
