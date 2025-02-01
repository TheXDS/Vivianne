using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;
using M = System.Windows.Media;
using St = TheXDS.Vivianne.Resources.Strings.ValueConverters.GraphDrawingContext;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Generates a graph for a given curve.
/// </summary>
public class GraphDrawingContext : IOneWayValueConverter<IEnumerable<double>, M.ImageSource?>
{
    /// <inheritdoc/>
    public M.ImageSource? Convert(IEnumerable<double> value, object? parameter, CultureInfo? culture)
    {
        parameter ??= new Size(150, 50);
        if (parameter is not Size sz) throw new ArgumentException(string.Format(St.InvalidParameter, typeof(Size).FullName));
        var values = value.ToArray();
        var min = values.Min();
        var max = values.Max();
        var hstep = (float)sz.Width / values.Length;
        var img = new Bitmap(sz.Width, sz.Height, PixelFormat.Format24bppRgb);
        var g = Graphics.FromImage(img);
        g.FillRectangle(Brushes.Black, new Rectangle(0, 0, sz.Width, sz.Height));
        var color = Pens.Red;
        var points = values.WithIndex().Select(p => new PointF(p.index * hstep, (float)((p.element - min) * sz.Height / (max - min)))).ToArray();
        g.DrawLines(color, points);
        return img.ToSource();
    }
}