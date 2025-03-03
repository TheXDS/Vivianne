using System.Collections.Generic;

namespace TheXDS.Vivianne.Models.Fce.Nfs3;

/// <summary>
/// Represents the render tree state for an FCE editor.
/// </summary>
public class FceRenderState
{
    /// <summary>
    /// Gets an enumeration of all currently visible parts.
    /// </summary>
    public IEnumerable<FcePart> VisibleParts { get; init; } = [];

    /// <summary>
    /// Gets or sets a reference to the texture to be applied to the model
    /// during rendering.
    /// </summary>
    public byte[]? Texture { get; init; }

    /// <summary>
    /// Gets or sets the color to apply to the car texture during rendering.
    /// </summary>
    public FceColor? SelectedColor { get; init; }

    /// <summary>
    /// Gets or sets a reference to the FCE file being rendered.
    /// </summary>
    public FceFile? FceFile { get; init; }
}
