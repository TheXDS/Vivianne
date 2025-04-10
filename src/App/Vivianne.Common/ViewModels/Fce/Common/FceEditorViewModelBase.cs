using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Resources;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.FceEditorView;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Base class for all types that represent FCE editor view models.
/// </summary>
/// <typeparam name="TState">Type of internal ViewModel state to use.</typeparam>
/// <typeparam name="TFile">Type of FCE file to be edited by this ViewModel.</typeparam>
/// <typeparam name="TFceColor">Type used by this ViewModel to represent a single FCE color.</typeparam>
/// <typeparam name="THsbColor">Type used by the FCE file to store a single FCE color.</typeparam>
/// <typeparam name="TFcePart">Type of FCE part to be stored and represented by this ViewModel.</typeparam>
/// <typeparam name="TFceTriangle">Type of FCE triangle used in the FCE parts.</typeparam>
/// <typeparam name="TFceRenderTree">Type of render tree to use when rendering the FCE model.</typeparam>
public abstract class FceEditorViewModelBase<TState, TFile, TFceColor, THsbColor, TFcePart, TFceTriangle, TFceRenderTree>
    : FileEditorViewModelBase<TState, TFile>
    where TState : FileStateBase<TFile>, IFceEditorState<TFceColor, THsbColor, TFcePart,TFceTriangle>, new()
    where TFile : FceFileBase<THsbColor, TFcePart, TFceTriangle>, new()
    where TFceColor : IFceColor<THsbColor>
    where THsbColor : IHsbColor
    where TFcePart : FcePartBase<TFceTriangle>
    where TFceTriangle : IFceTriangle
    where TFceRenderTree : IFceRenderState<TFcePart, TFceTriangle, TFceColor, THsbColor, TFile>, new()
{
    private static async IAsyncEnumerable<NamedObject<byte[]>> GetTextures(IBackingStore? store)
    {
        foreach (var file in store?.EnumerateFiles().Where(p => p.EndsWith(".tga")) ?? [])
        {
            if (await store!.ReadAsync(file) is byte[] contents)
            {
                yield return new NamedObject<byte[]>(contents, file);
            }
        }
    }

    private FceLodPreset _lodPreset;
    private byte[]? _selectedTexture;
    private TFceColor? _selectedColor;
    private bool _renderShadow = true;
    private bool _refreshEnabled;
    private TFceRenderTree? _renderTree;

    /// <summary>
    /// Gets a reference to an object that describes the rendered scene.
    /// </summary>
    public TFceRenderTree? RenderTree
    {
        get => _renderTree;
        private set => Change(ref _renderTree, value);
    }

    /// <summary>
    /// Gets a collection of the available textures for rendering.
    /// </summary>
    public ICollection<NamedObject<byte[]>> CarTextures { get; } = new ObservableCollection<NamedObject<byte[]>>();

    /// <summary>
    /// Gets or sets a value that enables ot disables rendering a shadow on the
    /// car.
    /// </summary>
    /// <remarks>
    /// The car shadow render is useful to check for the "floating car" issue
    /// that some mods might have.
    /// </remarks>
    public bool RenderShadow
    {
        get => _renderShadow;
        set
        {
            if (Change(ref _renderShadow, value)) OnVisibleChanged(null!, null!, default);
        }
    }

    /// <summary>
    /// Gets or sets the raw contents of the selected car texture.
    /// </summary>
    /// <remarks>By NFS3 standard, the raw texture will be in Targa (TGA) format.</remarks>
    public byte[]? SelectedCarTexture
    {
        get => _selectedTexture;
        set
        {
            if (Change(ref _selectedTexture, value)) OnVisibleChanged(this, null, PropertyChangeNotificationType.PropertyChanged);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates the index of the selected Car color.
    /// </summary>
    public TFceColor? SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (Change(ref _selectedColor, value)) OnVisibleChanged(this, null, PropertyChangeNotificationType.PropertyChanged);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates the LOD preset to load.
    /// </summary>
    public FceLodPreset FceLodPreset
    {
        get => _lodPreset;
        set
        {
            if (Change(ref _lodPreset, value)) SwitchToLod(value);
        }
    }

    /// <summary>
    /// Gets or sets a collection of the parts defined inthe FCE file.
    /// </summary>
    public ObservableListWrap<FcePartListItem<TFcePart>> Parts { get; private set; } = null!;

    /// <summary>
    /// Gets a reference to the command used to open a dialog to edit the FCE
    /// color tables.
    /// </summary>
    public ICommand ColorEditorCommand { get; }

    /// <summary>
    /// Gets a command that applies center transformations to fix a "floating
    /// car".
    /// </summary>
    public ICommand FceCenterCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to rename an FCE part.
    /// </summary>
    public ICommand RenamePartCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to rename an FCE part.
    /// </summary>
    public ICommand RenameDummyCommand { get; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="FceEditorViewModelBase{TState, TFile, TFceColor, THsbColor, TFcePart, TFceTriangle, TFceRenderTree}"/>
    /// class.
    /// </summary>
    protected FceEditorViewModelBase()
    {
        var cb = CommandBuilder.For(this);
        ColorEditorCommand = cb.BuildSimple(OnColorEditor);
        RenamePartCommand = cb.BuildSimple(OnPartRename);
        RenameDummyCommand = cb.BuildSimple(OnDummyRename);
        FceCenterCommand = cb.BuildSimple(OnFceCenter);
    }

    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        RenderShadow = Settings.Current.Fce_ShadowByDefault;
        Parts = GetObservable();
        await foreach (var j in GetTextures(BackingStore?.Store)) CarTextures.Add(j);
        CarTextures.Add(new(null!, St.NoTexture));
        await LoadColorNames();
        _lodPreset = FceLodPreset.High;
        if (CarTextures.Count > 0) SelectedCarTexture = CarTextures.First().Value;
        if (State.Colors.Count > 0) SelectedColor = State.Colors.First();
        State.UnsavedChanges = false;
        SwitchToLod(_lodPreset);
        await base.OnCreated();
    }

    /// <inheritdoc/>
    protected override bool BeforeSave()
    {
        if (Settings.Current.Fce_CenterModel)
        {
            OnFceCenter();
        }
        return base.BeforeSave();
    }

    private void OnVisibleChanged(object? instance, PropertyInfo? property, PropertyChangeNotificationType notificationType)
    {
        if (!_refreshEnabled) return;
        RenderTree = GetRenderTree(Parts.Where(p => p.IsVisible).Select(p => p.Part));
    }

    protected abstract TFceRenderTree GetRenderTree(IEnumerable<TFcePart> visibleParts);

    private void SwitchToLod(FceLodPreset preset)
    {
        _refreshEnabled = false;
        var partsToShow = (preset switch
        {
            FceLodPreset.High => State.Parts.Take(5),
            FceLodPreset.Medium => State.Parts.SkipIfMore(5).Take(5),
            FceLodPreset.Low => State.Parts.SkipIfMore(10).Take(1),
            FceLodPreset.Tiny => State.Parts.SkipIfMore(11).Take(1),
            _ => []
        }).ToArray();

        foreach (var j in Parts)
        {
            j.IsVisible = partsToShow.Contains(j.Part);
        }
        _refreshEnabled = true;
        OnVisibleChanged(null, null, default);
    }

    private async Task OnColorEditor()
    {
        //var state = new FceColorTableEditorState(State);
        //var vm = new FceColorEditorViewModel(state);
        //await DialogService!.Show(vm);
        //await LoadColorNames();
        //SelectedColor = State.Colors.FirstOrDefault();
        //OnVisibleChanged(null!, null!, default);
    }

    private async Task OnPartRename(object? parameter)
    {
        if (parameter is not FcePartListItem<TFcePart> { Part: Models.Base.INameable nameable } part || DialogService is null) return;
        var result = await DialogService.GetInputText(CommonDialogTemplates.Input with { Title = St.RenamePart, Text = St.RenamePartHelp }, nameable.Name);
        if (result.Success)
        {
            nameable.Name = result.Result;
            part.Refresh();
        }
    }

    private async Task OnDummyRename(object? parameter)
    {
        if (parameter is not FceDummy dummy || DialogService is null) return;
        var result = await DialogService.GetInputText(CommonDialogTemplates.Input with { Title = St.RenamePart, Text = St.RenamePartHelp }, dummy.Name);
        if (result.Success)
        {
            dummy.Name = result.Result;
            State.Dummies.Refresh();
        }
    }

    private void OnFceCenter()
    {
        var vertices = State.File.Parts.SelectMany(p => p.TransformedVertices).ToArray();
        var minX = vertices.Min(p => p.X);
        var minY = vertices.Min(p => p.Y);
        var minZ = vertices.Min(p => p.Z);
        var xDiff = minX + State.File.XHalfSize;
        var yDiff = minY + State.File.YHalfSize;
        var zDiff = minZ + State.File.ZHalfSize;
        if (((IEnumerable<float>)[xDiff, yDiff, zDiff]).AreZero()) return;
        foreach (var j in State.File.Parts)
        {
            j.Origin = new Vector3d
            {
                X = j.Origin.X - xDiff,
                Y = j.Origin.Y - yDiff,
                Z = j.Origin.Z - zDiff
            };
        }
        State.UnsavedChanges = true;
        OnVisibleChanged(null, null, PropertyChangeNotificationType.PropertyChanged);
    }

    private ObservableListWrap<FcePartListItem<TFcePart>> GetObservable()
    {
        var c = new ObservableListWrap<FcePartListItem<TFcePart>>([.. State.File.Parts.Select(p => new FcePartListItem<TFcePart>(p))]);
        c.CollectionChanged += (_, e) =>
        {
            if (e.OldItems is not null) foreach (var j in e.OldItems.Cast<FcePartListItem<TFcePart>>()) j.Unsubscribe(() => j.IsVisible);
            if (e.NewItems is not null) foreach (var j in e.NewItems.Cast<FcePartListItem<TFcePart>>()) j.Subscribe(() => j.IsVisible, OnVisibleChanged);
        };
        c.Refresh();
        return c;
    }

    private static void SetColorNames(ICollection<TFceColor> colors, IFeData feData)
    {
        string[] colorNames = feData.GetColorNames();

        foreach (var (index, element) in colors.WithIndex().Take(colorNames.Length))
        {
            element.Name = colorNames[index];
        }
    }

    private async Task LoadColorNames()
    {
        if (await (BackingStore?.Store.GetAnyFeData() ?? Task.FromResult<IFeData?>(null)) is { } fedata)
        {
            SetColorNames(State.Colors, fedata);
        }
    }
}

public interface IFceColor<TColor> : TheXDS.Vivianne.Models.Base.INameable where TColor : IHsbColor
{
    IEnumerable<TColor> Colors { get; }
}

public interface IFceRenderState<TFcePart, TFceTriangle, TFceColor, THsbColor, TFceFile>
    where TFceFile : FceFileBase<THsbColor, TFcePart, TFceTriangle>
    where TFceColor : IFceColor<THsbColor>
    where THsbColor : IHsbColor
    where TFcePart : FcePartBase<TFceTriangle>
    where TFceTriangle : IFceTriangle
{
    /// <summary>
    /// Gets an enumeration of all currently visible parts.
    /// </summary>
    public IEnumerable<TFcePart> VisibleParts { get; init; }

    /// <summary>
    /// Gets or sets a reference to the texture to be applied to the model
    /// during rendering.
    /// </summary>
    public byte[]? Texture { get; init; }

    /// <summary>
    /// Gets or sets the color to apply to the car texture during rendering.
    /// </summary>
    public TFceColor? SelectedColor { get; init; }

    /// <summary>
    /// Gets or sets a reference to the FCE file being rendered.
    /// </summary>
    public TFceFile? FceFile { get; init; }
}

/// <summary>
/// Base class for any type that represents the render tree state for an FCE editor.
/// </summary>
public abstract class FceRenderStateBase<TFcePart, TFceTriangle, TFceColor, THsbColor, TFceFile>
    : IFceRenderState<TFcePart, TFceTriangle, TFceColor, THsbColor, TFceFile>
    where TFceFile : FceFileBase<THsbColor, TFcePart, TFceTriangle>
    where TFceColor : IFceColor<THsbColor>
    where THsbColor : IHsbColor
    where TFcePart : FcePartBase<TFceTriangle>
    where TFceTriangle : IFceTriangle
{
    /// <summary>
    /// Gets an enumeration of all currently visible parts.
    /// </summary>
    public IEnumerable<TFcePart> VisibleParts { get; init; } = [];

    /// <summary>
    /// Gets or sets a reference to the texture to be applied to the model
    /// during rendering.
    /// </summary>
    public byte[]? Texture { get; init; }

    /// <summary>
    /// Gets or sets the color to apply to the car texture during rendering.
    /// </summary>
    public TFceColor? SelectedColor { get; init; }

    /// <summary>
    /// Gets or sets a reference to the FCE file being rendered.
    /// </summary>
    public TFceFile? FceFile { get; init; }
}
