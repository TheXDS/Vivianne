using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

/// <summary>
/// Implements a serializer that can read and write entities of type
/// <see cref="MapFile"/>.
/// </summary>
public class MapSerializer : ISerializer<MapFile>
{
    private const string Magic = "PFDx";

    /// <inheritdoc/>
    public MapFile Deserialize(Stream stream)
    {
        using BinaryReader reader = new(stream);

        var header = reader.MarshalReadStruct<MapFileHeader>();
        var sections = reader.MarshalReadArray<MapFileSection>(header.NumberOfSections);
        byte[][] unkData = new byte[header.NumRecords][];
        for (int i = 0; i < header.NumRecords; i++)
        {
            unkData[i] = reader.ReadBytes(header.RecordSize);
        }
        var offsets = reader.MarshalReadArray<int>(header.NumberOfSections).Select(p => p.FlipEndianness()).ToArray();

        List<MapItem> items = [];

        foreach (var (index, j) in sections.WithIndex())
        {
            items.Add(new()
            {
                MusOffset = offsets[index],
                Jumps = [.. j.Records.Take(j.NumRecords).Select(ToJump)]
            });
        }
        return new()
        {
            Unk_0x04 = header.Unk_0x04,
            Items = items
        };
    }

    private MapJump ToJump(MapSectionRecord record)
    {
        return new MapJump()
        {
            StateData = [record.Unk_0x00, record.Magic],
            NextItem = record.NextSection,
        };
    }

    /// <inheritdoc/>
    public void SerializeTo(MapFile entity, Stream stream)
    {
        throw new NotImplementedException();
    }
}