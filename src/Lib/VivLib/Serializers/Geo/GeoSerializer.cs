using System.Diagnostics;
using System.Numerics;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Geo;

namespace TheXDS.Vivianne.Serializers.Geo;

public partial class GeoSerializer : ISerializer<GeoFile>
{
    private static readonly byte[] BendMarker = [0x42, 0x45, 0x4E, 0x44, 0x00, 0x00];

    public GeoFile Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var header = reader.MarshalReadStruct<GeoHeader>();
        List<GeoPart> parts = [];
        for (int i = 0; i < 32; i++)
        {
            var geoPartBlockHeader = reader.MarshalReadStruct<GeoPartBlock>();
            if (geoPartBlockHeader.VertexCount == 0 || geoPartBlockHeader.PolyCount == 0) continue;
            var vertices = reader.MarshalReadArray<GeoVertex>(geoPartBlockHeader.VertexCount);
            if (geoPartBlockHeader.VertexCount % 2 != 0)
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
                    Debug.Print($"[WARNING] Unexpected data found when reading GEO part with odd number of vertices (offset 0x{stream.Position:X8}): {string.Join(" ",bendMarker.Select(p => p.ToString("X2")))}.");
                }
            }
            var polygons = reader.MarshalReadArray<GeoPolygon>(geoPartBlockHeader.PolyCount);
            parts.Add(new GeoPart
            {
                Origin = new Vector3(geoPartBlockHeader.XCoord, geoPartBlockHeader.YCoord, geoPartBlockHeader.ZCoord),
                Vertices = [.. vertices.Select(v => new Vector3(v.X, v.Y, v.Z))],
                Faces = [.. polygons.Select(p => new GeoFace
                {
                    MaterialFlags = p.MaterialFlags,
                    Vertex1 = p.Vertex1,
                    Vertex2 = p.Vertex2,
                    Vertex3 = p.Vertex3,
                    Vertex4 = p.Vertex4,
                    TextureName = System.Text.Encoding.ASCII.GetString(p.TextureName).TrimEnd('\0')
                })],
                Unk_0x14 = geoPartBlockHeader.Unk_0x14,
                Unk_0x18 = geoPartBlockHeader.Unk_0x18,
                Unk_0x1C = geoPartBlockHeader.Unk_0x1C,
                Unk_0x24 = geoPartBlockHeader.Unk_0x24,
                Unk_0x2C = geoPartBlockHeader.Unk_0x2C
            });
        }
        return new GeoFile
        {
            MagicNumber = header.MagicNumber,
            Unk_0x04 = header.Unk_0x04,
            Unk_0x84 = header.Unk_0x84,
            Parts = parts
        };
    }

    public void SerializeTo(GeoFile entity, Stream stream)
    {

        throw new NotImplementedException();
    }
}
