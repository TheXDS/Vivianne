namespace TheXDS.Vivianne.Models.Audio.Mus;

/// <summary>
/// Represents a jump entry with its state condition data.
/// </summary>
public class MapJump
{
    /// <summary>
    /// Gets or sets a block of data that [might?] determine if the jump to the specified item should be performed.
    /// </summary>
    public byte[] StateData { get; set; } = [];

    /// <summary>
    /// Indicates the next item to jump to.
    /// </summary>
    public int NextItem { get; set; }
}