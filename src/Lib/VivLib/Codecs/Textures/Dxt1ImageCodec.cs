namespace TheXDS.Vivianne.Codecs.Textures;

/// <summary>
/// Implements a codec for DXT1 compressed images.
/// </summary>
public class Dxt1ImageCodec : IImageCodec
{
    /// <inheritdoc/>
    public byte[] Decode(byte[] sourceBytes, int width, int height)
    {
        var decoder = new BCnEncoder.Decoder.BcDecoder();
        var decoded = decoder.DecodeRaw(sourceBytes, width, height, BCnEncoder.Shared.CompressionFormat.Bc1);
        return [.. decoded.SelectMany(p => (byte[])[p.a, p.r, p.g, p.b])];
    }

    ///// <inheritdoc/>
    //public byte[] Encode(byte[] sourceBytes, int width, int height)
    //{
    //    var encoder = new BCnEncoder.Encoder.BcEncoder();
    //    var encoded = encoder.EncodeToRawBytes(sourceBytes, width, height, BCnEncoder.Encoder.PixelFormat.Bgr24);
    //    throw new NotImplementedException();
    //}
}
