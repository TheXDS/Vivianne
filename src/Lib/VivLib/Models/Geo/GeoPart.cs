using TheXDS.Vivianne.Models.Shared;

namespace TheXDS.Vivianne.Models.Geo;

/// <summary>
/// Represents a single .GEO model part.
/// </summary>
public class GeoPart : TridimensionalObjectBase
{
    /// <summary>
    /// Gets or sets the collection of faces that forms this object.
    /// </summary>
    public GeoFace[] Faces { get; set; } = [];

    /// <summary>
    /// Gets or sets the unknown 32-bit value at offset 0x14.
    /// </summary>
    public int Unk_0x14 { get; set; }

    /// <summary>
    /// Gets or sets the unknown 32-bit value at offset 0x18.
    /// </summary>
    public int Unk_0x18 { get; set; }

    /// <summary>
    /// Gets or sets the unknown 64-bit value at offset 0x1c.
    /// </summary>
    public long Unk_0x1C { get; set; }

    /// <summary>
    /// Gets or sets the unknown 64-bit value at offset 0x24.
    /// </summary>
    public long Unk_0x24 { get; set; }

    /// <summary>
    /// Gets or sets the unknown 64-bit value at offset 0x2c.
    /// </summary>
    public long Unk_0x2C { get; set; }
}
