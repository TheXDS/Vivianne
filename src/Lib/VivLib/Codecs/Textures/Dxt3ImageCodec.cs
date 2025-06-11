namespace TheXDS.Vivianne.Codecs.Textures;

/// <summary>
/// Implements a codec for DXT3 compressed images.
/// </summary>
public class Dxt3ImageCodec : IImageCodec
{
    /// <inheritdoc/>
    public byte[] Decode(byte[] sourceBytes, int width, int height)
    {
        var decoder = new BCnEncoder.Decoder.BcDecoder();
        var decoded = decoder.DecodeRaw(sourceBytes, width, height, BCnEncoder.Shared.CompressionFormat.Bc2);
        return [.. decoded.SelectMany(p => (byte[])[p.b, p.g, p.r, p.a])];
    }

    ///// <inheritdoc/>
    //public byte[] Encode(byte[] sourceBytes, int width, int height)
    //{
    //    var encoder = new BCnEncoder.Encoder.BcEncoder();
    //    var encoded = encoder.EncodeToRawBytes(sourceBytes, width, height, BCnEncoder.Encoder.PixelFormat.Bgr24);
    //    throw new NotImplementedException();
    //}
}
