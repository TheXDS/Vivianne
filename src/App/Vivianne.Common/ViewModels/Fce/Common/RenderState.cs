using System.Collections.Generic;
using System.Linq;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Describes the full render scene state for an FCE file, independently of the
/// game version.
/// </summary>
public sealed class RenderState
{
    /// <summary>
    /// Enumerates all visible objects in the scene.
    /// </summary>
    public required IEnumerable<SceneObject> Objects { get; init; }

    /// <summary>
    /// Gets the number of visible triangles in the current scene.
    /// </summary>
    public int TriangleCount => Objects.Sum(p => p.Triangles.Length);

    /// <summary>
    /// Gets the total number of vertices across all objects in the collection.
    /// </summary>
    public int VertexCount => Objects.Sum(p => p.Vertices.Length);

    /// <summary>
    /// Enumerates the colors to be mixed with the texture for the specific
    /// alpha ranges supported by the FCE version being rendered.
    /// </summary>
    /// <value>
    /// An enumeration of the different color ranges and values to be mixed
    /// with the texture, using the alpha channel as a reference. May be set to
    /// <see langword="null"/> to indicate that no texture-color mixing shall
    /// occurr.
    /// </value>
    /// <remarks>
    /// FCE3 will define either 1 or 2 elements (primary and secondary colors).
    /// In case that the array includes a single element, the same primary
    /// color shall be used as the secondary color to be mixed. FCE4/FCE4M will
    /// always include 4 entries corresponding to primary, interior, secondary
    /// and driver's hair (or tertiary) colors.
    /// </remarks>
    public RenderColor[]? TextureColors { get; init; }

    /// <summary>
    /// Gets a reference to the raw contents of the texture to be loaded and
    /// used for rendering.
    /// </summary>
    public required byte[]? Texture { get; init; }

    /// <summary>
    /// Gets or sets a value that forces the UV vertical flip to a specific
    /// value.
    /// </summary>
    public bool? ForceVFlip { get; init; }

    /// <summary>
    /// Gets or sets a value that forces the UV horizontal flip to a specific
    /// value.
    /// </summary>
    public bool? ForceUFlip { get; init; }
}
