using System.Numerics;

namespace TheXDS.Vivianne.Models.Shared;

/// <summary>
/// Defines a set of basic properties for tridimensional objects.
/// </summary>
public class TridimensionalObjectBase
{
    /// <summary>
    /// Gets or sets the 3D object origin.
    /// </summary>
    public Vector3 Origin { get; set; }

    /// <summary>
    /// Gets or sets the collection of vertices associated to this 3D object.
    /// </summary>
    public Vector3[] Vertices { get; set; } = [];

    /// <summary>
    /// Gets an array of the part vertices with a pre-applied origin
    /// transformation.
    /// </summary>
    public Vector3[] TransformedVertices => [.. Vertices.Select(Transform)];

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
