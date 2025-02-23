using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs3;

public partial class FceSerializer
{
    private readonly record struct FceData(in Fce3FileHeader Header, in Vector3d[] Vertices, in Vector3d[] Normals, in FceTriangle[] Triangles);

    [StructLayout(LayoutKind.Sequential)]
    private  struct Fce3FileHeader
    {
        public int Unk_0x0;
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
        public int DummyCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public Vector3d[] Dummies;
        public int CarPartCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public Vector3d[] CarPartsCoords;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleCount;
        public int PrimaryColors;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] PrimaryColorTable;
        public int SecondaryColors;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public HsbColor[] SecondaryColorTable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceAsciiBlob[] DummyNames;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public FceAsciiBlob[] PartNames;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] Unk_0x1e04;
    }
}
