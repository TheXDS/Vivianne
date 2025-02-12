using TheXDS.Vivianne.Attributes;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a block of car information (FeData) for Need For Speed 3.
/// </summary>
public class FeData3 : IFeData
{
    #region Unknown values

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x0c</c>.
    /// </summary>
    public ushort Unk_0x0c { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x14</c>.
    /// </summary>
    public ushort Unk_0x14 { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x16</c>.
    /// </summary>
    public ushort Unk_0x16 { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x1a</c>.
    /// </summary>
    public ushort Unk_0x1a { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x1c</c>.
    /// </summary>
    public ushort Unk_0x1c { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x1e</c>.
    /// </summary>
    public ushort Unk_0x1e { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x20</c>.
    /// </summary>
    public ushort Unk_0x20 { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x22</c>.
    /// </summary>
    public ushort Unk_0x22 { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x24</c>.
    /// </summary>
    public ushort Unk_0x24 { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x26</c>.
    /// </summary>
    public ushort Unk_0x26 { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>BYTE</c> value at offset <c>0x2c</c>.
    /// </summary>
    public byte Unk_0x2c { get; set; }

    #endregion

    /// <inheritdoc/>
    public string CarId { get; set; } = string.Empty;

    /// <inheritdoc/>
    public ushort SerialNumber { get; set; }

    /// <summary>
    /// Gets or sets the vehicle performance class onto which this vehicle belongs to.
    /// </summary>
    public Nfs3CarClass VehicleClass { get; set; }

    /// <summary>
    /// Gets or sets the driver seat position for this vehicle.
    /// </summary>
    public DriverSeatPosition Seat { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a police car.
    /// </summary>
    public bool IsPolice { get; set; }

    /// <inheritdoc/>
    public bool IsBonus { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the car is part of a DLC (downloadable content) package.
    /// </summary>
    public ushort IsDlcCar { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is available to AI drivers.
    /// </summary>
    public bool AvailableToAi { get; set; }

    /// <summary>
    /// Gets or sets the acceleration compare value for this vehicle.
    /// </summary>
    public byte CarAccel { get; set; }

    /// <summary>
    /// Gets or sets the top speed compare value for this vehicle.
    /// </summary>
    public byte CarTopSpeed { get; set; }

    /// <summary>
    /// Gets or sets the car handling compare value for this vehicle.
    /// </summary>
    public byte CarHandling { get; set; }

    /// <summary>
    /// Gets or sets the braking compare value for this vehicle.
    /// </summary>
    public byte CarBraking { get; set; }

    /// <summary>
    /// Gets or sets the number of string entries present in the file.
    /// </summary>
    /// <remarks>
    /// For a valid NFS3 FeData file, This value must always be equal to 40.
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
}
