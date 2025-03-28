using TheXDS.Vivianne.Attributes;

namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Contains useful members for FeData3 and FeData4 files.
/// </summary>
public abstract class FeDataBase
{
    /// <summary>
    /// Enumerates all the known fedata file extensions.
    /// </summary>
    public static readonly string[] KnownExtensions = [".eng", ".bri", ".fre", ".ger", ".ita", ".spa", ".swe"];

    /// <inheritdoc/>
    public string CarId { get; set; } = string.Empty;

    /// <inheritdoc/>
    public ushort SerialNumber { get; set; }

    /// <inheritdoc/>
    public bool IsBonus { get; set; }

    /// <inheritdoc/>
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
}
