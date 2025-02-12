namespace TheXDS.Vivianne.Models;

/// <summary>
/// Indicates the possible values for the pursuit flag of a car in Need For
/// Speed 4.
/// </summary>
public enum Nfs4PursuitFlag : byte
{
    /// <summary>
    /// The car is not a police car.
    /// </summary>
    No = 0x00,
    
    /// <summary>
    /// The car is a police car.
    /// </summary>
    Police = 0x10,

    /// <summary>
    /// The car is not a police car (custom flag version for Mercedes Benz).
    /// </summary>
    NoMercedes = 0x20,

    /// <summary>
    /// The car is not a police car (custom flag version for Ferrari).
    /// </summary>
    NoFerrari = 0xa0,
}