using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Models;

[StructLayout(LayoutKind.Sequential)]
public struct FceFileHeader
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] Unk_0x0;
    public int Triangles;
    public int Vertices;
    public int Arts;
    public int VertexTblOffset;
    public int NormalsTblOffset;
    public int TriangleTblOffset;
    public int Rsvd1Offset;
    public int Rsvd2Offset;
    public int Rsvd3Offset;
    public float XHalfSize;
    public float YHalfSize;
    public float ZHalfSize;
    public int DummyCount; // Up to 16
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public Vector3d[] Dummies;
    public int CarParts; // Up to 64
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public Vector3d[] CarPartsCoords;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexOffset;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexCount;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleOffset;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleCount;
    public int PrimaryColors; // Up to 16
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceColor[] PrimaryColorTable;
    public int SecondaryColors; // Up to 16
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceColor[] SecondaryColorTable;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceAsciiBlob[] DummyNames;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public FceAsciiBlob[] PartNames;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] Unk_0x1e04;
}
