namespace TheXDS.Vivianne.Models.Audio.Mus;

/// <summary>
/// Represents a map used for MUS interactive and/or linear playback,
/// identifying how to stitch MUS audio sub-streams for both playback
/// scenarios.
/// </summary>
public class MapFile
{
    /// <summary>
    /// Gets a collection of offsets to stitch together to form a linear playback audio stream.
    /// </summary>
    public List<int> LinearPlaybackOffsets { get; } = [];
}