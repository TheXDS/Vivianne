using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Describes the dimension, origin and format information of the image.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ImageSpec
{
    /// <summary>
    /// X origin.
    /// </summary>
    public ushort XOrigin;

    /// <summary>
    /// Y origin.
    /// </summary>
    public ushort YOrigin;

    /// <summary>
    /// Image width.
    /// </summary>
    public ushort Width;

    /// <summary>
    /// Image height.
    /// </summary>
    public ushort Height;

    /// <summary>
    /// Bits per pixel for TrueColor images.
    /// </summary>
    public byte BitsPerPixel;

    /// <summary>
    /// Descriptor that indicates pixel ordering and alpha channel depth.
    /// </summary>
    public byte PixelFormatDescriptor;
}
