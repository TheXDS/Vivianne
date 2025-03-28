using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Models.Fe.Nfs4;

/// <summary>
/// Indicates the possible car roof combinations.
/// </summary>
public enum RoofFlag
{
    /// <summary>
    /// The car has a solid, non-removable roof.
    /// </summary>
    [Name("Solid roof")]SolidRoof,

    /// <summary>
    /// The car is a convertible.
    /// </summary>
    Convertible,

    /// <summary>
    /// The car has no roof.
    /// </summary>
    [Name("No roof")]NoRoof
}