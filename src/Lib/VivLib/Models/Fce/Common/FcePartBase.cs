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
    public Vector3d Origin { get; set; }

    /// <summary>
    /// Gets or sets the collection of vertices associated to this FCE part.
    /// </summary>
    public Vector3d[] Vertices { get; set; } = [];

    /// <summary>
    /// Gets an array of the part vertices with a pre-applied origin
    /// transformation.
    /// </summary>
    public Vector3d[] TransformedVertices => [.. Vertices.Select(Transform)];

    /// <summary>
    /// Gets or sets the collection of normals for vertices associated to this
    /// FCE part.
    /// </summary>
    public Vector3d[] Normals { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of triangles associated to this FCE part.
    /// </summary>
    public T[] Triangles { get; set; } = [];

    protected Vector3d Transform(Vector3d vector)
    {
        return new Vector3d(vector.X + Origin.X, vector.Y + Origin.Y, vector.Z + Origin.Z);
    }
}
