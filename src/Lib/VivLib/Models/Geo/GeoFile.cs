namespace TheXDS.Vivianne.Models.Geo;

/// <summary>
/// Represents a 3D model as used in Need For Speed 2/2SE.
/// </summary>
public class GeoFile
{
    /// <summary>
    /// Gets or sets the magic file signature.
    /// </summary>
    public int MagicNumber { get; set; }

    /// <summary>
    /// Gets or sets the unknown values table at offset 0x04.
    /// </summary>
    public int[] Unk_0x04 { get; set; } = [];

    /// <summary>
    /// Gets or sets the unknown 64-bit value at offset 0x84.
    /// </summary>
    public long Unk_0x84 { get; set; }

    /// <summary>
    /// Gets or sets the collection of parts in this model.
    /// </summary>
    public GeoPart?[] Parts { get; set; } = [];
}
