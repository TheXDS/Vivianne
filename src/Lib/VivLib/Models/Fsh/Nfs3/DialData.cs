using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models.Fsh.Nfs3;

/// <summary>
/// Represents the data for a single dial in the gauge cluster.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct DialData
{
    /// <summary>
    /// X coord of the center of the dial.
    /// </summary>
    public int CenterX;

    /// <summary>
    /// Y coord of the center of the dial.
    /// </summary>
    public int CenterY;

    /// <summary>
    /// Offset to start drawing the dial from, starting at the
    /// center.
    /// </summary>
    public int CenterOffset;

    /// <summary>
    /// Offset to end drawing the dial, starting at the edge.
    /// </summary>
    public int EdgeOffset;

    /// <summary>
    /// Minimum indicated value of the dial. Should ideally be <c>0</c>.
    /// </summary>
    public int Min;

    /// <summary>
    /// Maximum indicated value of the dial.
    /// </summary>
    public int Max;

    /// <summary>
    /// X coord of the minimum indicated value.
    /// </summary>
    public int MinX;

    /// <summary>
    /// Y coord of the minimum indicated value.
    /// </summary>
    public int MinY;

    /// <summary>
    /// X coord of the maximum indicated value.
    /// </summary>
    public int MaxX;

    /// <summary>
    /// Y coord of the maximum indicated value.
    /// </summary>
    public int MaxY;
}