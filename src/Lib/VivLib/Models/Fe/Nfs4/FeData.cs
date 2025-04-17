using TheXDS.Vivianne.Attributes;
using TheXDS.Vivianne.Models.Fe;

namespace TheXDS.Vivianne.Models.Fe.Nfs4;

/// <summary>
/// Represents a block of car information (FeData) for Need For Speed 4.
/// </summary>
public class FeData : FeDataBase, IFeData
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
    public RoofFlag Roof { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the location of the engine in this
    /// car.
    /// </summary>
    public EngineLocation EngineLocation { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the var is part of a DLC.
    /// </summary>
    public bool IsDlc { get; set; }

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
    [OffsetTableIndex(17)]
    public string DynamicStability { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(18)]
    public string TopSpeed { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(19)]
    public string Accel0To60 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(20)]
    public string Accel0To100 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(21)]
    public string Transmission { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(22)]
    public string Gearbox { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(23)]
    public string History1 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(24)]
    public string History2 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(25)]
    public string History3 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(26)]
    public string History4 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(27)]
    public string History5 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(28)]
    public string History6 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(29)]
    public string History7 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(30)]
    public string History8 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(31)]
    public string Color1 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(32)]
    public string Color2 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(33)]
    public string Color3 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(34)]
    public string Color4 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(35)]
    public string Color5 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(36)]
    public string Color6 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(37)]
    public string Color7 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(38)]
    public string Color8 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(39)]
    public string Color9 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(40)]
    public string Color10 { get; set; } = string.Empty;
}
