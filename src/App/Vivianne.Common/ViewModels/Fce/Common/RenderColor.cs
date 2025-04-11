using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Defines a color entry for applying a color to the texture used for
/// rendering.
/// </summary>
/// <param name="Color">Color to be mixed up with the texture.</param>
/// <param name="AlphaRange">Range of alpha values that indicates the color
/// mask to apply the <paramref name="Color"/>.</param>
public record struct RenderColor(IHsbColor Color, Range<float> AlphaRange);
