using TheXDS.Vivianne.Serializers.Audio;

namespace TheXDS.Vivianne.Codecs.Audio;

/// <summary>
/// Defines a set of members to be implemented by a type that encodes and
/// decodes raw audio data into some form of easily renderable, uncompressed
/// format.
/// </summary>
public interface IAudioCodec
{
    /// <summary>
    /// Decodes the specified audio data into a raw format.
    /// </summary>
    /// <param name="sourceBytes">
    /// Raw data to be decoded. This data is expected to be in a compressed
    /// format.
    /// </param>
    /// <param name="header">
    /// Audio header information to be used during decoding.
    /// </param>
    /// <returns>
    /// A byte array containing the uncompressed image data.
    /// </returns>
    byte[] Decode(byte[] sourceBytes, PtHeader header);

    /// <summary>
    /// Encodes the specified audio data into a compressed format.
    /// </summary>
    /// <param name="sourceBytes">
    /// Image data to be encoded. This data is expected to be in a raw format.
    /// </param>
    /// <param name="header">
    /// Audio header information to be used during encoding.
    /// </param>
    /// <returns>
    /// A byte array containing the compressed audio data.
    /// </returns>
    byte[] Encode(byte[] sourceBytes, PtHeader header) => throw new NotImplementedException("Encoding is not implemented for this codec.");
}
