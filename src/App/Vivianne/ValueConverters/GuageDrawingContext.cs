using SixLabors.ImageSharp.PixelFormats;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fsh.Nfs3;
using D = System.Drawing;
using M = System.Windows.Media;
namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Uses the gauge data to draw a preview of the gauge cluster dials.
/// </summary>
public class GuageDrawingContext : IOneWayValueConverter<GaugePreviewData?, M.ImageSource?>
{
    /// <inheritdoc/>
    public M.ImageSource? Convert(GaugePreviewData? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;
        var img = new Bitmap(640, 480, PixelFormat.Format32bppArgb);
        var g = Graphics.FromImage(img);
        var color = value.DialColor;
        var width = value.DialWidthBase;
        DrawDial(color, g, width, value.Speedometer, value.PreviewSpeed);
        DrawDial(color, g, width, value.Tachometer, value.PreviewRpm);
        return img.ToSource();
    }

    private static void DrawDial(Bgra32 color, Graphics g, int width, DialData dial, double value)
    {
        var pen = new Pen(D.Color.FromArgb(color.A, color.R, color.G, color.B), width);
        var (center, edge) = GetDialPoints(dial, value);
        g.DrawLine(pen, center, edge);
    }

    private static (D.Point center, D.Point edge) GetDialPoints(DialData dial, double value)
    {
        // TODO: Calculate coordinates with offsets and fill percentage
        return (new D.Point(dial.CenterX, dial.CenterY), new D.Point(dial.MinX, dial.MinY));
    }
}
