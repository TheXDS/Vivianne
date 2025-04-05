using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents a single FCE part with all of its vertices, triangles, normals
/// and Origin data.
/// </summary>
public class FcePart : FcePartBase<FceTriangle>
{
    /// <summary>
    /// Gets or sets the collection of vertices associated to this FCE part.
    /// </summary>
    public Vector3d[] DamagedVertices { get; set; } = [];

    /// <summary>
    /// Gets an array of the part damaged vertices with a pre-applied origin
    /// transformation.
    /// </summary>
    public Vector3d[] TransformedDamagedVertices => [.. DamagedVertices.Select(Transform)];

    /// <summary>
    /// Gets or sets the collection of normals for vertices associated to this
    /// FCE part.
    /// </summary>
    public Vector3d[] DamagedNormals { get; set; } = [];
}
