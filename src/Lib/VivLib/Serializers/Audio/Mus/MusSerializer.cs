using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

/// <summary>
/// Implements a serializer for MUS/ASF files.
/// </summary>
/// <remarks>
/// Technically, a MUS file with a single ASF sub-stream is essentially an ASF
/// file. Therefore, this serializer can be used to read an ASF file and return
/// it as a <see cref="MusFile"/> instance. However, a MUS file with multiple
/// sub-streams is not a valid .ASF file, and therefore this serializer can
/// only implement deserialization of <see cref="AsfFile"/> instances, even if
/// the formats are otherwise equivalent.
/// </remarks>
public partial class MusSerializer : ISerializer<MusFile>, ISerializer<AsfFile>
{
    /// <inheritdoc/>
    public MusFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var mus = new MusFile();
        do
        {
            if (ReadAsfFile(br) is { } asf) mus.AsfSubStreams.Add((int)stream.Position, asf);
        } while ((stream.Position + Marshal.SizeOf<AsfBlockHeader>()) < stream.Length);
        return mus;
    }

    /// <inheritdoc/>
    public void SerializeTo(MusFile entity, Stream stream)
    {
        using BinaryWriter bw = new(stream);
        foreach (AsfFile asf in entity.AsfSubStreams.Values)
        {
            WriteAsf(asf, bw);
        }
    }

    /// <inheritdoc/>
    public void SerializeTo(AsfFile entity, Stream stream)
    {
        using BinaryWriter bw = new(stream);
        WriteAsf(entity, bw);
    }

    AsfFile IOutSerializer<AsfFile>.Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        return ReadAsfFile(br) ?? throw new InvalidDataException("The file does not seem to be a valid ASF stream.");
    }
}
