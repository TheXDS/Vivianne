using System.Runtime.InteropServices;
using TheXDS.MCART.Math;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a color in HSB color space, including an alpha component.
/// </summary>
/// <param name="Hue">Color Hue</param>
/// <param name="Saturation">Color saturation.</param>
/// <param name="Brightness">Brightness of the color.</param>
/// <param name="Alpha">Alpha channel.</param>
[StructLayout(LayoutKind.Sequential)]
public record struct FceColor(int Hue, int Saturation, int Brightness, int Alpha)
{
    /// <summary>
    /// Converts a <see cref="FceColor"/> instance to RGBA color space.
    /// </summary>
    /// <returns>A tuple of the RGBA components of this color.</returns>
    public readonly (int R, int G, int B, int A) ToRgba()
    {
        var (R, G, B) = ToRgb();
        return (R, G, B, Alpha);
    }

    /// <summary>
    /// Converts a <see cref="FceColor"/> instance to RGB color space.
    /// </summary>
    /// <returns>A tuple of the RGB components of this color.</returns>
    public readonly (int R, int G, int B) ToRgb()
    {
        double h = (Hue / 255.0).Clamp(0.0, 1.0);
        double s = (Saturation / 255.0).Clamp(0.0, 1.0);
        double v = (Brightness / 255.0).Clamp(0.0, 1.0);
        if (s == 0)
        {
            return ((int)(v * 255), (int)(v * 255), (int)(v * 255));
        }

        h *= 6;
        int i = (int)Math.Floor(h);
        double f = h - i;
        double p = v * (1.0 - s);
        double q = v * (1.0 - s * f);
        double t = v * (1.0 - s * (1.0 - f));

        return i switch
        {
            0 => ((int)(v * 255), (int)(t * 255), (int)(p * 255)),
            1 => ((int)(q * 255), (int)(v * 255), (int)(p * 255)),
            2 => ((int)(p * 255), (int)(v * 255), (int)(t * 255)),
            3 => ((int)(p * 255), (int)(q * 255), (int)(v * 255)),
            4 => ((int)(t * 255), (int)(p * 255), (int)(v * 255)),
            _ => ((int)(v * 255), (int)(p * 255), (int)(q * 255)),
        };
    }
}
