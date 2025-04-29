using TheXDS.Vivianne.Models.Fsh;

namespace TheXDS.Vivianne.Codecs;

/// <summary>
/// Defines a set of members to be implemented by a type that provides
/// information about a codec.
/// </summary>
/// <typeparam name="TCodec"></typeparam>
public interface ICodecInfo<out TCodec> where TCodec : IImageCodec
{
    /// <summary>
    /// Gets the format of the output data produced by the codec. It must be a
    /// value that represents uncompressed, raw pixel data.
    /// </summary>
    FshBlobFormat OutputFormat { get; }

    /// <summary>
    /// Gets the codec instance that can be used to encode and decode image
    /// data.
    /// </summary>
    /// <returns>
    /// A new instance of the codec that can be used to encode and decode image
    /// data.
    /// </returns>
    TCodec GetCodec();
}
