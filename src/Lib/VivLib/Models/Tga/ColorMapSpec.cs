using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models.Tga;

/// <summary>
/// Describes the color map information in a TGA file.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ColorMapSpec
{
    /// <summary>
    /// Offset of the color map.
    /// </summary>
    public ushort ColorMapOffset;

    /// <summary>
    /// Number of elements in the color map.
    /// </summary>
    public ushort ColorMapSize;

    /// <summary>
    /// Bits per color for each element of the color map.
    /// </summary>
    public byte BitsPerColor;
}
