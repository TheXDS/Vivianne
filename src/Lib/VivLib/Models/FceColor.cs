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

        double red = 0.0;
        double green = 0.0;
        double blue = 0.0;
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

            switch (i)
            {
                case 0:
                    red = b;
                    green = t;
                    blue = p;
                    break;
                case 1:
                    red = q;
                    green = b;
                    blue = p;
                    break;
                case 2:
                    red = p;
                    green = b;
                    blue = t;
                    break;
                case 3:
                    red = p;
                    green = q;
                    blue = b;
                    break;
                case 4:
                    red = t;
                    green = p;
                    blue = b;
                    break;
                case 5:
                    red = b;
                    green = p;
                    blue = q;
                    break;
            }
        }
        return ((int)(red * 255), (int)(green * 255), (int)(blue * 255));
    }
}


