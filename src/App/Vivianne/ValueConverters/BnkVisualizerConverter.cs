using System.Drawing;
using System.Globalization;
using System.Windows.Media;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Misc;
using TheXDS.Vivianne.Models.Bnk;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Renders a visual representation of the selected <see cref="BnkStream"/> into
/// a <see cref="ImageSource"/>.
/// </summary>
public class BnkVisualizerConverter : IOneWayValueConverter<BnkStream?, ImageSource?>
{
    const int width = 1024;
    const int height = 1024;

    /// <inheritdoc/>
    public ImageSource? Convert(BnkStream? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;
        double[] normalizedSamples = ParseAudioStream(value.SampleData, value.BytesPerSample * 8);
#if !EnableBnkCompression
        if (value.Compression)
        {
            return null;
        }
#endif
        using Bitmap bitmap = new(width, height);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(System.Drawing.Color.FromArgb(0,0,0,0));
            System.Drawing.Pen pen1 = new(System.Drawing.Color.DarkOliveGreen, 1);
            System.Drawing.Pen pen2 = new(System.Drawing.Color.DarkOrange, 1);
            System.Drawing.Pen loopPen = new(System.Drawing.Color.SkyBlue, 1);
            System.Drawing.Pen gridPen = new(System.Drawing.Color.LightGray);
            System.Drawing.Pen loopStartPen = new(System.Drawing.Color.Red);
            var loopStart = value.LoopStart;
            var loopEnd = value.LoopEnd;
            graphics.DrawLine(gridPen, 0, 512, 1024, 512);
            for (int i = 0; i < normalizedSamples.Length - 1; i++)
            {
                double x1 = (double)i / normalizedSamples.Length * width;
                double x2 = (double)(i + 1) / normalizedSamples.Length * width;
                double y1 = height / 2 - normalizedSamples[i] * height / 2;
                double y2 = height / 2 - normalizedSamples[i + 1] * height / 2;
                if ((i == loopStart || i == loopEnd) && loopStart < loopEnd && loopEnd != 0)
                {
                    graphics.DrawLine(gridPen, (float)x1, 0, (float)x1, 1024);
                }
                graphics.DrawLine(i.IsBetween(loopStart, loopEnd) ? loopPen : pen1, (float)x1, (float)y1, (float)x2, (float)y2);
            }
        }
        return bitmap.ToImage();
    }

    private static double[] ParseAudioStream(byte[] rawData, int bits)
    {
        return bits switch
        {
            16 => Parse16bitStream(rawData),
            8 => Parse8bitStream(rawData),
            _ => [],
        };
    }

    private static double[] Parse8bitStream(byte[] rawData)
    {
        var samples = CommonHelpers.MaptoSByte(rawData);
        int maxSample = sbyte.MaxValue;
        return [.. samples.Select(p => (double)p / maxSample)];

    }
    private static double[] Parse16bitStream(byte[] rawData)
    {
        short[] samples = new short[rawData.Length / 2];
        Buffer.BlockCopy(rawData, 0, samples, 0, rawData.Length);
        int maxSample = (int)Math.Pow(2, 16) / 2;
        return [.. samples.Select(p => (double)p / maxSample)];
    }

}
