using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs4;

/// <summary>
/// Builds <see cref="RenderState"/> objects from an FCE editor state data.
/// </summary>
public class Fce4RenderStateBuilder : FceRenderStateBuilderBase<Fce4EditorState, Fce4Part>, IFceRenderStateBuilder<Fce4EditorState>
{
    /// <inheritdoc/>
    protected override RenderColor[]? GetRenderColors(Fce4EditorState state)
    {
        return state.SelectedColor is
        {
            PrimaryColor: { } primary,
            InteriorColor: { } interior,
            SecondaryColor: { } secondary,
            DriverHairColor: { } driverHair,
        } ? (RenderColor[]) [
            new(primary, new(0.3f,0.4f)),
            new(interior, new(0.4f,0.5f)),
            new(secondary, new(0.5f,0.6f)),
            new(driverHair, new(0.6f,0.7f)),
        ] : null;
    }

    /// <inheritdoc/>
    protected override SceneObject? ToSceneObject(Fce4EditorState state, FcePartListItem<Fce4Part> part) => part.IsVisible ? new()
    {
        Normals = state.ShowDamagedModel ? part.Part.DamagedNormals : part.Part.Normals,
        Vertices = state.ShowDamagedModel ? part.Part.TransformedDamagedVertices : part.Part.TransformedVertices,
        Triangles = part.Part.Triangles
    } : null;

    /// <inheritdoc/>
    protected override MaterialFlags InferMaterial(string dummyName)
    {
        return dummyName[1] switch
        {
            'W' => MaterialFlags.WhiteDummy,
            'R' => MaterialFlags.RedChannel | MaterialFlags.NoShading,
            'O' or 'Y' => MaterialFlags.RedChannel | MaterialFlags.GreenChannel | MaterialFlags.NoShading,
            'B' => MaterialFlags.BlueChannel | MaterialFlags.NoShading,
            _ => MaterialFlags.GreenChannel
        };
    }
}
