using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents a single triangle as defined in the FCE format.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FceTriangle
{
    /// <summary>
    /// Gets the texture page associated with this triangle.
    /// </summary>
    public int TexturePage;

    /// <summary>
    /// Gets the index of the first vertex associated with this triangle.
    /// </summary>
    public int I1;

    /// <summary>
    /// Gets the index of the second vertex associated with this triangle.
    /// </summary>
    public int I2;

    /// <summary>
    /// Gets the index of the third vertex associated with this triangle.
    /// </summary>
    public int I3;

    /// <summary>
    /// Gets a 12-byte table with unknown values (<see cref="short"/> data type
    /// presumed).
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public byte[] Unk_0x10;

    /// <summary>
    /// Gets a flagset that indicates properties for the triangle.
    /// </summary>
    public TriangleFlags Flags;

    /// <summary>
    /// Gets the U component of the UV coordinates for the first vertex of this triangle.
    /// </summary>
    public float U1;

    /// <summary>
    /// Gets the U component of the UV coordinates for the second vertex of this triangle.
    /// </summary>
    public float U2;

    /// <summary>
    /// Gets the U component of the UV coordinates for the third vertex of this triangle.
    /// </summary>
    public float U3;

    /// <summary>
    /// Gets the V component of the UV coordinates for the first vertex of this triangle.
    /// </summary>
    public float V1;

    /// <summary>
    /// Gets the V component of the UV coordinates for the second vertex of this triangle.
    /// </summary>
    public float V2;

    /// <summary>
    /// Gets the V component of the UV coordinates for the third vertex of this triangle.
    /// </summary>
    public float V3;
}
