using TheXDS.Vivianne.Models.Fe;

namespace TheXDS.Vivianne.Models.Fe.Nfs3;

/// <summary>
/// Represents a block of car information (FeData) for Need For Speed 3.
/// </summary>
public class FeData : FeDataBase
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
}
