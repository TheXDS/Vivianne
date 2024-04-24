namespace TheXDS.Vivianne.Models;

/// <summary>
/// Enumerates the possible engine locations in a vehicle.
/// </summary>
public enum EngineLocation : ushort
{
    /// <summary>
    /// The engine is located at the front.
    /// </summary>
    Front,
    /// <summary>
    /// The engine is located in the middle.
    /// </summary>
    Mid,
    /// <summary>
    /// The engine is located on the rear.
    /// </summary>
    Rear,
}