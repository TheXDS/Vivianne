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
        double b = (Brightness / 255.0).Clamp(0.0, 1.0);
        double red;
        double green;
        double blue;
        if (s == 0)
        {
            red = b;
            green = b;
            blue = b;
        }
        else
        {
            h *= 6.0;
            int i = (int)Math.Floor(h);
            double f = h - i;
            double p = b * (1.0 - s);
            double q = b * (1.0 - s * f);
            double t = b * (1.0 - s * (1.0 - f));
            (red, green, blue) = i switch
            {
                0 => (b, t, p),
                1 => (q, b, p),
                2 => (p, b, t),
                3 => (p, q, b),
                4 => (t, p, b),
                5 => (b, p, q),
                _ => throw new InvalidOperationException()
            };
        }
        return ((int)(red * 255), (int)(green * 255), (int)(blue * 255));
    }
}
