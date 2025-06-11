using TheXDS.Vivianne.Models.Fsh;

namespace TheXDS.Vivianne.Codecs.Textures;

/// <summary>
/// Defines a set of members to be implemented by a type that provides
/// information about an image codec.
/// </summary>
/// <typeparam name="TCodec">Type of codec for which to get information.</typeparam>
public interface IImageCodecInfo<out TCodec> : ICodecInfo<TCodec> where TCodec : IImageCodec
{
    /// <summary>
    /// Gets the format of the output data produced by the codec. It must be a
    /// value that represents uncompressed, raw pixel data.
    /// </summary>
    FshBlobFormat OutputFormat { get; }
}
