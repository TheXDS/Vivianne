using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Represents the renderring state for the <see cref="FcePreviewViewModel"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RenderTreeState"/> class.
/// </remarks>
/// <param name="source">Data source for the render tree.</param>
public class RenderTreeState(FcePreviewViewModel source)
{
    private readonly FcePreviewViewModel source = source;

    /// <summary>
    /// Enumerates the visible parts from the source ViewModel.
    /// </summary>
    public IEnumerable<FcePart> Parts => source.Parts.Where(p => p.IsVisible).Select(p => p.Part);

    /// <summary>
    /// Gets the pre-transformed texture to be used to draw the FCE model. This will include applied car colors.
    /// </summary>
    public byte[]? Texture => source.SelectedCarTexture;
}
