using TheXDS.Vivianne.Codecs.Audio;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers.Audio;

namespace TheXDS.Vivianne.Codecs.Textures;

/// <summary>
/// Represents a codec information object that provides information about a
/// codec and its output format.
/// </summary>
/// <typeparam name="TCodec">
/// The type of the codec. This type must implement the
/// <see cref="IImageCodec"/> interface.
/// </typeparam>
/// <param name="outputFormat">
/// Codec output format. This value must be a value that represents the raw,
/// uncompressed pixel format.
/// </param>
public class ImageCodecInfo<TCodec>(FshBlobFormat outputFormat) : IImageCodecInfo<IImageCodec> where TCodec : IImageCodec, new()
{
    /// <inheritdoc/>
    public FshBlobFormat OutputFormat { get; } = outputFormat;

    /// <inheritdoc/>
    public IImageCodec GetCodec() => new TCodec();
}
