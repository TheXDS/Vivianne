using System.Numerics;
using TheXDS.Vivianne.Models.Geo;

namespace TheXDS.Vivianne.ViewModels.Geo;

/// <summary>
/// Describes a single scene object.
/// </summary>
public class SceneObject
{
    /// <summary>
    /// Gets the entire set of vertices for the object after origin
    /// transformations have been applied.
    /// </summary>
    public required Vector3[] Vertices { get; init; }

    /// <summary>
    /// Gets all the defined quads for the object.
    /// </summary>
    public required GeoFace[] Faces { get; init; }
}
