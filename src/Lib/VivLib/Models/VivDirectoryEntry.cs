using System.Runtime.InteropServices;
using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a single VIV directory entry.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct VivDirectoryEntry
{
    /// <summary>
    /// Indicates the offset of the file's data.
    /// </summary>
    [Endianness(Endianness.BigEndian)]
    public int Offset;

    /// <summary>
    /// Indicates the size of the file in bytes.
    /// </summary>
    [Endianness(Endianness.BigEndian)]
    public int Length;
}