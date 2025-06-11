namespace TheXDS.Vivianne.Codecs;

/// <summary>
/// Defines a set of members to be implemented by a type that provides
/// information about a codec.
/// </summary>
/// <typeparam name="TCodec">Type of codec for which to get information.</typeparam>
public interface ICodecInfo<out TCodec>
{
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