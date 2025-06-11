namespace TheXDS.Vivianne.Codecs.Textures;

/// <summary>
/// Implements a null codec that does not perform any compression or
/// decompression.
/// </summary>
public class NullImageCodec : IImageCodec
{
    /// <inheritdoc/>
    public byte[] Decode(byte[] sourceBytes, int width, int height)
    {
        return sourceBytes;
    }

    /// <inheritdoc/>
    public byte[] Encode(byte[] sourceBytes, int width, int height)
    {
        return sourceBytes;
    }
}
