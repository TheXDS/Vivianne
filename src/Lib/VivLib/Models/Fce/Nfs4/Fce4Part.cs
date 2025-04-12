using System.Numerics;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents a extended FCE part that includes additional data sets used by FCE4.
/// </summary>
public class Fce4Part : FcePart
{
    /// <summary>
    /// Gets or sets the collection of vertices associated to this FCE part.
    /// </summary>
    public Vector3[] DamagedVertices { get; set; } = [];

    /// <summary>
    /// Gets an array of the part damaged vertices with a pre-applied origin
    /// transformation.
    /// </summary>
    public Vector3[] TransformedDamagedVertices => [.. DamagedVertices.Select(Transform)];

    /// <summary>
    /// Gets or sets the collection of normals for vertices associated to this
    /// FCE part.
    /// </summary>
    public Vector3[] DamagedNormals { get; set; } = [];
}
