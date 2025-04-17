using TheXDS.Vivianne.Attributes;

namespace TheXDS.Vivianne.Models.Fe.Nfs3;

/// <summary>
/// Represents a block of car information (FeData) for Need For Speed 3.
/// </summary>
public class FeData : FeDataBase, IFeData
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

    /// <summary>
    /// Gets or sets the vehicle performance class onto which this vehicle belongs to.
    /// </summary>
    public CarClass VehicleClass { get; set; }

    /// <summary>
    /// Gets or sets the driver seat position for this vehicle.
    /// </summary>
    public DriverSeatPosition Seat { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a police car.
    /// </summary>
    public bool IsPolice { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the car is part of a DLC (downloadable content) package.
    /// </summary>
    public ushort IsDlc { get; set; }

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
