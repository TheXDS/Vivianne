namespace TheXDS.Vivianne.Models.Geo;

/// <summary>
/// Represents a single quad face as used in .GEO 3D models.
/// </summary>
public class GeoFace
{
    private string textureName = new('\0', 4);

    /// <summary>
    /// Gets or sets the material flags to associate with this quad face.
    /// </summary>
    public GeoMaterialFlags MaterialFlags { get; set; }

    /// <summary>
    /// Gets or sets the index of the first vertex of this quad face.
    /// </summary>
    public byte Vertex1 { get; set; }

    /// <summary>
    /// Gets or sets the index of the second vertex of this quad face.
    /// </summary>
    public byte Vertex2 { get; set; }

    /// <summary>
    /// Gets or sets the index of the third vertex of this quad face.
    /// </summary>
    public byte Vertex3 { get; set; }

    /// <summary>
    /// Gets or sets the index of the fourth vertex of this quad face.
    /// </summary>
    public byte Vertex4 { get; set; }

    /// <summary>
    /// Gets or sets the associated texture of this quad face.
    /// </summary>
    /// <remarks>
    /// The texture name should be exactly 4 characters in length.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if you try to set the texture name to a string that does not
    /// have exactly 4 bytes in length.
    /// </exception>
    public string TextureName
    {
        get => textureName;
        set
        {
            if (value.Length != 4) throw new ArgumentException("The texture name should be exactly 4 characters in length.");
            textureName = value;
        }
    }
}
