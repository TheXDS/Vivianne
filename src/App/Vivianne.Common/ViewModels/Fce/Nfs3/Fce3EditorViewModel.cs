using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs3;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model for
/// NFS3.
/// </summary>
public class Fce3EditorViewModel : FceEditorViewModelBase<
    FceEditorState,
    FceFile,
    FceColor,
    HsbColor,
    FcePart,
    FceTriangle,
    FceRenderState>
{
    protected override FceRenderState GetRenderTree(IEnumerable<FcePart> visibleParts)
    {
        return new()
        {
            VisibleParts = Parts.Where(p => p.IsVisible).Select(p => p.Part),
            SelectedColor = SelectedColor,
            Texture = SelectedCarTexture,
            FceFile = RenderShadow ? State.File : null
        };
    }
}


public class Fce3RenderStateBuilder : IFceRenderStateBuilder<Fce3EditorState>
{
    private record VertexUv(Vector3 Vertex, Vector2 Uv, Vector3 Normal);

    public RenderState Build(Fce3EditorState state) => new()
    {
        Objects = state.Parts.Select(ToSceneObject)
            .Concat(state.Dummies.Select(CreateDummy))
            .Append(CreateShadow(state)).NotNull(),
        Texture = state.SelectedTexture,
        TextureColors = GetRenderColors(state)
    };

    private SceneObject? ToSceneObject(FcePartListItem<FcePart> part)
    {
        if (!part.IsVisible) return null;
        return null;
    }

    private static SceneObject? CreateDummy(FcePartListItem<FceDummy> dummy)
    {
        if (!dummy.IsVisible) return null;
        MaterialFlags material = InferMaterial(dummy.Part.Name);
        return new()
        {
            Normals = CreateCube(0.02f, dummy.Part.Position),
            Vertices = CreateCube(0.01f, dummy.Part.Position),
            Triangles =
            [
                new() { I1 = 1, I2 = 0, I3 = 2, Material = material },
                new() { I1 = 1, I2 = 2, I3 = 3, Material = material },
                new() { I1 = 5, I2 = 4, I3 = 6, Material = material },
                new() { I1 = 5, I2 = 6, I3 = 7, Material = material },

                // TODO: add more faces
            ]
        };
    }

    private static SceneObject? CreateShadow(Fce3EditorState state) => state.RenderShadow ? new()
    {
        Normals =
        [
            new(state.File.YHalfSize, state.File.ZHalfSize, state.File.XHalfSize),
            new(state.File.YHalfSize, state.File.ZHalfSize, -state.File.XHalfSize),
            new(state.File.YHalfSize, -state.File.ZHalfSize, state.File.XHalfSize),
            new(state.File.YHalfSize, -state.File.ZHalfSize, -state.File.XHalfSize),
        ],
        Triangles =
        [
            new() { I1 = 1, I2 = 0, I3 = 2, Material = MaterialFlags.NoShading },
            new() { I1 = 1, I2 = 2, I3 = 3, Material = MaterialFlags.NoShading }
        ],
        Vertices =
        [
            new(-state.File.YHalfSize, state.File.ZHalfSize, state.File.XHalfSize),
            new(-state.File.YHalfSize, state.File.ZHalfSize, -state.File.XHalfSize),
            new(-state.File.YHalfSize, -state.File.ZHalfSize, state.File.XHalfSize),
            new(-state.File.YHalfSize, -state.File.ZHalfSize, -state.File.XHalfSize),
        ],
    } : null;

    private static Vector3[] CreateCube(float size, Vector3 location)
    {
        return [..((Vector3[])[
            new Vector3(1, 1, 1),
            new Vector3(1, 1, -1),
            new Vector3(1, -1, 1),
            new Vector3(1, -1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, -1, -1),
        ]).Select(p => (p * size) + location)];
    }

    private static MaterialFlags InferMaterial(string dummyName)
    {
        return dummyName[0] switch
        {
            'H' => MaterialFlags.WhiteDummy,
            'T' => MaterialFlags.RedChannel | MaterialFlags.NoShading,
            'S' when dummyName[2] == 'L' => MaterialFlags.RedChannel | MaterialFlags.NoShading,
            'S' when dummyName[2] == 'R' => MaterialFlags.BlueChannel | MaterialFlags.NoShading,
            _ => MaterialFlags.GreenChannel
        };
    }

    private RenderColor[]? GetRenderColors(Fce3EditorState state)
    {
        if (state.SelectedColor is not { PrimaryColor: { } primary, SecondaryColor: { } secondary }) return null;
        return
        [
            new(primary, new(0.2f,0.4f)),
            new(secondary, new(0.6f,0.8f)),
        ];
    }
}

public class Fce3EditorState : FileStateBase<FceFile>
{
    private ObservableListWrap<FceColor>? _colors;
    private ObservableCollection<FcePartListItem<FceDummy>>? _dummies;
    private ObservableCollection<FcePartListItem<FcePart>>? _parts;
    private byte[]? selectedTexture;
    private FceColor? selectedColor;
    private bool renderShadow;

    /// <summary>
    /// Gets a collection of the available colors from the FCE file.
    /// </summary>
    public ObservableListWrap<FceColor> Colors => _colors ??= GetObservable(ColorsFromFce(File));

    /// <summary>
    /// Gets a collection of all available dummies from te FCE file.
    /// </summary>
    public ObservableCollection<FcePartListItem<FceDummy>> Dummies => _dummies ??= [.. GetListItem(File.Dummies)];

    /// <summary>
    /// Gets a collection of all available parts from the FCE file.
    /// </summary>
    public ObservableCollection<FcePartListItem<FcePart>> Parts => _parts ??= [.. GetListItem(File.Parts)];

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
    public FceColor? SelectedColor
    {
        get => selectedColor;
        set => Change(ref selectedColor, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the shadow should be rendered.
    /// </summary>
    public bool RenderShadow
    {
        get => renderShadow;
        set => Change(ref renderShadow, value);
    }

    private static IEnumerable<FcePartListItem<T>> GetListItem<T>(IList<T> parts)
    {
        return parts.Select(p => new FcePartListItem<T>(p));
    }

    private static List<FceColor> ColorsFromFce(FceFile fce)
    {
        ICollection<HsbColor> primary = fce.PrimaryColors;
        IEnumerable<HsbColor> secondary = fce.SecondaryColors.Count > 0 ? fce.SecondaryColors.ToArray().Wrapping(16) : primary;
        return [.. primary.Zip(secondary).Select(p => new FceColor { Name = p.First.ToString(), PrimaryColor = p.First, SecondaryColor = p.Second })];
    }
}