using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Models.Fe.Nfs4;

/// <summary>
/// Indicates the possible values for the pursuit flag of a car in Need For
/// Speed 4.
/// </summary>
public enum PursuitFlag : byte
{
    /// <summary>
    /// The car is not a police car.
    /// </summary>
    No = 0x00,

    /// <summary>
    /// The car is a police car.
    /// </summary>
    [Name("Yes")] Police = 0x10,

    /// <summary>
    /// The car is not a police car (custom flag version for Mercedes Benz).
    /// </summary>
    [Name("No (Mercedes)")] NoMercedes = 0x20,

    /// <summary>
    /// The car is not a police car (custom flag version for Ferrari).
    /// </summary>
    [Name("No (Ferrari)")] NoFerrari = 0xa0,
}
