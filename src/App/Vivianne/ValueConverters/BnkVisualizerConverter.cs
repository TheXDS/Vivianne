using System.Drawing;
using System.Globalization;
using System.Windows.Media;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Misc;
using TheXDS.Vivianne.Models.Audio.Bnk;

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
        using Bitmap bitmap = new(width, height);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(System.Drawing.Color.FromArgb(0,0,0,0));

            System.Drawing.Pen[] channelPens = [
                new(System.Drawing.Color.DarkOliveGreen, 1),
                new(System.Drawing.Color.DarkOrange, 1)
                ];
            System.Drawing.Pen[] loopPens = [
                new(System.Drawing.Color.SkyBlue, 1),
                new(System.Drawing.Color.SteelBlue, 1)
                ];
            System.Drawing.Pen gridPen = new(System.Drawing.Color.LightGray);
            System.Drawing.Pen loopStartPen = new(System.Drawing.Color.Red);
            var loopStart = value.LoopStart / value.Channels;
            var loopEnd = value.LoopEnd / value.Channels;
            graphics.DrawLine(gridPen, 0, 512, 1024, 512);
            foreach ((var currentChannelIndex, var currentChannelData) in Enumerable.Range(0, value.Channels).Select(p => GetChannelData(normalizedSamples, value.Channels, p)).WithIndex())
            {
                var currentPen = channelPens[currentChannelIndex];
                var loopPen = loopPens[currentChannelIndex];
                for (int i = 0; i < currentChannelData.Length - 1; i++)
                {
                    double x1 = (double)i / currentChannelData.Length * width;
                    double x2 = (double)(i + 1) / currentChannelData.Length * width;
                    double y1 = (height / 2) - (currentChannelData[i] * height / 2);
                    double y2 = (height / 2) - (currentChannelData[i + 1] * height / 2);
                    if ((i == loopStart || i == loopEnd) && loopStart < loopEnd && loopEnd != 0)
                    {
                        graphics.DrawLine(gridPen, (float)x1, 0, (float)x1, 1024);
                    }
                    graphics.DrawLine(i.IsBetween(loopStart, loopEnd) ? loopPen : currentPen, (float)x1, (float)y1, (float)x2, (float)y2);
                }
            }
        }
        return bitmap.ToImage();
    }

    private static double[] GetChannelData(double[] rawData, int channels, int channel)
    {
        return [.. rawData.Slice(channels).ElementAt(channel)];
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
