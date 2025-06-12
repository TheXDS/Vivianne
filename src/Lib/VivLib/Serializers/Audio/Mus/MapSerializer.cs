using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

/// <summary>
/// Implements a serializer that can read and write entities of type
/// <see cref="MapFile"/>.
/// </summary>
public class MapSerializer : ISerializer<MapFile>
{
    /// <inheritdoc/>
    public MapFile Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SerializeTo(MapFile entity, Stream stream)
    {
        throw new NotImplementedException();
    }
}