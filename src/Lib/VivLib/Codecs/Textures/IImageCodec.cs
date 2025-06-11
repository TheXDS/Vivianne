namespace TheXDS.Vivianne.Codecs.Textures;

/// <summary>
/// Defines a set of members to be implemented by a type that encodes and
/// decodes raw image data into some form of easily renderable, uncompressed
/// format.
/// </summary>
public interface IImageCodec
{
    /// <summary>
    /// Decodes the specified image data into a raw format.
    /// </summary>
    /// <param name="sourceBytes">
    /// Raw data to be decoded. This data is expected to be in a compressed
    /// format.
    /// </param>
    /// <param name="width">Target width of the uncompressed image.</param>
    /// <param name="height">Target height of the uncompressed image.</param>
    /// <returns>
    /// A byte array containing the uncompressed image data.
    /// </returns>
    byte[] Decode(byte[] sourceBytes, int width, int height);

    /// <summary>
    /// Encodes the specified image data into a compressed format.
    /// </summary>
    /// <param name="sourceBytes">
    /// Image data to be encoded. This data is expected to be in a raw format.
    /// </param>
    /// <param name="width">Current width of the source image.</param>
    /// <param name="height">Current height of the source image.</param>
    /// <returns>
    /// A byte array containing the compressed image data.
    /// </returns>
    byte[] Encode(byte[] sourceBytes, int width, int height) => throw new NotImplementedException("This codec does not support encoding.");
}
