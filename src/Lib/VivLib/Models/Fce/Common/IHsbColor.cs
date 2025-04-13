namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Defines an interface for a color in HSBA color space that can expose its
/// component values as discrete byte values.
/// </summary>
public interface IHsbColor
{
    /// <summary>
    /// Gets the alpha channel for the color.
    /// </summary>
    byte Alpha { get; }
    
    /// <summary>
    /// Gets the Brightness value for the color.
    /// </summary>
    byte Brightness { get; }

    /// <summary>
    /// Gets the color hue value for the color.
    /// </summary>
    byte Hue { get; }

    /// <summary>
    /// Gets the saturation value for the color.
    /// </summary>
    byte Saturation { get; }

    /// <summary>
    /// Converts this instance to RGBA color space.
    /// </summary>
    /// <returns>A tuple of the RGBA components of this color.</returns>
    (int R, int G, int B, int A) ToRgba();
}