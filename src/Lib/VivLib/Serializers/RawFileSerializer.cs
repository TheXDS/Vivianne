using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Provides functionality to serialize and deserialize raw byte arrays to and
/// from streams.
/// </summary>
/// <remarks>
/// This class is designed for scenarios where raw binary data needs to be
/// written to or read from a stream. It implements the
/// <see cref="ISerializer{T}"/> interface for byte arrays.
/// </remarks>
public class RawFileSerializer : ISerializer<RawFile>
{
    /// <inheritdoc/>
    public RawFile Deserialize(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return new RawFile { Data = ms.ToArray() };
    }

    /// <inheritdoc/>
    public void SerializeTo(RawFile entity, Stream stream)
    {
        stream.Write(entity.Data, 0, entity.Data.Length);
    }
}