using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models.Fe;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Includes extension methods for the <see cref="IFeData"/> interface.
/// </summary>
internal static class FeDataExtensions
{ 
    /// <summary>
    /// Gets the collection of defined color names from the specified
    /// <see cref="IFeData"/>.
    /// </summary>
    /// <param name="feData">Instance to get the color names from.</param>
    /// <returns></returns>
    public static string[] GetColorNames(this IFeData feData)
    {
        return [.. ((string[])[
            feData.Color1,
            feData.Color2,
            feData.Color3,
            feData.Color4,
            feData.Color5,
            feData.Color6,
            feData.Color7,
            feData.Color8,
            feData.Color9,
            feData.Color10]).NotEmpty()];
    }
}
