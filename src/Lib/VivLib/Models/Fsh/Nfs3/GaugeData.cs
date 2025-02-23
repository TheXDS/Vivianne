using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models.Fsh.Nfs3;

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
    /// Dial data for the speedometer.
    /// </summary>
    public DialData Speedometer;

    /// <summary>
    /// Dial data for the tachometer.
    /// </summary>
    public DialData Tachometer;
}
