using System.Diagnostics;
using System.Numerics;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Geo;
using TheXDS.Vivianne.Extensions;

namespace TheXDS.Vivianne.Serializers.Geo;

public partial class GeoSerializer
{
    private static readonly byte[] BendMarker = [0x42, 0x45, 0x4E, 0x44, 0x00, 0x00];
    private const int NumberOfParts = 32;

    private static void SkipBendMarker(BinaryReader reader)
    {
        /* .GEO is a bit weird. Going by the currently understood spec,
         * a GEO part will always have an even number of vertices, even
         * if they are reused by a quad. As luck may have it, whenever
         * we have this odd number of vertices, there is some kind of
         * "Block End" marker? which, happens to be 6 bytes in length,
         * the same size as a GeoVertex struct.
         * 
         * The content of this marker may be either all zeroes, or
         * '0x42 0x45 0x4E 0x44 0x00 0x00' ("BEND\0\0" in ASCII).
         * Several GEO files will have one or the other, but a few
         * include other "trash?" data here. Still, as far as we know,
         * the data in this area will go unused, so it really doesn't
         * matter what's in it.
         * 
         * We'll have to skip this marker so we can properly read the
         * rest of the file. We can also use it for sanity checks.
         */
        var bendMarker = reader.ReadBytes(6);
        if (!(bendMarker.SequenceEqual(BendMarker) || bendMarker.AreZero()))
        {
            Debug.Print($"[WARNING] Unexpected data found when reading GEO part with odd number of vertices (offset 0x{reader.BaseStream.Position:X8}): {string.Join(" ", bendMarker.Select(p => p.ToString("X2")))}.");
        }
    }

    private static GeoFace ToGeoFace(GeoPolygon polygon) => new()
    {
        MaterialFlags = polygon.MaterialFlags,
        Vertex1 = polygon.Vertex1,
        Vertex2 = polygon.Vertex2,
        Vertex3 = polygon.Vertex3,
        Vertex4 = polygon.Vertex4,
        TextureName = System.Text.Encoding.ASCII.GetString(polygon.TextureName).TrimEnd('\0')
    };

    private static GeoPolygon ToGeoPolygon(GeoFace face) => new()
    {
        MaterialFlags = face.MaterialFlags,
        Vertex1 = face.Vertex1,
        Vertex2 = face.Vertex2,
        Vertex3 = face.Vertex3,
        Vertex4 = face.Vertex4,
        TextureName = System.Text.Encoding.ASCII.GetBytes(face.TextureName).ArrayOfSize(4)
    };

    private static Vector3 ToVector3(GeoVertex v) => new(v.X, v.Y, v.Z);
    
    private static GeoVertex ToGeoVertex(Vector3 v) => new() { X = (short)v.X, Y = (short)v.Y, Z = (short)v.Z };

    private static GeoPart ToGeoPart(GeoPartBlock header, GeoVertex[] vertices, GeoPolygon[] polygons) => new()
    {
        Origin = new Vector3(header.XCoord, header.YCoord, header.ZCoord),
        Vertices = [.. vertices.Select(ToVector3)],
        Faces = [.. polygons.Select(ToGeoFace)],
        Unk_0x14 = header.Unk_0x14,
        Unk_0x18 = header.Unk_0x18,
        Unk_0x1C = header.Unk_0x1C,
        Unk_0x24 = header.Unk_0x24,
        Unk_0x2C = header.Unk_0x2C
    };

    private static GeoPartBlock ToGeoPartBlock(GeoPart part) => new()
    {
        XCoord = part.Origin.X,
        YCoord = part.Origin.Y,
        ZCoord = part.Origin.Z,
        PolyCount = part.Faces.Length,
        VertexCount = part.Vertices.Length,
        Unk_0x14 = part.Unk_0x14,
        Unk_0x18 = part.Unk_0x18,
        Unk_0x1C = part.Unk_0x1C,
        Unk_0x24 = part.Unk_0x24,
        Unk_0x2C = part.Unk_0x2C
    };

    private static GeoPart? ReadGeoPart(BinaryReader reader)
    {
        var geoPartBlockHeader = reader.MarshalReadStruct<GeoPartBlock>();
        if (geoPartBlockHeader.VertexCount == 0 || geoPartBlockHeader.PolyCount == 0) return null;
        var vertices = reader.MarshalReadArray<GeoVertex>(geoPartBlockHeader.VertexCount);
        if (geoPartBlockHeader.VertexCount % 2 != 0)
        {
            SkipBendMarker(reader);
        }
        var polygons = reader.MarshalReadArray<GeoPolygon>(geoPartBlockHeader.PolyCount);
        return ToGeoPart(geoPartBlockHeader, vertices, polygons);
    }

    private static void WriteGeoPart(BinaryWriter writer, GeoPart? part)
    {
        if (part is null)
        {
            writer.MarshalWriteStruct(new GeoPartBlock());
            return;
        }
        writer.MarshalWriteStruct(ToGeoPartBlock(part));
        writer.MarshalWriteStructArray(part.Vertices.Select(ToGeoVertex).ToArray());
        if (part.Vertices.Length % 2 != 0)
        {
            writer.Write(BendMarker);
        }
        writer.MarshalWriteStructArray(part.Faces.Select(ToGeoPolygon).ToArray());
    }

    private static GeoHeader CreateGeoHeader(GeoFile file)
    {
        return new GeoHeader
        {
            MagicNumber = file.MagicNumber,
            Unk_0x04 = file.Unk_0x04.ArrayOfSize(32),
            Unk_0x84 = file.Unk_0x84
        };
    }
}
