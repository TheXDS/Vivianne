using TheXDS.Vivianne.Attributes;
using TheXDS.Vivianne.Models.Fe;

namespace TheXDS.Vivianne.Models.Fe.Nfs4;

/// <summary>
/// Represents a block of car information (FeData) for Need For Speed 4.
/// </summary>
public class FeData : FeDataBase
{
    /// <summary>
    /// Represents the magic header for a valid NFS4 FeData file.
    /// </summary>
    public static readonly byte[] Magic = [0x4];

    /// <summary>
    /// Determines if the given raw data is a valid NFS4 FeData file.
    /// </summary>
    /// <param name="data">Raw data to verify.</param>
    /// <returns>
    /// <see langword="true"/> if the raw data matches the expected magic file
    /// header for a NFS4 FeData file, <see langword="false"/> otherwise.
    /// </returns>
    public static bool IsValid(byte[] data) => data.Length > 0 && data.Take(Magic.Length).SequenceEqual(Magic);

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a police car.
    /// </summary>
    public PursuitFlag PoliceFlag { get; set; }

    /// <summary>
    /// Gets or sets the vehicle performance class onto which this vehicle belongs to.
    /// </summary>
    public CarClass VehicleClass { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle can be upgraded in
    /// carreer mode.
    /// </summary>
    public bool Upgradable { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a convertible.
    /// </summary>
    public bool Convertible { get; set; }

    /// <summary>
    /// Gets or sets the default compare table for this vehicle.
    /// </summary>
    public CompareTable DefaultCompare { get; set; }

    /// <summary>
    /// Gets or sets the first upgrade compare table for this vehicle.
    /// </summary>
    public CompareTable CompareUpg1 { get; set; }

    /// <summary>
    /// Gets or sets the second upgrade compare table for this vehicle.
    /// </summary>
    public CompareTable CompareUpg2 { get; set; }

    /// <summary>
    /// Gets or sets the third upgrade compare table for this vehicle.
    /// </summary>
    public CompareTable CompareUpg3 { get; set; }

    /// <summary>
    /// Gets or sets a string for the "Dynamic Stability" field.
    /// </summary>
    [OffsetTableIndex(40)]
    public string DynamicStability { get; set; } = string.Empty;
}
