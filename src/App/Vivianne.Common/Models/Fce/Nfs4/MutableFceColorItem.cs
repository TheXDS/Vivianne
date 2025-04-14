namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents a mutable class equivalent to a car color set, including the
/// primary and secondary colors.
/// </summary>
/// <param name="PrimaryColor">Reference to the primary color.</param>
/// <param name="InteriorColor">Reference to the interior color.</param>
/// <param name="SecondaryColor">Reference to the secondary color.</param>
/// <param name="DriverHairColor">Reference to the driver's hair color.</param>
public record class MutableFceColorItem(MutableFceColor PrimaryColor, MutableFceColor InteriorColor, MutableFceColor SecondaryColor, MutableFceColor DriverHairColor);
