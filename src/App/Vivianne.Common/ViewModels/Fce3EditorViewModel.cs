using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model.
/// </summary>
public class Fce3EditorViewModel : FileEditorViewModelBase<Fce3EditorState, FceFile>
{
    private NamedObject<byte[]>? _selectedTexture;
    private Fce3Color? _selectedColor;
    private FceLodPreset _lodPreset;
    private FceRenderState _renderTree;

    /// <summary>
    /// Gets a collection of the available textures for rendering.
    /// </summary>
    public ICollection<NamedObject<byte[]>> CarTextures { get; } = new ObservableCollection<NamedObject<byte[]>>();

    /// <summary>
    /// Gets or sets the raw contents of the selected car texture.
    /// </summary>
    /// <remarks>By NFS3 standard, the raw texture will be in Targa (TGA) format.</remarks>
    public NamedObject<byte[]>? SelectedCarTexture
    {
        get => _selectedTexture;
        set => Change(ref _selectedTexture, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the index of the selected Car color.
    /// </summary>
    public Fce3Color? SelectedColor
    {
        get => _selectedColor;
        set => Change(ref _selectedColor, value);
    }

    public FceLodPreset FceLodPreset
    {
        get => _lodPreset;
        set
        {
            if (Change(ref _lodPreset, value)) SwitchToLod(value);
        }
    }

    public ObservableListWrap<FcePartListItem> Parts { get; private set; }

    public FceRenderState RenderTree
    { 
        get => _renderTree;
        private set => Change(ref _renderTree, value);
    }

    private static IEnumerable<NamedObject<Fce3Color>> CreateColors(IFeData? feData, FceFile fce)
    {
        static string[] ReadColors(IFeData fe) => [fe.Color1, fe.Color2, fe.Color3, fe.Color4, fe.Color5, fe.Color6, fe.Color7, fe.Color8, fe.Color9, fe.Color10];
        var names = (feData is not null ? ReadColors(feData) : fce.PrimaryColors.Select(p => p.ToString())).ToArray();
        return CreateFromFce(fce).WithIndex().Select(p => new NamedObject<Fce3Color>(p.element, names[p.index]));
    }

    private static IEnumerable<Fce3Color> CreateFromFce(FceFile fce)
    {
        ICollection<HsbColor> primary = fce.PrimaryColors;
        IEnumerable<HsbColor> secondary = fce.SecondaryColors.Count > 0 ? fce.SecondaryColors.ToArray().Wrapping(16) : primary;
        return primary.Zip(secondary).Select(p => new Fce3Color { PrimaryColor = p.First, SecondaryColor = p.Second});
    }

    private static IEnumerable<NamedObject<byte[]>> GetTextures(IDictionary<string, byte[]> vivDirectory)
    {
        return vivDirectory.Where(p => p.Key.EndsWith(".tga")).Select(p => new NamedObject<byte[]>(p.Value, p.Key));
    }

    protected override Task OnCreated()
    {

        Parts = GetObservable();
        //State.CarTextures.AddRange(GetTextures(vivDirectory));
        //State.Colors.AddRange(CreateColors(vivDirectory.GetAnyParsedFeData(), State.File));
        return base.OnCreated();
    }





    /// <summary>
    /// Initializes a new instance of the <see cref="Fce3EditorViewModel"/>
    /// class.
    /// </summary>
    public Fce3EditorViewModel()
    {
        var cb = CommandBuilder.For(this);
        ColorEditorCommand = cb.BuildSimple(OnColorEditor);
    }

    /// <summary>
    /// Gets a reference to the command used to open a dialog to edit the FCE
    /// color tables.
    /// </summary>
    public ICommand ColorEditorCommand { get; }

    private async Task OnColorEditor()
    {
        //var state = new FceColorTableEditorState(fce);
        //var vm = new FceColorEditorViewModel(state);
        //vm.StateSaved += (sender, e) =>
        //{
        //    CarColors.Clear();
        //    CarColors.AddRange(CreateColors(vivDirectory, fce.Header));
        //    SelectedColorIndex = 0;
        //};
        //await DialogService!.Show(vm);
    }

    private void SwitchToLod(FceLodPreset preset)
    {
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
            j.IsVisible = partsToShow.Contains(j);
        }
    }


    private ObservableListWrap<FcePartListItem> GetObservable()
    {
        var c = new ObservableListWrap<FcePartListItem>([.. State.File.Parts.Select(p => new FcePartListItem(p))]);
        c.CollectionChanged += (_, e) =>
        {
            if (e.OldItems is not null) foreach (var j in e.OldItems.Cast<FcePartListItem>()) j.Unsubscribe(() => j.IsVisible);
            if (e.NewItems is not null) foreach (var j in e.NewItems.Cast<FcePartListItem>()) j.Subscribe(() => j.IsVisible, OnVisibleChanged);
        };
        c.Refresh();
        return c;
    }

    private void OnVisibleChanged(object instance, PropertyInfo property, PropertyChangeNotificationType notificationType)
    {
        RenderTree = new()
        {
            VisibleParts = Parts.Where(p => p.IsVisible).Select(p => p.Part),
            SelectedColor = SelectedColor,
            Texture = SelectedCarTexture,
        };
    }
}
