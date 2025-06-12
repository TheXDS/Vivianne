namespace TheXDS.Vivianne.Models.Audio.Mus;

/// <summary>
/// Represents a MUS file, which is a collection of ASF audio streams,
/// generally used for interactive music.
/// </summary>
/// <remarks>
/// Technically, a MUS file with a single sub-stream is essentially an ASF
/// file. Therefore, this structure could be used to represent an ASF file as
/// well, given that there's only one single stream.
/// </remarks>
public class MusFile
{
    /// <summary>
    /// Gets the collection of ASF audio sub-streams contained in this MUS
    /// file.
    /// </summary>
    public IDictionary<int, AsfFile> AsfSubStreams { get; } = new Dictionary<int, AsfFile>();
}
