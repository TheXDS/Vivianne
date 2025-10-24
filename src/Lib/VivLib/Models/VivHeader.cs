using System.Runtime.InteropServices;
using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Defines a the header in a VIV file.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct VivHeader
{
    /// <summary>
    /// Indicates the 4-byte magic file signature.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Magic;

    /// <summary>
    /// Indicates the File length as stored on the header.
    /// </summary>
    /// <remarks>
    /// Probably used for either memory allocation or quick size sanity
    /// checkup.
    /// </remarks>
    [Endianness(Endianness.BigEndian)]
    public int VivLength;

    /// <summary>
    /// Indicates the number of files contained in the VIV file.
    /// </summary>
    [Endianness(Endianness.BigEndian)]
    public int Entries;

    /// <summary>
    /// Indicates the location of the data pool.
    /// </summary>
    [Endianness(Endianness.BigEndian)]
    public int PoolOffset;
}
