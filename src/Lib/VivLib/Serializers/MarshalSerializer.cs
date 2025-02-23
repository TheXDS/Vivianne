using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Serializers;

public class MarshalSerializer<T> : ISerializer<T> where T : struct
{
    public T Deserialize(Stream stream)
    {
        using var br = new BinaryReader(stream);
        return br.MarshalReadStruct<T>();
    }

    public void SerializeTo(T entity, Stream stream)
    {
        using var bw = new BinaryWriter(stream);
        bw.MarshalWriteStruct(entity);
    }
}
