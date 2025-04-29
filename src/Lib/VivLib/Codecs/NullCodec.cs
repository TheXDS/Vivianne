namespace TheXDS.Vivianne.Codecs;

/// <summary>
/// Implements a null codec that does not perform any compression or
/// decompression.
/// </summary>
public class NullCodec : IImageCodec
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

public class Dxt1ImageCodec : IImageCodec
{
    /// <inheritdoc/>
    public byte[] Decode(byte[] sourceBytes, int width, int height)
    {
        var decoder = new BCnEncoder.Decoder.BcDecoder();
        var decoded = decoder.DecodeRaw(sourceBytes, width, height, BCnEncoder.Shared.CompressionFormat.Bc1);
        return [.. decoded.SelectMany(p => (byte[])[p.a, p.r, p.g, p.b])];
    }

    /// <inheritdoc/>
    public byte[] Encode(byte[] sourceBytes, int width, int height)
    {
        var encoder = new BCnEncoder.Encoder.BcEncoder();
        var encoded = encoder.EncodeToRawBytes(sourceBytes, width, height, BCnEncoder.Encoder.PixelFormat.Bgr24);


        // Implement DXT1 compression logic here
        throw new NotImplementedException();
    }
}

public class Dxt3ImageCodec : IImageCodec
{
    /// <inheritdoc/>
    public byte[] Decode(byte[] sourceBytes, int width, int height)
    {
        var decoder = new BCnEncoder.Decoder.BcDecoder();
        var decoded = decoder.DecodeRaw(sourceBytes, width, height, BCnEncoder.Shared.CompressionFormat.Bc2);
        return [.. decoded.SelectMany(p => (byte[])[p.b, p.g, p.r, p.a])];
    }

    /// <inheritdoc/>
    public byte[] Encode(byte[] sourceBytes, int width, int height)
    {
        var encoder = new BCnEncoder.Encoder.BcEncoder();
        var encoded = encoder.EncodeToRawBytes(sourceBytes, width, height, BCnEncoder.Encoder.PixelFormat.Bgr24);


        // Implement DXT1 compression logic here
        throw new NotImplementedException();
    }
}
