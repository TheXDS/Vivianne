using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a block of data with dashboard gauge configuration.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct GaugeData
{
    public int Unk1;
    public int Unk2;
    public int DialColorX;
    public int DialColorY;
    public int DialWidthBase;
    public int DialWidthTip;
    public int SpeedometerCenterX;
    public int SpeedometerCenterY;
    public int SpeedometerCenterOffsetBase;
    public int SpeedometerCenterOffsetTip;
    public int SpeedometerMin;
    public int SpeedometerMax;
    public int SpeedometerMinX;
    public int SpeedometerMinY;
    public int SpeedometerMaxX;
    public int SpeedometerMaxY;
    public int TachometerCenterX;
    public int TachometerCenterY;
    public int TachometerCenterOffsetBase;
    public int TachometerCenterOffsetTip;
    public int TachometerMin;
    public int TachometerMax;
    public int TachometerMinX;
    public int TachometerMinY;
    public int TachometerMaxX;
    public int TachometerMaxY;
}