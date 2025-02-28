using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents a single FCE part with all of its vertices, triangles, normals
/// and Origin data.
/// </summary>
public class FcePart: INameable
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
    /// Gets or sets the collection of vertices associated to this FCE part.
    /// </summary>
    public Vector3d[] DamagedVertices { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of normals for vertices associated to this
    /// FCE part.
    /// </summary>
    public Vector3d[] Normals { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of normals for vertices associated to this
    /// FCE part.
    /// </summary>
    public Vector3d[] DamagedNormals { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of triangles associated to this FCE part.
    /// </summary>
    public FceTriangle[] Triangles { get; set; } = [];
}
