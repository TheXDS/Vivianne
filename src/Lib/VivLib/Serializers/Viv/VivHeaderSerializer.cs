using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Serializers.Viv;

/// <summary>
/// Implements a serializer that can read and write <see cref="VivFileHeader"/>
/// entities.
/// </summary>
public class VivHeaderSerializer : ISerializer<VivFileHeader>
{
    /// <inheritdoc/>
    public VivFileHeader Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var header = reader.MarshalReadStruct<VivHeader>();
        var dir = new Dictionary<string, VivDirectoryEntry>();
        for (int j = 0; j < header.Entries; j++)
        {
            var entry = reader.MarshalReadStruct<VivDirectoryEntry>();
            var name = reader.ReadNullTerminatedString();
            dir.Add(name, entry);
        }
        return new VivFileHeader(header, dir);
    }

    /// <inheritdoc/>
    public void SerializeTo(VivFileHeader entity, Stream stream)
    {
        using var writer = new BinaryWriter(stream);
    }
}
