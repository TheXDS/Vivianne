namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents an inmutable FCE color set.
/// </summary>
/// <param name="PrimaryColor">Value of the primary color.</param>
/// <param name="SecondaryColor">Value of the secondary color.</param>
public readonly record struct FceColorItem(FceColor PrimaryColor, FceColor SecondaryColor);
