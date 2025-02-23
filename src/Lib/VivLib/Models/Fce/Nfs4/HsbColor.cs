using System.Runtime.InteropServices;
using TheXDS.MCART.Math;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents a color in HSB color space, including an alpha component.
/// </summary>
/// <param name="Hue">Color Hue</param>
/// <param name="Saturation">Color saturation.</param>
/// <param name="Brightness">Brightness of the color.</param>
/// <param name="Alpha">Alpha channel.</param>
[StructLayout(LayoutKind.Sequential)]
public record struct HsbColor(byte Hue, byte Saturation, byte Brightness, byte Alpha)
{
    /// <summary>
    /// Converts a <see cref="HsbColor"/> instance to RGBA color space.
    /// </summary>
    /// <returns>A tuple of the RGBA components of this color.</returns>
    public readonly (byte R, byte G, byte B, byte A) ToRgba()
    {
        var (R, G, B) = ToRgb();
        return (R, G, B, Alpha);
    }

    /// <summary>
    /// Converts a <see cref="HsbColor"/> instance to RGB color space.
    /// </summary>
    /// <returns>A tuple of the RGB components of this color.</returns>
    public readonly (byte R, byte G, byte B) ToRgb()
    {
        double h = (Hue / 255.0).Clamp(0.0, 1.0);
        double s = (Saturation / 255.0).Clamp(0.0, 1.0);
        double v = (Brightness / 255.0).Clamp(0.0, 1.0);
        if (s == 0)
        {
            return ((byte)(v * 255), (byte)(v * 255), (byte)(v * 255));
        }

        h *= 6;
        int i = (int)Math.Floor(h);
        double f = h - i;
        double p = v * (1.0 - s);
        double q = v * (1.0 - s * f);
        double t = v * (1.0 - s * (1.0 - f));

        return i switch
        {
            0 => ((byte)(v * 255), (byte)(t * 255), (byte)(p * 255)),
            1 => ((byte)(q * 255), (byte)(v * 255), (byte)(p * 255)),
            2 => ((byte)(p * 255), (byte)(v * 255), (byte)(t * 255)),
            3 => ((byte)(p * 255), (byte)(q * 255), (byte)(v * 255)),
            4 => ((byte)(t * 255), (byte)(p * 255), (byte)(v * 255)),
            _ => ((byte)(v * 255), (byte)(p * 255), (byte)(q * 255)),
        };
    }
}
