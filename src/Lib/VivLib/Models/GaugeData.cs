using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a block of data with dashboard gauge configuration.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct GaugeData
{
    /// <summary>
    /// First unknown value from the gauge data table.
    /// </summary>
    public int Unk1;

    /// <summary>
    /// Second unknown value from the gauge data table.
    /// </summary>
    public int Unk2;

    /// <summary>
    /// X coord of the dial color reference.
    /// </summary>
    public int DialColorX;

    /// <summary>
    /// Y coord of the dial color reference.
    /// </summary>
    public int DialColorY;

    /// <summary>
    /// Width of the dials on the base.
    /// </summary>
    public int DialWidthBase;

    /// <summary>
    /// Width of the dials on the tip.
    /// </summary>
    public int DialWidthTip;

    /// <summary>
    /// X coord of the center of the speedometer dial.
    /// </summary>
    public int SpeedometerCenterX;

    /// <summary>
    /// Y coord of the center of the speedometer dial.
    /// </summary>
    public int SpeedometerCenterY;

    /// <summary>
    /// Offset to start drawing the speedometer dial from, starting at the
    /// center.
    /// </summary>
    public int SpeedometerCenterOffset;

    /// <summary>
    /// Offset to end drawing the speedometer dial, starting at the edge.
    /// </summary>
    public int SpeedometerEdgeOffset;

    /// <summary>
    /// Minimum value of the speedometer. Should ideally be <c>0</c>.
    /// </summary>
    public int SpeedometerMin;

    /// <summary>
    /// Maximum value of the speedometer.
    /// </summary>
    public int SpeedometerMax;

    /// <summary>
    /// X coord of the minimum indicated speedometer value.
    /// </summary>
    public int SpeedometerMinX;

    /// <summary>
    /// Y coord of the minimum indicated speedometer value.
    /// </summary>
    public int SpeedometerMinY;

    /// <summary>
    /// X coord of the maximum indicated speedometer value.
    /// </summary>
    public int SpeedometerMaxX;

    /// <summary>
    /// Y coord of the maximum indicated speedometer value.
    /// </summary>
    public int SpeedometerMaxY;

    /// <summary>
    /// X coord of the center of the tachometer dial.
    /// </summary>
    public int TachometerCenterX;

    /// <summary>
    /// Y coord of the center of the tachometer dial.
    /// </summary>
    public int TachometerCenterY;
    /// <summary>
    /// Offset to start drawing the tachometer dial from, starting at the
    /// center.
    /// </summary>
    public int TachometerCenterOffset;
    /// <summary>
    /// Offset to end drawing the tachometer dial, starting at the edge.
    /// </summary>
    public int TachometerEdgeOffset;

    /// <summary>
    /// Minimum value of the tachometer. Should ideally be <c>0</c>.
    /// </summary>
    public int TachometerMin;

    /// <summary>
    /// Maximum value of the tachometer.
    /// </summary>
    public int TachometerMax;

    /// <summary>
    /// X coord of the minimum indicated tachometer value.
    /// </summary>
    public int TachometerMinX;

    /// <summary>
    /// Y coord of the minimum indicated tachometer value.
    /// </summary>
    public int TachometerMinY;

    /// <summary>
    /// X coord of the maximum indicated tachometer value.
    /// </summary>
    public int TachometerMaxX;

    /// <summary>
    /// Y coord of the maximum indicated tachometer value.
    /// </summary>
    public int TachometerMaxY;
}