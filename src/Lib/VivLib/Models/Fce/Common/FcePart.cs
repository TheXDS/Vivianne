using System.Numerics;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Shared;

namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Represents a single FCE part with all of its vertices, triangles, normals
/// and Origin data.
/// </summary>
public class FcePart : TridimensionalObjectBase, INameable
{
    /// <summary>
    /// Gets or sets the name of the FCE part.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of normals for vertices associated to this
    /// FCE part.
    /// </summary>
    public Vector3[] Normals { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of triangles associated to this FCE part.
    /// </summary>
    public FceTriangle[] Triangles { get; set; } = [];
}
