using TheXDS.Vivianne.Attributes;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a block of car information (FeData) for Need For Speed 4.
/// </summary>
public class FeData4 : IFeData
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

    /// <inheritdoc/>
    public string CarId { get; set; } = string.Empty;

    /// <inheritdoc/>
    public ushort SerialNumber { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a police car.
    /// </summary>
    public Nfs4PursuitFlag PoliceFlag { get; set; }

    /// <summary>
    /// Gets or sets the vehicle performance class onto which this vehicle belongs to.
    /// </summary>
    public Nfs4CarClass VehicleClass { get; set; }

    /// <inheritdoc/>
    public bool IsBonus { get; set; }

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
    public Nfs4CompareTable DefaultCompare { get; set; }

    /// <summary>
    /// Gets or sets the first upgrade compare table for this vehicle.
    /// </summary>
    public Nfs4CompareTable CompareUpg1 { get; set; }

    /// <summary>
    /// Gets or sets the second upgrade compare table for this vehicle.
    /// </summary>
    public Nfs4CompareTable CompareUpg2 { get; set; }

    /// <summary>
    /// Gets or sets the third upgrade compare table for this vehicle.
    /// </summary>
    public Nfs4CompareTable CompareUpg3 { get; set; }

    /// <summary>
    /// Gets or sets the number of string entries present in the file.
    /// </summary>
    /// <remarks>
    /// For a valid NFS4 FeData file, This value must always be equal to 41.
    /// </remarks>
    public ushort StringEntries { get; set; }

    /// <inheritdoc/>
    [OffsetTableIndex(0)]
    public string Manufacturer { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(1)]
    public string Model { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(2)]
    public string CarName { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(3)]
    public string Price { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(4)]
    public string Status { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(5)]
    public string Weight { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(6)]
    public string WeightDistribution { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(7)]
    public string Length { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(8)]
    public string Width { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(9)]
    public string Height { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(10)]
    public string Engine { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(11)]
    public string Displacement { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(12)]
    public string Hp { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(13)]
    public string Torque { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(14)]
    public string MaxEngineSpeed { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(15)]
    public string Brakes { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(16)]
    public string Tires { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(17)]
    public string TopSpeed { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(18)]
    public string Accel0To60 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(19)]
    public string Accel0To100 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(20)]
    public string Transmission { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(21)]
    public string Gearbox { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(22)]
    public string History1 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(23)]
    public string History2 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(24)]
    public string History3 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(25)]
    public string History4 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(26)]
    public string History5 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(27)]
    public string History6 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(28)]
    public string History7 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(29)]
    public string History8 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(30)]
    public string Color1 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(31)]
    public string Color2 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(32)]
    public string Color3 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(33)]
    public string Color4 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(34)]
    public string Color5 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(35)]
    public string Color6 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(36)]
    public string Color7 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(37)]
    public string Color8 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(38)]
    public string Color9 { get; set; } = string.Empty;

    /// <inheritdoc/>
    [OffsetTableIndex(39)]
    public string Color10 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the "Dynamic Stability" field.
    /// </summary>
    [OffsetTableIndex(40)]
    public string DynamicStability { get; set; } = string.Empty;
}
