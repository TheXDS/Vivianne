using System.Runtime.InteropServices;
using TheXDS.MCART.Math;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs3;

/// <summary>
/// Represents a color in HSB color space, including an alpha component.
/// </summary>
/// <param name="Hue">Color Hue</param>
/// <param name="Saturation">Color saturation.</param>
/// <param name="Brightness">Brightness of the color.</param>
/// <param name="Alpha">Alpha channel.</param>
[StructLayout(LayoutKind.Sequential)]
public record struct HsbColor(int Hue, int Saturation, int Brightness, int Alpha) : IHsbColor
{
    readonly byte IHsbColor.Alpha => (byte)Alpha.Clamp(0, 255);

    readonly byte IHsbColor.Brightness => (byte)Brightness.Clamp(0, 255);

    readonly byte IHsbColor.Hue => (byte)Hue.Clamp(0, 255);

    readonly byte IHsbColor.Saturation => (byte)Saturation.Clamp(0, 255);

    /// <summary>
    /// Converts a <see cref="HsbColor"/> instance to RGBA color space.
    /// </summary>
    /// <returns>A tuple of the RGBA components of this color.</returns>
    public readonly (int R, int G, int B, int A) ToRgba()
    {
        var (R, G, B) = ToRgb();
        return (R, G, B, Alpha);
    }

    /// <summary>
    /// Converts a <see cref="HsbColor"/> instance to RGB color space.
    /// </summary>
    /// <returns>A tuple of the RGB components of this color.</returns>
    public readonly (int R, int G, int B) ToRgb()
    {
        return ColorConversion.ToRgb((byte)Hue.Clamp(255), (byte)Saturation.Clamp(255), (byte)Brightness.Clamp(255));
    }
}
