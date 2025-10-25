using System.Linq;
using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.Models.Shared;
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
        } ? (RenderColor[])[
            new(driverHair, new(0.1f,0.3f)),
            new(secondary, new(0.3f,0.5f)),
            new(interior, new(0.5f,0.7f)),
            new(primary, new(0.7f,0.9f)),
        ] : null;
    }

    /// <inheritdoc/>
    protected override bool? FlipUvY() => false;

    /// <inheritdoc/>
    protected override bool? FlipUvX(Fce4EditorState state) => (VersionIdentifier.FceVersion(state.File.Magic) == NfsVersion.Mco) ? false : null;

    /// <inheritdoc/>
    protected override SceneObject? ToSceneObject(Fce4EditorState state, FcePartListItem<Fce4Part> part) => part.IsVisible ? new()
    {
        Normals = state.ShowDamagedModel ? part.Part.DamagedNormals : part.Part.Normals,
        Vertices = state.ShowDamagedModel ? part.Part.TransformedDamagedVertices : part.Part.TransformedVertices,
        Triangles = [.. part.Part.Triangles.Select(NormalizeMaterial)]
    } : null;

    /// <inheritdoc/>
    protected override MaterialFlags InferMaterial(string dummyName)
    {
        return dummyName.TrimStart(':').ElementAtOrDefault(1) switch
        {
            'W' => MaterialFlags.WhiteDummy,
            'R' => MaterialFlags.RedChannel | MaterialFlags.NoShading,
            'O' or 'Y' => MaterialFlags.RedChannel | MaterialFlags.GreenChannel | MaterialFlags.NoShading,
            'B' => MaterialFlags.BlueChannel | MaterialFlags.NoShading,
            _ => MaterialFlags.GreenChannel
        };
    }
}
