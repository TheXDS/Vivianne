using System.Numerics;
using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs4;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs4;

public partial class FceSerializer
{
    private readonly record struct FceData(in FceFileHeader Header, in Vector3[] Vertices, in Vector3[] DamagedVertices, in Vector3[] Normals, in Vector3[] DamagedNormals, in FceTriangle[] Triangles);

    [StructLayout(LayoutKind.Sequential)]
    private struct FceFileHeader
    {
        public int Magic;
        public int Unk_0x4;
        public int Triangles;
        public int Vertices;
        public int Arts;
        public int VertexTblOffset;
        public int NormalsTblOffset;
        public int TriangleTblOffset;
        public int Rsvd1Offset;
        public int Rsvd2Offset;
        public int Rsvd3Offset;
        public int UndamagedVertexTblOffset;
        public int UndamagedNormalsTblOffset;
        public int DamagedVertexTblOffset;
        public int DamagedNormalsTblOffset;
        public int Rsvd4Offset;
        public int AnimationTblOffset;
        public int Rsvd5Offset;
        public int Rsvd6Offset;
        public float XHalfSize;
        public float YHalfSize;
        public float ZHalfSize;
        public int DummyCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public Vector3[] Dummies;
        public int CarPartCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public Vector3[] CarPartsCoords;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleCount;
        public int Colors;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] PrimaryColorTable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] InteriorColorTable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] SecondaryColorTable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] DriverColorTable;
        public int Unk_0x0924;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] Unk_0x0928;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceAsciiBlob[] DummyNames;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public FceAsciiBlob[] PartNames;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 528)] public byte[] Unk_0x1e28;
    }
}