using System.Numerics;
using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Shared;

namespace TheXDS.Vivianne.Models.Fce.Common;

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
    public MaterialFlags Flags;

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

    /// <summary>
    /// Gets a vector that represents the UV coordinates for the first vertex.
    /// </summary>
    public readonly Vector2 Uv1 => new(U1, V1);

    /// <summary>
    /// Gets a vector that represents the UV coordinates for the second vertex.
    /// </summary>
    public readonly Vector2 Uv2 => new(U2, V2);

    /// <summary>
    /// Gets a vector that represents the UV coordinates for the third vertex.
    /// </summary>
    public readonly Vector2 Uv3 => new(U3, V3);
}
