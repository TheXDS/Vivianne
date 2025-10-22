using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Geo;
using TheXDS.Vivianne.Serializers.Misc;

namespace TheXDS.Vivianne.Serializers.Geo;

// TODO: This parser is partially broken - origins for objects are not being loaded properly somehow.
public partial class GeoSerializer
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private struct GeoHeader
    {
        public int MagicNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public int[] Unk_0x04;
        public long Unk_0x84;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private struct GeoPartBlock
    {
        public int VertexCount;
        public int PolyCount;
        public FixedPointDecimal32 XCoord;
        public FixedPointDecimal32 YCoord;
        public FixedPointDecimal32 ZCoord;
        public int Unk_0x14;
        public int Unk_0x18;
        public long Unk_0x1C; // = 0
        public long Unk_0x24; // = 1
        public long Unk_0x2C; // = 1

        public GeoPartBlock()
        {
            Unk_0x24 = 1;
            Unk_0x2C = 1;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    private struct GeoVertex
    {
        public short X;
        public short Y;
        public short Z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct GeoPolygon
    {
        public GeoMaterialFlags MaterialFlags;
        public byte Vertex1;
        public byte Vertex2;
        public byte Vertex3;
        public byte Vertex4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] TextureName;
    }
}
