using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Enumerates the possible material flags for a triangle.
/// </summary>
[Flags]
public enum TriangleFlags : int
{
    /// <summary>
    /// No flags. Rendered as semi-glossy textured material.
    /// </summary>
    None,
    /// <summary>
    /// No blending. Rendered as matte textured material.
    /// </summary>
    NoBlending = 1,
    /// <summary>
    /// High blending. Rendered as high-gloss textured material.
    /// </summary>
    HighBlending = 2,
    /// <summary>
    /// No Culling. Triangles will be drawn on both faces.
    /// </summary>
    NoCulling = 4,
    /// <summary>
    /// Semi transparent. Rendered as semi-transparent, semi-glossy textured material.
    /// </summary>
    Semitrans = 8,
    /// <summary>
    /// Semi transparent. Rendered as semi-transparent, matte textured material.
    /// </summary>
    SemitransNoBlending = 9,
    /// <summary>
    /// Semi transparent. Rendered as semi-transparent, high-gloss textured material.
    /// </summary>
    SemitransHighBlending = 10,
    /// <summary>
    /// No culling, no blending.
    /// </summary>
    NcNoBlending = 1 | NoCulling,
    /// <summary>
    /// No culling, high blending.
    /// </summary>
    NcHighBlending = 2 | NoCulling,
    /// <summary>
    /// No culling, semi-transparent.
    /// </summary>
    NcSemitrans = 8 | NoCulling,
    /// <summary>
    /// No culling, semi-transparent, no blending.
    /// </summary>
    NcSemitransNoBlending = 9 | NoCulling,
    /// <summary>
    /// No culling, semi-transparent, high blending.
    /// </summary>
    NcSemitransHighBlending = 10 | NoCulling,
}

/// <summary>
/// Represents a single triangle as defined in the FCE format.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Triangle
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
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public short[] Unk_0x10;

    /// <summary>
    /// Gets a 32-bit flagset that indicates properties for the triangle.
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
