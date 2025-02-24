using TheXDS.Vivianne.Models.Fce;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents an inmutable FCE color set for NFS4 cars.
/// </summary>
/// <param name="PrimaryColor">Value of the primary color.</param>
/// <param name="SecondaryColor">Value of the secondary color.</param>
/// <param name="Interior">Value of the interior color.</param>
/// <param name="Driver">Value of the driver's hair color.</param>
public record Fce4Color(HsbColor PrimaryColor, HsbColor SecondaryColor, HsbColor Interior, HsbColor Driver);
