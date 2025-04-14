namespace TheXDS.Vivianne.Models.Fce.Nfs3;

/// <summary>
/// Represents a mutable class equivalent to a car color set, including the
/// primary and secondary colors.
/// </summary>
/// <param name="PrimaryColor">Reference to the primary color.</param>
/// <param name="SecondaryColor">Reference to the secondary color.</param>
public record class MutableFceColorItem(MutableFceColor PrimaryColor, MutableFceColor SecondaryColor);
