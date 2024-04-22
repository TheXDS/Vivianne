namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a single FCE part with all of its vertices, triangles, normals
/// and Origin data.
/// </summary>
public class FcePart
{
    /// <summary>
    /// Gets or sets the FCE part origin.
    /// </summary>
    public Vector3d Origin { get; set; }

    /// <summary>
    /// Gets or sets the collection of vertices associated to this FCE part.
    /// </summary>
    public Vector3d[] Vertices { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of normals for vertices associated to this
    /// FCE part.
    /// </summary>
    public Vector3d[] Normals { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of triangles associated to this FCE part.
    /// </summary>
    public Triangle[] Triangles { get; set; } = [];
}
