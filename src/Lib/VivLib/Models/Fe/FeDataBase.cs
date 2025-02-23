using TheXDS.Vivianne.Attributes;

namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Contains useful members for FeData3 and FeData4 files.
/// </summary>
public abstract class FeDataBase : IFeData
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


    ///// <summary>
    ///// Gets a FeData deserializer for the given raw data.
    ///// </summary>
    ///// <param name="data">Data to deserialize.</param>
    ///// <returns>
    ///// A FeData deserializer for the given raw data, automatically selecting
    ///// the proper serializer class to use for the file format.
    ///// </returns>
    //public static IOutSerializer<IFeData> GetSerializer(byte[] data)
    //{
    //    return FeData4.IsValid(data) ? new FeData4Serializer() : new FeData3Serializer();
    //}

    ///// <summary>
    ///// Gets a deserialized FeData instance from the given raw data.
    ///// </summary>
    ///// <param name="data">Data to deserialize</param>
    ///// <returns>
    ///// A game-agnostic FeData instance deserialized from the given raw data.
    ///// </returns>
    //public static IFeData GetFeData(byte[] data)
    //{
    //    return GetSerializer(data).Deserialize(data);
    //}
}
