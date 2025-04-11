namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Represents a single triangle to be used for rendering after all material
/// and vertex transformations are applied.
/// </summary>
public struct RenderTriangle
{
    /// <summary>
    /// Gets the texture page associated with this triangle.
    /// </summary>
    public int TexturePage;

    /// <summary>
    /// Gets the index of the first vertex associated with this triangle.
    /// </summary>
    public int I1;

    /// <summary>
    /// Gets the index of the second vertex associated with this triangle.
    /// </summary>
    public int I2;

    /// <summary>
    /// Gets the index of the third vertex associated with this triangle.
    /// </summary>
    public int I3;

    /// <summary>
    /// Gets the material flags for this triangle.
    /// </summary>
    public MaterialFlags Material;

    /// <summary>
    /// Gets the U component of the UV coordinates for the first vertex of this triangle.
    /// </summary>
    public float U1;

    /// <summary>
    /// Gets the U component of the UV coordinates for the second vertex of this triangle.
    /// </summary>
    public float U2;

    /// <summary>
    /// Gets the U component of the UV coordinates for the third vertex of this triangle.
    /// </summary>
    public float U3;

    /// <summary>
    /// Gets the V component of the UV coordinates for the first vertex of this triangle.
    /// </summary>
    public float V1;

    /// <summary>
    /// Gets the V component of the UV coordinates for the second vertex of this triangle.
    /// </summary>
    public float V2;

    /// <summary>
    /// Gets the V component of the UV coordinates for the third vertex of this triangle.
    /// </summary>
    public float V3;
}
