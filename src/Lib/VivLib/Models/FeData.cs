using TheXDS.Vivianne.Attributes;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a block of car information (FeData)
/// </summary>
public class FeData
{
    /// <summary>
    /// Enumerates all the known fedata file extensions.
    /// </summary>
    public static readonly string[] KnownExtensions = [".bri", ".eng", ".fre", ".ger", ".ita", ".spa", ".swe"];

    /// <summary>
    /// Gets or sets the four character car id.
    /// </summary>
    public string CarId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serial number for the car.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a bonus car.
    /// </summary>
    public bool IsBonus { get; set; }

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
    /// Gets or sets a string for the manufacturer of this vehicle.
    /// </summary>
    [OffsetTableIndex(0)]
    public string Manufacturer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the model of this vehicle.
    /// </summary>
    [OffsetTableIndex(1)]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the car name of this vehicle.
    /// </summary>
    [OffsetTableIndex(2)]
    public string CarName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the sale price of this vehicle.
    /// </summary>
    [OffsetTableIndex(3)]
    public string Price { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the production status of this vehicle.
    /// </summary>
    [OffsetTableIndex(4)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the weight of this vehicle.
    /// </summary>
    [OffsetTableIndex(5)]
    public string Weight { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the weight distribution of this vehicle.
    /// </summary>
    [OffsetTableIndex(6)]
    public string WeightDistribution { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the length of this vehicle.
    /// </summary>
    [OffsetTableIndex(7)]
    public string Length { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the width of this vehicle.
    /// </summary>
    [OffsetTableIndex(8)]
    public string Width { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the height of this vehicle.
    /// </summary>
    [OffsetTableIndex(9)]
    public string Height { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the engine description for this vehicle.
    /// </summary>
    [OffsetTableIndex(10)]
    public string Engine { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the engine displacement of this vehicle.
    /// </summary>
    [OffsetTableIndex(11)]
    public string Displacement { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the horsepower rating of this vehicle.
    /// </summary>
    [OffsetTableIndex(12)]
    public string Hp { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the max torque of this vehicle.
    /// </summary>
    [OffsetTableIndex(13)]
    public string Torque { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the maximum engine RPM of this vehicle.
    /// </summary>
    [OffsetTableIndex(14)]
    public string MaxEngineSpeed { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the brakes of this vehicle.
    /// </summary>
    [OffsetTableIndex(15)]
    public string Brakes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the tires of this vehicle.
    /// </summary>
    [OffsetTableIndex(16)]
    public string Tires { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the top speed of this vehicle.
    /// </summary>
    [OffsetTableIndex(17)]
    public string TopSpeed { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the 0 to 60 MPP acceleration time of this vehicle.
    /// </summary>
    [OffsetTableIndex(18)]
    public string Accel0To60 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the 0 to 100 MPP acceleration time of this vehicle.
    /// </summary>
    [OffsetTableIndex(19)]
    public string Accel0To100 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the transmission of this vehicle.
    /// </summary>
    [OffsetTableIndex(20)]
    public string Transmission { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the gearbox of this vehicle.
    /// </summary>
    [OffsetTableIndex(21)]
    public string Gearbox { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the first history line of this vehicle.
    /// </summary>
    [OffsetTableIndex(22)]
    public string History1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the second history line of this vehicle.
    /// </summary>
    [OffsetTableIndex(23)]
    public string History2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the third history line of this vehicle.
    /// </summary>
    [OffsetTableIndex(24)]
    public string History3 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the fourth history line of this vehicle.
    /// </summary>
    [OffsetTableIndex(25)]
    public string History4 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the fifth history line of this vehicle.
    /// </summary>
    [OffsetTableIndex(26)]
    public string History5 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the sixth history line of this vehicle.
    /// </summary>
    [OffsetTableIndex(27)]
    public string History6 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the seventh history line of this vehicle.
    /// </summary>
    [OffsetTableIndex(28)]
    public string History7 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the eighth history line of this vehicle.
    /// </summary>
    [OffsetTableIndex(29)]
    public string History8 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the first color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(30)]
    public string Color1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the second color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(31)]
    public string Color2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the third color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(32)]
    public string Color3 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the fourth color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(33)]
    public string Color4 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the fifth color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(34)]
    public string Color5 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the sixth color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(35)]
    public string Color6 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the seventh color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(36)]
    public string Color7 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the eighth color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(37)]
    public string Color8 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the ninth color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(38)]
    public string Color9 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string for the tenth color description of this vehicle.
    /// </summary>
    [OffsetTableIndex(39)]
    public string Color10 { get; set; } = string.Empty;
}
