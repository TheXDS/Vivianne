using System.Drawing;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Represents a single Car color, including its primary and secondary colors,
/// as well as the name of the color.
/// </summary>
/// <param name="Name">Name of the color.</param>
/// <param name="Primary">Primary color.</param>
/// <param name="Secondary">Secondary color.</param>
public record class CarColorItem(string Name, Color Primary, Color Secondary);