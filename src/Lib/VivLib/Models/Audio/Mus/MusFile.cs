namespace TheXDS.Vivianne.Models.Audio.Mus;

/// <summary>
/// Represents a MUS file, which is a collection of ASF audio streams,
/// generally used for interactive music.
/// </summary>
public class MusFile
{
    /// <summary>
    /// Gets the collection of ASF audio streams contained in this MUS file.
    /// </summary>
    public IDictionary<int, AsfFile> AsfSubStreams { get; } = new Dictionary<int, AsfFile>();
}

public class MapFile
{
    public List<int> LinearPlaybackOffsets { get; } = [];
}