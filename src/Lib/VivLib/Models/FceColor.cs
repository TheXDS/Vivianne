using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models;

[StructLayout(LayoutKind.Sequential)]
public record struct FceColor(int Hue, int Saturation, int Brightness, int Transparency)
{
    /// <summary>
    /// Converts a <see cref="FceColor"/> instance to RGB color space.
    /// </summary>
    /// <returns></returns>
    public readonly (int R, int G, int B) ToRgb()
    {
        double red = 0.0;
        double green = 0.0;
        double blue = 0.0;

        if (Saturation == 0)
        {
            red = Brightness;
            green = Brightness;
            blue = Brightness;
        }
        else
        {
            double h = Hue * 6.0;
            if (h == 6.0)
            {
                h = 0.0;
            }

            int i = (int)Math.Floor(h);
            double f = h - i;
            double p = Brightness * (1.0 - Saturation);
            double q = Brightness * (1.0 - Saturation * f);
            double t = Brightness * (1.0 - Saturation * (1.0 - f));

            switch (i)
            {
                case 0:
                    red = Brightness;
                    green = t;
                    blue = p;
                    break;
                case 1:
                    red = q;
                    green = Brightness;
                    blue = p;
                    break;
                case 2:
                    red = p;
                    green = Brightness;
                    blue = t;
                    break;
                case 3:
                    red = p;
                    green = q;
                    blue = Brightness;
                    break;
                case 4:
                    red = t;
                    green = p;
                    blue = Brightness;
                    break;
                case 5:
                    red = Brightness;
                    green = p;
                    blue = q;
                    break;
            }
        }

        return ((int)(red * 255), (int)(green * 255), (int)(blue * 255));
    }
}
