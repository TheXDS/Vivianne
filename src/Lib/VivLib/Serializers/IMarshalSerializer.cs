using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Serializers;

internal interface IMarshalSerializer<TModel, TMarshal> : ISerializer<TModel> where TMarshal : struct
{
    protected TModel Convert(TMarshal entity);

    protected TMarshal Convert(TModel entity);

    TModel IOutSerializer<TModel>.Deserialize(Stream stream)
    {
        using var br = new BinaryReader(stream);
        return Convert(br.MarshalReadStruct<TMarshal>());
    }

    void IInSerializer<TModel>.SerializeTo(TModel entity, Stream stream)
    {
        using var bw = new BinaryWriter(stream);
        bw.MarshalWriteStruct(Convert(entity));
    }
}