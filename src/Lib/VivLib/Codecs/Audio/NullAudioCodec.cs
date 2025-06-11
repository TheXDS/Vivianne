using TheXDS.Vivianne.Serializers.Audio;

namespace TheXDS.Vivianne.Codecs.Audio;

/// <summary>
/// Implements a codec that does not perform any audio processing.
/// </summary>
public class NullAudioCodec : IAudioCodec
{
    /// <summary>
    /// Creates a new instance of the <see cref="NullAudioCodec"/> class.
    /// </summary>
    /// <returns></returns>
    public static NullAudioCodec Create() => new();

    /// <inheritdoc/>
    public byte[] Decode(byte[] sourceBytes, PtHeader header) => sourceBytes;

    /// <inheritdoc/>
    public byte[] Encode(byte[] sourceBytes, PtHeader header) => sourceBytes;
}
