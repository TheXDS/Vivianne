using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Models.Fsh;

namespace TheXDS.Vivianne.ViewModels.Geo;

/// <summary>
/// Describes the full render scene state for a GEO file.
/// </summary>
/// <remarks>
/// This is different from the Render state used for FCE files as seen in later
/// games, because .GEO uses fundamentally different structures and texturing
/// techniques; such as quads instead of triangles, and per-quad texturing.
/// </remarks>
public sealed class RenderState
{
    /// <summary>
    /// Enumerates all visible objects in the scene.
    /// </summary>
    public required IEnumerable<SceneObject> Objects { get; init; }

    /// <summary>
    /// Gets the number of visible quads in the current scene.
    /// </summary>
    public int QuadsCount => Objects.Sum(p => p.Faces.Length);

    /// <summary>
    /// Gets the number of visible triangles in the current scene.
    /// </summary>
    public int TrianglesCount => QuadsCount * 2;

    /// <summary>
    /// Gets the total number of vertices across all objects in the collection.
    /// </summary>
    public int VertexCount => Objects.Sum(p => p.Vertices.Length);

    /// <summary>
    /// Gets a dictionary containing each texture used by the scene.
    /// </summary>
    public required FshFile Textures { get; init; }
}
