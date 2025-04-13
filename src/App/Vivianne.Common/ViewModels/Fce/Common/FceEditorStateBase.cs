using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Base class for all types that represent FCE editor states.
/// </summary>
/// <typeparam name="TFceFile">Type of FCE file to store the FCE model data into.</typeparam>
/// <typeparam name="TFceColor">Type used by this state to represent a single FCE color.</typeparam>
/// <typeparam name="THsbColor">Type used by the FCE file to store a single FCE color.</typeparam>
/// <typeparam name="TFcePart">Type of FCE part to be stored on the FCE file.</typeparam>
public abstract class FceEditorStateBase<TFceFile, TFceColor, THsbColor, TFcePart>
    : FileStateBase<TFceFile>, IFceEditorState<TFcePart>
    where TFceFile : FceFileBase<THsbColor, TFcePart>
    where TFceColor : IFceColor<THsbColor>
    where TFcePart : FcePart
    where THsbColor : IHsbColor
{
    private ObservableListWrap<TFceColor>? _colors;
    private ObservableCollection<FcePartListItem<TFcePart>>? _parts;
    private ObservableCollection<FcePartListItem<FceDummy>>? _dummies;
    private ObservableCollection<NamedObject<byte[]>>? _carTextures;
    private byte[]? selectedTexture;
    private TFceColor? selectedColor;
    private bool renderShadow;
    private FceLodPreset _lodPreset;
    private RenderState? _renderTree;

    /// <summary>
    /// Enumerates the FCE colors defined in the FCE file.
    /// </summary>
    /// <param name="fce">FCE file to extract the colors from.</param>
    /// <returns>A list of all the colors defined in the FCE file.</returns>
    protected abstract List<TFceColor> ColorsFromFce(TFceFile fce);

    /// <summary>
    /// Gets a collection of the available colors from the FCE file.
    /// </summary>
    public ObservableListWrap<TFceColor> Colors => _colors ??= GetObservable(ColorsFromFce(File));

    /// <summary>
    /// Gets a collection of all available dummies from te FCE file.
    /// </summary>
    public ObservableCollection<FcePartListItem<FceDummy>> Dummies => _dummies ??= [.. GetListItem(File.Dummies)];

    /// <summary>
    /// Gets a collection of all available elements from the FCE file.
    /// </summary>
    public ObservableCollection<FcePartListItem<TFcePart>> Parts => _parts ??= [.. GetListItem(File.Parts)];

    /// <summary>
    /// Gets a collection of the available textures for rendering.
    /// </summary>
    public ObservableCollection<NamedObject<byte[]>> CarTextures => _carTextures ??= [];

    /// <summary>
    /// Gets or sets the raw contents of the file from which to load the
    /// texture to use when rendering the FCE model.
    /// </summary>
    public byte[]? SelectedTexture
    {
        get => selectedTexture;
        set => Change(ref selectedTexture, value);
    }

    /// <summary>
    /// Gets or sets a reference to the selected car color.
    /// </summary>
    public TFceColor? SelectedColor
    {
        get => selectedColor;
        set => Change(ref selectedColor, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the shadow should be rendered.
    /// </summary>
    /// <remarks>
    /// The car shadow render is useful to check for the "floating car" issue
    /// that some mods might have.
    /// </remarks>
    public bool RenderShadow
    {
        get => renderShadow;
        set => Change(ref renderShadow, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the LOD preset to load.
    /// </summary>
    public FceLodPreset FceLodPreset
    {
        get => _lodPreset;
        set => Change(ref _lodPreset, value);
    }

    /// <summary>
    /// Gets a reference to an object that describes the rendered scene.
    /// </summary>
    public RenderState? RenderTree
    {
        get => _renderTree;
        set => Change(ref _renderTree, value);
    }

    /// <inheritdoc/>
    public Vector3 HalfSize => new(File.YHalfSize, File.ZHalfSize, File.XHalfSize);

    /// <summary>
    /// Builds a hideable list for object lists where each element may be
    /// hidden from view.
    /// </summary>
    /// <typeparam name="T">Type of elements to expose.</typeparam>
    /// <param name="elements">original element list.</param>
    /// <returns></returns>
    protected static IEnumerable<FcePartListItem<T>> GetListItem<T>(IList<T> elements)
    {
        return elements.Select(p => new FcePartListItem<T>(p));
    }

    IEnumerable<FcePartListItem<TFcePart>> IFceEditorState<TFcePart>.Parts => Parts;

    IEnumerable<FcePartListItem<FceDummy>> IFceEditorState<TFcePart>.Dummies => Dummies;
}
