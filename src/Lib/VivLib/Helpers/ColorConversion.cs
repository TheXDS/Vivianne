using TheXDS.Vivianne.Models.Bnk;

namespace TheXDS.Vivianne.Helpers;

/// <summary>
/// Includes helper functions to perform Rendering operations on BNK audio
/// streams, such as conversion to WAV files, etc.
/// </summary>
public static class BnkRender
{
    /// <summary>
    /// Renders an uncompressed, 16-bit PCM BNK blob into a .WAV file.
    /// </summary>
    /// <param name="blob"></param>
    /// <returns></returns>
    public static MemoryStream RenderBnk(BnkBlob blob)
    {
        int fileSize = 36 + blob.SampleData.Length;
        var wavStream = new MemoryStream();
        wavStream.Write("RIFF"u8.ToArray(), 0, 4);
        wavStream.Write(BitConverter.GetBytes(fileSize), 0, 4);
        wavStream.Write("WAVE"u8.ToArray(), 0, 4);
        wavStream.Write("fmt "u8.ToArray(), 0, 4);
        wavStream.Write(BitConverter.GetBytes(16), 0, 4); // size of fmt header
        wavStream.Write(BitConverter.GetBytes((short)1), 0, 2); // format tag (PCM)
        wavStream.Write(BitConverter.GetBytes((short)blob.Channels), 0, 2);
        wavStream.Write(BitConverter.GetBytes(blob.SampleRate), 0, 4);
        wavStream.Write(BitConverter.GetBytes(blob.SampleRate * blob.Channels * 2), 0, 4); // byte rate
        wavStream.Write(BitConverter.GetBytes((short)(blob.Channels * 2)), 0, 2); // block align
        wavStream.Write(BitConverter.GetBytes((short)16), 0, 2); // bits per sample
        wavStream.Write("data"u8.ToArray(), 0, 4);
        wavStream.Write(BitConverter.GetBytes(blob.SampleData.Length), 0, 4);
        wavStream.Write(blob.SampleData, 0, blob.SampleData.Length);
        wavStream.Position = 0;
        return wavStream;
    }
}

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
