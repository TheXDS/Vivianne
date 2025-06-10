using TheXDS.Vivianne.Models.Audio.Base;

namespace TheXDS.Vivianne.Models.Audio.Mus;

public class MusFile
{
    public IList<AsfFile> AsfElement { get; } = [];
}

public class AsfFile : AudioStreamBase
{
    /// <summary>
    /// Gets the collection of audio blocks contained in this ASF file.
    /// </summary>
    public IList<byte[]> AudioBlocks { get; } = [];
}