namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Defines a set of properties that are common to all FeData files.
/// </summary>
public interface IFeData : ISerialNumberModel
{
    /// <summary>
    /// Gets or sets the four character car id.
    /// </summary>
    string CarId { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a bonus car.
    /// </summary>
    bool IsBonus { get; set; }

    /// <summary>
    /// Gets or sets the number of string entries present in the file.
    /// </summary>
    ushort StringEntries { get; set; }

    /// <summary>
    /// Gets or sets a string for the manufacturer of this vehicle.
    /// </summary>
    string Manufacturer { get; set; }

    /// <summary>
    /// Gets or sets a string for the model of this vehicle.
    /// </summary>
    string Model { get; set; }

    /// <summary>
    /// Gets or sets a string for the car name of this vehicle.
    /// </summary>
    string CarName { get; set; }

    /// <summary>
    /// Gets or sets a string for the sale price of this vehicle.
    /// </summary>
    string Price { get; set; }

    /// <summary>
    /// Gets or sets a string for the engine displacement of this vehicle.
    /// </summary>
    string Displacement { get; set; }

    /// <summary>
    /// Gets or sets a string for the engine description for this vehicle.
    /// </summary>
    string Engine { get; set; }

    /// <summary>
    /// Gets or sets a string for the height of this vehicle.
    /// </summary>
    string Height { get; set; }

    /// <summary>
    /// Gets or sets a string for the horsepower rating of this vehicle.
    /// </summary>
    string Hp { get; set; }

    /// <summary>
    /// Gets or sets a string for the length of this vehicle.
    /// </summary>
    string Length { get; set; }

    /// <summary>
    /// Gets or sets a string for the production status of this vehicle.
    /// </summary>
    string Status { get; set; }

    /// <summary>
    /// Gets or sets a string for the weight of this vehicle.
    /// </summary>
    string Weight { get; set; }

    /// <summary>
    /// Gets or sets a string for the weight distribution of this vehicle.
    /// </summary>
    string WeightDistribution { get; set; }

    /// <summary>
    /// Gets or sets a string for the width of this vehicle.
    /// </summary>
    string Width { get; set; }

    /// <summary>
    /// Gets or sets a string for the 0 to 100 MPH acceleration time of this vehicle.
    /// </summary>
    string Accel0To100 { get; set; }

    /// <summary>
    /// Gets or sets a string for the 0 to 60 MPH acceleration time of this vehicle.
    /// </summary>
    string Accel0To60 { get; set; }

    /// <summary>
    /// Gets or sets a string for the brakes of this vehicle.
    /// </summary>
    string Brakes { get; set; }

    /// <summary>
    /// Gets or sets a string for the gearbox of this vehicle.
    /// </summary>
    string Gearbox { get; set; }

    /// <summary>
    /// Gets or sets a string for the maximum engine RPM of this vehicle.
    /// </summary>
    string MaxEngineSpeed { get; set; }

    /// <summary>
    /// Gets or sets a string for the tires of this vehicle.
    /// </summary>
    string Tires { get; set; }

    /// <summary>
    /// Gets or sets a string for the top speed of this vehicle.
    /// </summary>
    string TopSpeed { get; set; }

    /// <summary>
    /// Gets or sets a string for the max torque of this vehicle.
    /// </summary>
    string Torque { get; set; }

    /// <summary>
    /// Gets or sets a string for the transmission of this vehicle.
    /// </summary>
    string Transmission { get; set; }

    /// <summary>
    /// Gets or sets a string for the first history line of this vehicle.
    /// </summary>
    string History1 { get; set; }

    /// <summary>
    /// Gets or sets a string for the second history line of this vehicle.
    /// </summary>
    string History2 { get; set; }

    /// <summary>
    /// Gets or sets a string for the third history line of this vehicle.
    /// </summary>
    string History3 { get; set; }

    /// <summary>
    /// Gets or sets a string for the fourth history line of this vehicle.
    /// </summary>
    string History4 { get; set; }

    /// <summary>
    /// Gets or sets a string for the fifth history line of this vehicle.
    /// </summary>
    string History5 { get; set; }

    /// <summary>
    /// Gets or sets a string for the sixth history line of this vehicle.
    /// </summary>
    string History6 { get; set; }

    /// <summary>
    /// Gets or sets a string for the seventh history line of this vehicle.
    /// </summary>
    string History7 { get; set; }

    /// <summary>
    /// Gets or sets a string for the eighth history line of this vehicle.
    /// </summary>
    string History8 { get; set; }

    /// <summary>
    /// Gets or sets a string for the first color description of this vehicle.
    /// </summary>
    string Color1 { get; set; }

    /// <summary>
    /// Gets or sets a string for the second color description of this vehicle.
    /// </summary>
    string Color2 { get; set; }

    /// <summary>
    /// Gets or sets a string for the third color description of this vehicle.
    /// </summary>
    string Color3 { get; set; }

    /// <summary>
    /// Gets or sets a string for the fourth color description of this vehicle.
    /// </summary>
    string Color4 { get; set; }

    /// <summary>
    /// Gets or sets a string for the fifth color description of this vehicle.
    /// </summary>
    string Color5 { get; set; }

    /// <summary>
    /// Gets or sets a string for the sixth color description of this vehicle.
    /// </summary>
    string Color6 { get; set; }

    /// <summary>
    /// Gets or sets a string for the seventh color description of this vehicle.
    /// </summary>
    string Color7 { get; set; }

    /// <summary>
    /// Gets or sets a string for the eighth color description of this vehicle.
    /// </summary>
    string Color8 { get; set; }

    /// <summary>
    /// Gets or sets a string for the ninth color description of this vehicle.
    /// </summary>
    string Color9 { get; set; }

    /// <summary>
    /// Gets or sets a string for the tenth color description of this vehicle.
    /// </summary>
    string Color10 { get; set; }
}