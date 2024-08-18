using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the header of an FCE file.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FceFileHeader
{
    /// <summary>
    /// Gets the first 4 bytes from the FCE header.
    /// </summary>
    /// <remarks>
    /// Might be a magic header, but NFS3 will happily load an FCE file where
    /// these bytes are set to <c>0</c>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] Unk_0x0;

    /// <summary>
    /// Gets the triangle count for the entire model.
    /// </summary>
    /// <remarks>
    /// This value is used to load a global triangle table, from where FCE
    /// parts may then select their own triangles.
    /// </remarks>
    public int Triangles;

    /// <summary>
    /// Gets the vertex count for the entire model.
    /// </summary>
    /// <remarks>
    /// This value is used to load a global vertex table, from where FCE parts
    /// may then select their own vertices.
    /// </remarks>
    public int Vertices;

    /// <summary>
    /// Gets a value that indicates the number of arts contained in the FCE
    /// file.
    /// </summary>
    /// <remarks>
    /// For NFS3, this value should remain at <c>1</c> for a valid FCE file.
    /// </remarks>
    public int Arts;

    /// <summary>
    /// Gets the offset of the global vertex table relative to the length of
    /// the FCE header.
    /// </summary>
    public int VertexTblOffset;

    /// <summary>
    /// Gets the offset of the global normals table relative to the length of
    /// the FCE header.
    /// </summary>
    public int NormalsTblOffset;

    /// <summary>
    /// Gets the offset of the global triangle table relative to the length of
    /// the FCE header.
    /// </summary>
    public int TriangleTblOffset;

    /// <summary>
    /// Gets the offset of the first reserved data table relative to the length
    /// of the FCE header.
    /// </summary>
    public int Rsvd1Offset;

    /// <summary>
    /// Gets the offset of the second reserved data table relative to the
    /// length of the FCE header.
    /// </summary>
    public int Rsvd2Offset;

    /// <summary>
    /// Gets the offset of the third reserved data table relative to the length
    /// of the FCE header.
    /// </summary>
    public int Rsvd3Offset;

    /// <summary>
    /// Gets a value that, when multiplied by <c>2.0</c>, indicates the size of
    /// the model in the X axis. 
    /// </summary>
    public float XHalfSize;

    /// <summary>
    /// Gets a value that, when multiplied by <c>2.0</c>, indicates the size of the model in the Y axis. 
    /// </summary>
    public float YHalfSize;

    /// <summary>
    /// Gets a value that, when multiplied by <c>2.0</c>, indicates the size of the model in the Z axis. 
    /// </summary>
    public float ZHalfSize;

    /// <summary>
    /// Gets a value that indicates the total number of Dummies in the FCE file.
    /// </summary>
    /// <remarks>This value should never exceed <c>16</c>.</remarks>
    public int DummyCount;

    /// <summary>
    /// Gets the entire Dummy vector table.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 16 <see cref="Vector3d"/>
    /// elements. The correct number of existing Dummies is indicated by
    /// <see cref="DummyCount"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public Vector3d[] Dummies;

    /// <summary>
    /// Gets a value that indicates the total number of car parts in the FCE file.
    /// </summary>
    /// <remarks>This value should never exceed <c>64</c>.</remarks>
    public int CarPartCount;

    /// <summary>
    /// Gets the entire car part origin vector table.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 64 <see cref="Vector3d"/>
    /// elements. The correct number of existing car part origin elements is
    /// indicated by <see cref="CarPartCount"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public Vector3d[] CarPartsCoords;

    /// <summary>
    /// Gets a 64-element table with the offsets for each car part in the
    /// global vertex table.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 64 <see cref="int"/> offsets,
    /// each pointing to the index of the first vertex in the global vertex
    /// table for the car part referenced by the index of the element. The
    /// correct number of existing car part vertex table offset elements is
    /// indicated by <see cref="CarPartCount"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexOffset;

    /// <summary>
    /// Gets a 64-element table with number of vertices to load for each part
    /// from the global vertex table.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 64 <see cref="int"/> offsets,
    /// each pointing to the index of the first vertex in the global vertex
    /// table for the car part referenced by the index of the element. The
    /// correct number of existing car part vertex table offset elements is
    /// indicated by <see cref="CarPartCount"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexCount;

    /// <summary>
    /// Gets a 64-element table with the offsets for each car part in the
    /// global triangle table.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 64 <see cref="int"/> offsets,
    /// each pointing to the index of the first triangle in the global triangle
    /// table for the car part referenced by the index of the element. The
    /// correct number of existing car part triangle table offset elements is
    /// indicated by <see cref="CarPartCount"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleOffset;

    /// <summary>
    /// Gets a 64-element table with number of triangles to load for each part
    /// from the global triangle table.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 64 <see cref="int"/> offsets,
    /// each pointing to the index of the first triangle in the global triangle
    /// table for the car part referenced by the index of the element. The
    /// correct number of existing car part triangle table offset elements is
    /// indicated by <see cref="CarPartCount"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleCount;

    /// <summary>
    /// Gets the number of primary colors in the primary color table.
    /// </summary>
    /// <remarks>
    /// This value should never exceed <c>16</c>, and its recommended to avoid
    /// exceeding <c>10</c> due to limitations in the related FeData file
    /// format for the color names, and should match the value in
    /// <see cref="SecondaryColors"/>.
    /// </remarks>
    public int PrimaryColors;

    /// <summary>
    /// Gets a 16-element table containing all the defined primary colors for
    /// the car.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 16 <see cref="FceColor"/>
    /// elements, each representing a single primary color. The correct number
    /// of primary colors is indicated by <see cref="PrimaryColors"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceColor[] PrimaryColorTable;

    /// <summary>
    /// Gets the number of secondary colors in the secondary color table.
    /// </summary>
    /// <remarks>
    /// This value should never exceed <c>16</c>, and its recommended to avoid
    /// exceeding <c>10</c> due to limitations in the related FeData file
    /// format for the color names, and should match the value in
    /// <see cref="PrimaryColors"/>.
    /// </remarks>
    public int SecondaryColors;

    /// <summary>
    /// Gets a 16-element table containing all the defined secondary colors for
    /// the car.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 16 <see cref="FceColor"/>
    /// elements, each representing a single secondary color. The correct
    /// number of secondary colors is indicated by
    /// <see cref="SecondaryColors"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceColor[] SecondaryColorTable;

    /// <summary>
    /// Gets a 16-element table containing the names for all the Dummies.
    /// </summary>
    /// <remarks>
    /// This table will contain a static size of 16 <see cref="FceAsciiBlob"/>
    /// elements, each representing a null-terminated string with the name for
    /// each existing Dummy. The correct number of existing Dummies is
    /// indicated by <see cref="DummyCount"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceAsciiBlob[] DummyNames;

    /// <summary>
    /// Gets a 16-element table containing the names for all the car parts.
    /// </summary>
    /// <remarks>
    /// This table will contain a static size of 16 <see cref="FceAsciiBlob"/>
    /// elements, each representing a null-terminated string with the name for
    /// each existing car part. The correct number of existing car parts is
    /// indicated by <see cref="CarPartCount"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public FceAsciiBlob[] PartNames;

    /// <summary>
    /// Gets a 64-byte table with data whose purpose is currently unknown.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] Unk_0x1e04;
}
