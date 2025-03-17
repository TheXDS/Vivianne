using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models.Bnk;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts an <see cref="int"/> value into a string that represents a
/// human-readable file size in bytes.
/// </summary>
public class ByteSizeConverter : IOneWayValueConverter<int, string>
{
    /// <inheritdoc/>
    public string Convert(int value, object? parameter, CultureInfo? culture)
    {
        return Common.ByteUnits(value);
    }
}

/// <summary>
/// Renders a visual representation of the selected <see cref="BnkBlob"/> into
/// a <see cref="ImageSource"/>.
/// </summary>
public class BnkVisualizerConverter : IOneWayValueConverter<BnkBlob?, ImageSource?>
{
    /// <inheritdoc/>
    public ImageSource? Convert(BnkBlob? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;
        short[] samples = new short[value.SampleData.Length / 2];
        Buffer.BlockCopy(value.SampleData, 0, samples, 0, value.SampleData.Length);

        int maxSample = samples.Select(p => (int)p).Max(Math.Abs);
        double[] normalizedSamples = [.. samples.Select(p => (double)p / maxSample)];

        int width = 1280;
        int height = 300;

        using Bitmap bitmap = new(width, height);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(System.Drawing.Color.FromArgb(40,40,40));
            System.Drawing.Pen pen = new(System.Drawing.Color.DarkOliveGreen, 1);
            System.Drawing.Pen loopPen = new(System.Drawing.Color.DarkBlue, 1);
            var loopStart = value.LoopStart * 2;
            var loopEnd = value.LoopLength + loopStart;

            for (int i = 0; i < normalizedSamples.Length - 1; i++)
            {
                double x1 = (double)i / normalizedSamples.Length * width;
                double x2 = (double)(i + 1) / normalizedSamples.Length * width;
                double y1 = height / 2 - normalizedSamples[i] * height / 2;
                double y2 = height / 2 - normalizedSamples[i + 1] * height / 2;
                graphics.DrawLine(i.IsBetween(loopStart, loopEnd) ? loopPen : pen, (float)x1, (float)y1, (float)x2, (float)y2);
            }
        }

        return bitmap.ToImage();
    }
}