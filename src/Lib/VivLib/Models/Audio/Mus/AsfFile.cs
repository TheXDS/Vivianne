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

    /// <summary>
    /// Gets or sets the offset in samples at which the audio stream should
    /// loop. Known to be used by Need For Speed II .ASF music files.
    /// </summary>
    public int? LoopOffset { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating internal byte alignment of each main
    /// block on this .ASF file.
    /// </summary>
    public byte? ByteAlignment { get; set; } = null;
}