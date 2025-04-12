using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs3;

/// <summary>
/// Builds <see cref="RenderState"/> objects from an FCE editor state data.
/// </summary>
public class Fce3RenderStateBuilder : FceRenderStateBuilderBase<Fce3EditorState, FcePart>, IFceRenderStateBuilder<Fce3EditorState>
{
    /// <inheritdoc/>
    protected override RenderColor[]? GetRenderColors(Fce3EditorState state)
    {
        return state.SelectedColor is { PrimaryColor: { } primary, SecondaryColor: { } secondary } ? [
            new(primary, new(0.2f,0.4f)),
            new(secondary, new(0.6f,0.8f)),
        ] : (RenderColor[]?)null;
    }

    /// <inheritdoc/>
    protected override MaterialFlags InferMaterial(string dummyName)
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
}
