using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Implements a generic serializer for simple structs based on Marshaling.
/// </summary>
/// <typeparam name="T">
/// Type of structure to (de)serialize. It must be compatible with Marshaling.
/// </typeparam>
public class MarshalSerializer<T> : ISerializer<T> where T : struct
{
    /// <inheritdoc/>
    public T Deserialize(Stream stream)
    {
        using var br = new BinaryReader(stream);
        return br.MarshalReadStruct<T>();
    }

    /// <inheritdoc/>
    public void SerializeTo(T entity, Stream stream)
    {
        using var bw = new BinaryWriter(stream);
        bw.MarshalWriteStruct(entity);
    }
}
