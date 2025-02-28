using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs4;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs4;

/// <summary>
/// Represents the header of an FCE4/FCE4M file.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FceFileHeader
{
    /// <summary>
    /// Gets the magic file signature for the FCE file.
    /// </summary>
    /// <remarks>
    /// A value of 0x00101014 indicates a regular FCE4 file, whereas 0x00101015 indicates a special FCE4M file.
    /// </remarks>
    public int Magic;

    /// <summary>
    /// Gets a 4-byte nullable value at offset 0x4 whose purpose is currently
    /// unknown.
    /// </summary>
    public int Unk_0x4;

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
    /// For NFS4, this value should remain at <c>1</c>, unless the model supports non-zero texture pages (like, Cop models, certain special objects, etc.).
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
    /// Gets the offset of the global vertex table for the undamaged model
    /// relative to the length of the FCE header.
    /// </summary>
    /// <remarks>
    /// This should be a copy of the <see cref="VertexTblOffset"/> value.
    /// </remarks>
    public int UndamagedVertexTblOffset;

    /// <summary>
    /// Gets the offset of the global normals table for the undamaged model
    /// relative to the length of the FCE header.
    /// </summary>
    /// <remarks>
    /// This should be a copy of the <see cref="NormalsTblOffset"/> value.
    /// </remarks>
    public int UndamagedNormalsTblOffset;

    /// <summary>
    /// Gets the offset of the global vertex table for the damaged model
    /// relative to the length of the FCE header.
    /// </summary>
    public int DamagedVertexTblOffset;

    /// <summary>
    /// Gets the offset of the global normals table for the damaged model
    /// relative to the length of the FCE header.
    /// </summary>
    public int DamagedNormalsTblOffset;

    /// <summary>
    /// Gets the offset of the fourth reserved data table relative to the
    /// length of the FCE header.
    /// </summary>
    public int Rsvd4Offset;

    /// <summary>
    /// Gets the offset of the global animation data table relative to the
    /// length of the FCE header.
    /// </summary>
    public int AnimationTblOffset;

    /// <summary>
    /// Gets the offset of the fifth reserved data table relative to the length
    /// of the FCE header.
    /// </summary>
    public int Rsvd5Offset;

    /// <summary>
    /// Gets the offset of the sixth reserved data table relative to the length
    /// of the FCE header.
    /// </summary>
    public int Rsvd6Offset;

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
    /// format for the color names.
    /// </remarks>
    public int Colors;

    /// <summary>
    /// Gets a 16-element table containing all the defined primary colors for
    /// the model.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 16 <see cref="HsbColor"/>
    /// elements, each representing a single primary color. The correct number
    /// of primary colors is indicated by <see cref="Colors"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] PrimaryColorTable;

    /// <summary>
    /// Gets a 16-element table containing all the defined interior colors for
    /// the model.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 16 <see cref="HsbColor"/>
    /// elements, each representing a single interior color. The correct number
    /// of interior colors is indicated by <see cref="Colors"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] InteriorColorTable;

    /// <summary>
    /// Gets a 16-element table containing all the defined secondary colors for
    /// the model.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 16 <see cref="HsbColor"/>
    /// elements, each representing a single secondary color. The correct
    /// number of secondary colors is indicated by <see cref="Colors"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] SecondaryColorTable;

    /// <summary>
    /// Gets a 16-element table containing all the defined driver hair colors
    /// for the model.
    /// </summary>
    /// <remarks>
    /// This table will have a static size of 16 <see cref="HsbColor"/>
    /// elements, each representing a driver hair color. The correct number
    /// of driver hair colors is indicated by <see cref="Colors"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] DriverColorTable;

    /// <summary>
    /// Gets a 4-byte nullable value at offset 0x0924 whose purpose is
    /// currently unknown.
    /// </summary>
    public int Unk_0x0924;

    /// <summary>
    /// Gets a 256-byte table with data whose purpose is currently unknown.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] Unk_0x0928;

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
    /// Gets a 64-element table containing the names for all the car parts.
    /// </summary>
    /// <remarks>
    /// This table will contain a static size of 64 <see cref="FceAsciiBlob"/>
    /// elements, each representing a null-terminated string with the name for
    /// each existing car part. The correct number of existing car parts is
    /// indicated by <see cref="CarPartCount"/>.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public FceAsciiBlob[] PartNames;

    /// <summary>
    /// Gets a 528-byte table with data whose purpose is currently unknown.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 528)] public byte[] Unk_0x1e04;
}
