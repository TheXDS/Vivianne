using TheXDS.Vivianne.Models.Audio.Base;

namespace TheXDS.Vivianne.Models.Audio.Mus;

/// <summary>
/// Represents an ASF audio stream, which is a single audio stream.
/// </summary>
public class AsfFile : AudioStreamBase
{
    /// <summary>
    /// Gets the collection of audio blocks contained in this ASF file.
    /// </summary>
    public IList<byte[]> AudioBlocks { get; } = [];
}