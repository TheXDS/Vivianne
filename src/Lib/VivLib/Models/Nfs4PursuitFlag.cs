namespace TheXDS.Vivianne.Models;

/// <summary>
/// Indicates the possible values for the pursuit flag of a car in Need For
/// Speed 4.
/// </summary>
public enum Nfs4PursuitFlag : ushort
{
    /// <summary>
    /// The car is not a police car.
    /// </summary>
    No,
    
    /// <summary>
    /// The car is a police car.
    /// </summary>
    Police,

    /// <summary>
    /// The car is not a police car (custom flag version for Mercedes Benz).
    /// </summary>
    NoMercedes,

    /// <summary>
    /// The car is not a police car (custom flag version for Ferrari).
    /// </summary>
    NoFerrari,
}