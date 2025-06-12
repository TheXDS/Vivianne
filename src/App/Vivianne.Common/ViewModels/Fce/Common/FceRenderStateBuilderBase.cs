using System.Linq;
using System.Numerics;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Base class for a type that builds <see cref="RenderState"/> objects from an
/// FCE editor state data.
/// </summary>
public abstract class FceRenderStateBuilderBase<TState, TPart>
    where TState : IFceEditorState<TPart>
    where TPart : FcePart
{
    /// <summary>
    /// Builds the entire render state from the specified editor state.
    /// </summary>
    /// <param name="state">
    /// State from which to extract the render scene data.
    /// </param>
    /// <returns>
    /// A new <see cref="RenderState"/> that can be used to render and display
    /// the FCE model.
    /// </returns>
    public RenderState Build(TState state) => new()
    {
        Objects = state.Parts.Select(p => ToSceneObject(state, p))
            .Concat(state.Dummies.Select(CreateDummy))
            .Append(CreateShadow(state)).NotNull(),
        Texture = state.SelectedTexture,
        TextureColors = GetRenderColors(state),
        ForceUFlip = FlipUvX(state),
        ForceVFlip = FlipUvY()
    };

    /// <summary>
    /// When overriden in a derivate class, allows the inheritor to force the UV Y flip value.
    /// </summary>
    /// <returns></returns>
    protected virtual bool? FlipUvY() => null;

    /// <summary>
    /// When overriden in a derivate class, allows the inheritor to force the UV X flip value.
    /// </summary>
    /// <returns></returns>
    protected virtual bool? FlipUvX(TState state) => null;

    private static SceneObject? CreateShadow(TState state) => state.RenderShadow ? CreateShadow(state.HalfSize) : null;

    /// <summary>
    /// Creates a dummy object.
    /// </summary>
    /// <param name="dummy">Dummy part to be rendered.</param>
    /// <returns>
    /// A new <see cref="SceneObject"/> conformed by a small cube with a
    /// material that's determined by the dummy name, or <see langword="null"/>
    /// if the dummy is not visible in the render tree.
    /// </returns>
    protected SceneObject? CreateDummy(FcePartListItem<FceDummy> dummy) => dummy.IsVisible ? new()
    {
        Normals = CreateCube(0.04f, dummy.Part.Position),
        Vertices = CreateCube(0.02f, dummy.Part.Position),
        Triangles = GetCubeTriangles(InferMaterial(dummy.Part.Name))
    } : null;

    /// <summary>
    /// Infers the material to use for a dummy based on its name.
    /// </summary>
    /// <param name="dummyName">Name of the dummy.</param>
    /// <returns>A material that's suitable to render the dummy.</returns>
    protected abstract MaterialFlags InferMaterial(string dummyName);

    /// <summary>
    /// Transforms the colors as defined in the FCE file format to a universal
    /// <see cref="RenderColor"/> value that can be passed to the rendering
    /// engine.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    protected abstract RenderColor[]? GetRenderColors(TState state);

    /// <summary>
    /// Creates a shadow mesh based on the FCE half-size properties.
    /// </summary>
    /// <param name="modelHalfSize">Vector with the model half size.</param>
    /// <returns>
    /// A new <see cref="SceneObject"/> conformed by a plane with a black,
    /// non-shaded material that represents the shadow of the FCE model based
    /// on its half-size bounding box.
    /// </returns>
    protected static SceneObject CreateShadow(Vector3 modelHalfSize)
    {
        var verts = CreateCube(-modelHalfSize, Vector3.Zero);
        return new()
        {
            Normals = verts,
            Triangles = [
                new() { I1 = 0, I2 = 2, I3 = 3, Flags = (int)MaterialFlags.NoShading },
                new() { I1 = 0, I2 = 3, I3 = 1, Flags = (int)MaterialFlags.NoShading },
            ],
            Vertices = [ verts[0], verts[1], verts[4], verts[5] ],
        };
    }

    /// <summary>
    /// Creates a cube vertex array.
    /// </summary>
    /// <param name="size">Size of the cube.</param>
    /// <param name="location">Intended center location of the cube.</param>
    /// <returns>
    /// The vertex array for a cube.
    /// </returns>
    protected static Vector3[] CreateCube(Vector3 size, Vector3 location)
    {
        return
        [
#if NET9_0_OR_GREATER
            // These FMA calls are hardware accelerated in .NET 9 and later.
            Vector3.FusedMultiplyAdd(new Vector3( 1,  1,  1), size, location),
            Vector3.FusedMultiplyAdd(new Vector3( 1,  1, -1), size, location),
            Vector3.FusedMultiplyAdd(new Vector3( 1, -1,  1), size, location),
            Vector3.FusedMultiplyAdd(new Vector3( 1, -1, -1), size, location),
            Vector3.FusedMultiplyAdd(new Vector3(-1,  1,  1), size, location),
            Vector3.FusedMultiplyAdd(new Vector3(-1,  1, -1), size, location),
            Vector3.FusedMultiplyAdd(new Vector3(-1, -1,  1), size, location),
            Vector3.FusedMultiplyAdd(new Vector3(-1, -1, -1), size, location),
#else
            // .NET 8 and earlier did not support hardware-accelerated FMA.
            (new Vector3( 1,  1,  1) * size) + location,
            (new Vector3( 1,  1, -1) * size) + location,
            (new Vector3( 1, -1,  1) * size) + location,
            (new Vector3( 1, -1, -1) * size) + location,
            (new Vector3(-1,  1,  1) * size) + location,
            (new Vector3(-1,  1, -1) * size) + location,
            (new Vector3(-1, -1,  1) * size) + location,
            (new Vector3(-1, -1, -1) * size) + location,
#endif
        ];
    }

    /// <summary>
    /// Creates a cube vertex array.
    /// </summary>
    /// <param name="size">Size of the cube.</param>
    /// <param name="location">Intended center location of the cube.</param>
    /// <returns>
    /// The vertex array for a cube.
    /// </returns>
    protected static Vector3[] CreateCube(float size, Vector3 location)
    {
        return CreateCube(new Vector3(size), location);
    }

    /// <summary>
    /// Converts an FCE part into a <see cref="SceneObject"/>.
    /// </summary>
    /// <param name="state">FCE editor state data.</param>
    /// <param name="part">Part to be converted.</param>
    /// <returns>
    /// The converted part as a <see cref="SceneObject"/>, or
    /// <see langword="null"/> if the part was not visible in the render tree.
    /// </returns>
    protected virtual SceneObject? ToSceneObject(TState state, FcePartListItem<TPart> part) => part.IsVisible ? new()
    {
        Normals = part.Part.Normals,
        Vertices = part.Part.TransformedVertices,
        Triangles = [.. part.Part.Triangles.Select(NormalizeMaterial)]
    } : null;

    /// <summary>
    /// Generates an array of triangles corresponding to the cube vertex array
    /// generated by <see cref="CreateCube(float, Vector3)"/>
    /// </summary>
    /// <param name="material">
    /// Material to be assigned to the triangles.
    /// </param>
    /// <returns>
    /// An array of <see cref="FceTriangle"/> values that correspond to the
    /// vertex array generated by <see cref="CreateCube(float, Vector3)"/>.
    /// </returns>
    protected static FceTriangle[] GetCubeTriangles(MaterialFlags material) => [
        new() { I1 = 0, I2 = 1, I3 = 2, Flags = (int)material },
        new() { I1 = 2, I2 = 1, I3 = 3, Flags = (int)material },
        new() { I1 = 5, I2 = 4, I3 = 6, Flags = (int)material },
        new() { I1 = 5, I2 = 6, I3 = 7, Flags = (int)material },
        new() { I1 = 2, I2 = 6, I3 = 4, Flags = (int)material },
        new() { I1 = 0, I2 = 2, I3 = 4, Flags = (int)material },
        new() { I1 = 0, I2 = 4, I3 = 5, Flags = (int)material },
        new() { I1 = 0, I2 = 5, I3 = 1, Flags = (int)material },
        new() { I1 = 2, I2 = 7, I3 = 6, Flags = (int)material },
        new() { I1 = 2, I2 = 3, I3 = 7, Flags = (int)material },
        new() { I1 = 3, I2 = 5, I3 = 7, Flags = (int)material },
        new() { I1 = 1, I2 = 5, I3 = 3, Flags = (int)material },
    ];

    /// <summary>
    /// Creates a copy of the triangle normalizing its material flags for
    /// rendering.
    /// </summary>
    /// <param name="triangle">Triangle to normalize.</param>
    /// <returns>
    /// A triangle with the same data as the original, but with its flags
    /// masked to expose only those which affect material selection.
    /// </returns>
    protected static FceTriangle NormalizeMaterial(FceTriangle triangle) => new()
    {
        I1 = triangle.I1,
        I2 = triangle.I2,
        I3 = triangle.I3,
        Flags = triangle.Flags & (int)MaterialFlags.FceMaterialMask,
        TexturePage = triangle.TexturePage,
        U1 = triangle.U1,
        U2 = triangle.U2,
        U3 = triangle.U3,
        V1 = triangle.V1,
        V2 = triangle.V2,
        V3 = triangle.V3,
        Unk_0x10 = triangle.Unk_0x10,
    };
}