using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TheXDS.Vivianne.Extensions;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a block of data used to preview gauge data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GaugePreviewData"/> class.
/// </remarks>
/// <param name="state"></param>
public class GaugePreviewData(GaugeDataState state)
{
    private readonly GaugeDataState state = state;
    private readonly ImageFrame<Bgra32> frame = state.Cabin.ToImage(null)!.CloneAs<Bgra32>().Frames[0];

    /// <summary>
    /// Gets the color used to draw the dials.
    /// </summary>
    public Bgra32 DialColor => GetGaugeColor(state.DialColorX, state.DialColorY);

    /// <summary>
    /// Gets the dial data associated with the speedometer.
    /// </summary>
    public DialData Speedometer => state.BackingStore.Speedometer;

    /// <summary>
    /// Gets the dial data associated with the tachometer.
    /// </summary>
    public DialData Tachometer => state.BackingStore.Tachometer;

    public int DialWidthBase => state.DialWidthBase;

    /// <summary>
    /// Gets the currently indicated speed.
    /// </summary>
    public int PreviewSpeed => state.PreviewSpeed;

    /// <summary>
    /// Gets the currently indicated RPM.
    /// </summary>
    public int PreviewRpm => state.PreviewRpm;

    private Bgra32 GetGaugeColor(int x, int y)
    {
        return Color.FromPixel(frame[x, y]);
    }
}
