using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Geo;

namespace TheXDS.Vivianne.Serializers.Geo;

/// <summary>
/// Implements a serializer for .GEO 3D models.
/// </summary>
public partial class GeoSerializer : ISerializer<GeoFile>
{
    /// <inheritdoc/>
    public GeoFile Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var header = reader.MarshalReadStruct<GeoHeader>();
        List<GeoPart?> parts = [];
        for (int i = 0; i < NumberOfParts; i++)
        {
            parts.Add(ReadGeoPart(reader));
        }
        return new GeoFile
        {
            MagicNumber = header.MagicNumber,
            Unk_0x04 = header.Unk_0x04,
            Unk_0x84 = header.Unk_0x84,
            Parts = [.. parts]
        };
    }

    /// <inheritdoc/>
    public void SerializeTo(GeoFile entity, Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        writer.MarshalWriteStruct(CreateGeoHeader(entity));
        foreach (var j in entity.Parts)
        {
            WriteGeoPart(writer, j);
        }
    }
}
