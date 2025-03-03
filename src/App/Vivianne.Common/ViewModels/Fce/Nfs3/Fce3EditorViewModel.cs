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
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.FceEditorView;
namespace TheXDS.Vivianne.ViewModels.Fce.Nfs3;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model.
/// </summary>
public class Fce3EditorViewModel : FileEditorViewModelBase<FceEditorState, FceFile>
{
    private byte[]? _selectedTexture;
    private FceColor? _selectedColor;
    private FceLodPreset _lodPreset;
    private FceRenderState? _renderTree;
    private bool _refreshEnabled;
    private bool _renderShadow = true;

    /// <summary>
    /// Gets a collection of the available textures for rendering.
    /// </summary>
    public ICollection<NamedObject<byte[]>> CarTextures { get; } = new ObservableCollection<NamedObject<byte[]>>();

    /// <summary>
    /// Gets or sets the raw contents of the selected car texture.
    /// </summary>
    /// <remarks>By NFS3 standard, the raw texture will be in Targa (TGA) format.</remarks>
    public byte[]? SelectedCarTexture
    {
        get => _selectedTexture;
        set => Change(ref _selectedTexture, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the index of the selected Car color.
    /// </summary>
    public FceColor? SelectedColor
    {
        get => _selectedColor;
        set => Change(ref _selectedColor, value);
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
    /// Gets or sets a collection of the parts defined inthe FCE file.
    /// </summary>
    public ObservableListWrap<FcePartListItem<FcePart>> Parts { get; private set; } = null!;

    /// <summary>
    /// Gets a reference to an object that describes the rendered scene.
    /// </summary>
    public FceRenderState? RenderTree
    { 
        get => _renderTree;
        private set => Change(ref _renderTree, value);
    }

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
    /// Initializes a new instance of the <see cref="Fce3EditorViewModel"/>
    /// class.
    /// </summary>
    public Fce3EditorViewModel()
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
        Parts = GetObservable();
        await foreach (var j in GetTextures(BackingStore?.Store)) CarTextures.Add(j);
        CarTextures.Add(new(null!, St.NoTexture));
        if (await (BackingStore?.Store.ReadAsync("fedata.eng") ?? Task.FromResult<byte[]?>(null)) is { } fedata)
        {
            SetColorNames(State.Colors, fedata);
        }
        _lodPreset = FceLodPreset.High;
        SwitchToLod(_lodPreset);
        if (CarTextures.Count > 0) SelectedCarTexture = CarTextures.First().Value;
        if (State.Colors.Count > 0) SelectedColor = State.Colors.First();
        State.UnsavedChanges = false;
        await base.OnCreated();
    }

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        Subscribe(() => SelectedCarTexture, OnVisibleChanged);
        Subscribe(() => SelectedColor, OnVisibleChanged);
    }

    private async Task OnColorEditor()
    {
        var state = new FceColorTableEditorState(State);
        var vm = new FceColorEditorViewModel(state);
        await DialogService!.Show(vm);
        OnVisibleChanged(null!, null!, default);
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

    private async Task OnPartRename(object? parameter)
    {
        if (parameter is not FcePartListItem<FcePart> { Part: Models.Base.INameable nameable } part || DialogService is null) return;
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

    private ObservableListWrap<FcePartListItem<FcePart>> GetObservable()
    {
        var c = new ObservableListWrap<FcePartListItem<FcePart>>([.. State.File.Parts.Select(p => new FcePartListItem<FcePart>(p))]);
        c.CollectionChanged += (_, e) =>
        {
            if (e.OldItems is not null) foreach (var j in e.OldItems.Cast<FcePartListItem<FcePart>>()) j.Unsubscribe(() => j.IsVisible);
            if (e.NewItems is not null) foreach (var j in e.NewItems.Cast<FcePartListItem<FcePart>>()) j.Subscribe(() => j.IsVisible, OnVisibleChanged);
        };
        c.Refresh();
        return c;
    }

    private void OnVisibleChanged(object? instance, PropertyInfo? property, PropertyChangeNotificationType notificationType)
    {
        if (!_refreshEnabled) return;
        RenderTree = new()
        {
            VisibleParts = Parts.Where(p => p.IsVisible).Select(p => p.Part),
            SelectedColor = SelectedColor,
            Texture = SelectedCarTexture,
            FceFile = _renderShadow ? State.File : null
        };
    }

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

    private static void SetColorNames(ICollection<FceColor> colors, byte[] feDataContents)
    {
        IFeData feData = feDataContents[0] == 4
            ? ((IOutSerializer<Models.Fe.Nfs4.FeData>)new Serializers.Fe.Nfs4.FeDataSerializer()).Deserialize(feDataContents)
            : ((IOutSerializer<Models.Fe.Nfs3.FeData>)new Serializers.Fe.Nfs3.FeDataSerializer()).Deserialize(feDataContents);

        string[] colorNames = [
            feData.Color1,
            feData.Color2,
            feData.Color3,
            feData.Color4,
            feData.Color5,
            feData.Color6,
            feData.Color7,
            feData.Color8,
            feData.Color9,
            feData.Color10];

        foreach (var (index, element) in colors.WithIndex())
        {
            element.Name = colorNames[index];
        }
    }
}
