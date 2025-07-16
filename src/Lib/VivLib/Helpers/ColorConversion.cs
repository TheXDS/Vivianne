namespace TheXDS.Vivianne.Helpers;

/// <summary>
/// Includes methods to perform color conversion between different color
/// spaces.
/// </summary>
public static class ColorConversion
{
    /// <summary>
    /// Converts a color from HSB color space to RGB color space.
    /// </summary>
    /// <returns>A tuple of the RGB components of this color.</returns>
    public static (int R, int G, int B) ToRgb(in byte hue, in byte saturation, in byte brightness)
    {
        double h = (hue / 255.0);
        double s = (saturation / 255.0);
        double v = (brightness / 255.0);
        if (s == 0)
        {
            return ((int)(v * 255), (int)(v * 255), (int)(v * 255));
        }

        h *= 6;
        int i = (int)Math.Floor(h);
        double f = h - i;
        double p = v * (1.0 - s);
        double q = v * (1.0 - (s * f));
        double t = v * (1.0 - (s * (1.0 - f)));

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
