using System.Numerics;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Represents a single FCE part with all of its vertices, triangles, normals
/// and Origin data.
/// </summary>
public class FcePartBase<T> : INameable where T : IFceTriangle
{
    /// <summary>
    /// Gets or sets the name of the FCE part.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the FCE part origin.
    /// </summary>
    public Vector3 Origin { get; set; }

    /// <summary>
    /// Gets or sets the collection of vertices associated to this FCE part.
    /// </summary>
    public Vector3[] Vertices { get; set; } = [];

    /// <summary>
    /// Gets an array of the part vertices with a pre-applied origin
    /// transformation.
    /// </summary>
    public Vector3[] TransformedVertices => [.. Vertices.Select(Transform)];

    /// <summary>
    /// Gets or sets the collection of normals for vertices associated to this
    /// FCE part.
    /// </summary>
    public Vector3[] Normals { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of triangles associated to this FCE part.
    /// </summary>
    public T[] Triangles { get; set; } = [];

    /// <summary>
    /// Performs vector transformations based on the object's origin.
    /// </summary>
    /// <param name="vector">Vector to transform.</param>
    /// <returns>A transformed vector in respect to the object's origin.</returns>
    protected Vector3 Transform(Vector3 vector)
    {
        return vector + Origin;
    }
}
