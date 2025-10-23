namespace TheXDS.Vivianne.Models.Audio.Mus;

/// <summary>
/// Represents a map used for MUS interactive and/or linear playback,
/// identifying how to stitch MUS audio sub-streams for both playback
/// scenarios.
/// </summary>
public class MapFile
{
    /// <summary>
    /// Gets or sets a value at offset 0x04 whose purpose is currently unknown.
    /// </summary>
    public byte Unk_0x04 { get; set; }

    /// <summary>
    /// Gets a collection of items that correlate to ASF substreams in a .MUS
    /// file on the same location and with the same name as this .MAP file.
    /// </summary>
    public List<MapItem> Items { get; init; } = [];

    /// <summary>
    /// Gets or sets a value that indicates the index of the first
    /// <see cref="MapItem"/> to play.
    /// </summary>
    public int FirstItem { get; set; }
}
