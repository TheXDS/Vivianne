using TheXDS.Vivianne.Models.Audio.Base;

namespace TheXDS.Vivianne.Models.Audio.Mus;

/// <summary>
/// Represents an ASF audio file, which is a single audio stream generally used for music.
/// </summary>
public class AsfFile : AudioStreamBase
{
    /// <summary>
    /// Gets the collection of audio blocks contained in this ASF file.
    /// </summary>
    public IList<byte[]> AudioBlocks { get; } = [];

    /// <inheritdoc/>
    public override int TotalSamples => AudioBlocks.Sum(p => p.Length) / BytesPerSample;
}