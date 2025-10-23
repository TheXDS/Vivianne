namespace TheXDS.Vivianne.Models.Audio.Mus;

/// <summary>
/// Represents a single MAP entry, which directly correlates with a
/// corresponding ASF substream in a .MUS file.
/// </summary>
public class MapItem
{
    /// <summary>
    /// Offset (or also ID in the form of an offset) of the ASF substream
    /// related to this instance.
    /// </summary>
    public int MusOffset { get; set; }

    /// <summary>
    /// Gets a table that contains jump conditions for this item.
    /// </summary>
    public List<MapJump> Jumps { get; init; } = [];
}
