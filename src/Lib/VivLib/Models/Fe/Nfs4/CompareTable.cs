namespace TheXDS.Vivianne.Models.Fe.Nfs4;

/// <summary>
/// Represents a single compare table for a vehicle in Need For Speed 4.
/// </summary>
/// <param name="Acceleration">
/// Acceleration value. It must be between 0 and 20.
/// </param>
/// <param name="TopSpeed">
/// Top speed value. It must be between 0 and 20.
/// </param>
/// <param name="Handling">Handling value. It must be between 0 and 20.</param>
/// <param name="Braking">Braking value. It must be between 0 and 20.</param>
/// <param name="Overall">Overall value. It must be between 0 and 20.</param>
/// <param name="Price">
/// Price. For the default compare table, indicates the car value. For upgrade
/// tables, indicates the price to upgrade the current car to this performance
/// level.
/// </param>
public record struct CompareTable(byte Acceleration, byte TopSpeed, byte Handling, byte Braking, byte Overall, int Price);
