using System.Collections.Generic;
using System.Numerics;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Describes a single scene object.
/// </summary>
public sealed class SceneObject
{
    /// <summary>
    /// Gets the entire set of vertices for the object after origin
    /// transformations have been applied.
    /// </summary>
    public required IEnumerable<Vector3> Vertices { get; init; }

    /// <summary>
    /// Gets the entire set of normals for the object after origin
    /// transformations have been applied.
    /// </summary>
    public required IEnumerable<Vector3> Normals { get; init; }

    /// <summary>
    /// Gets all the defined triangles for the object.
    /// </summary>
    public required IEnumerable<FceTriangle> Triangles { get; init; }
}
