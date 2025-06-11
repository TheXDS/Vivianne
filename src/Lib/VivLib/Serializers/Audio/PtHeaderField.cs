namespace TheXDS.Vivianne.Serializers.Audio;

/// <summary>
/// Enumerates the possible header blocks that can be found in a PT header.
/// </summary>
/// <remarks>
/// Custom values are permitted, and its values will be available to be read
/// and written.
/// </remarks>
public enum PtHeaderField : byte
{
    /// <summary>
    /// Indicates that the following data is an audio stream header.
    /// </summary>
    AudioHeader = 0xfd,
    
    /// <summary>
    /// Indicates a special data block that has not been identified yet (might
    /// be a secondary alt stream, needs to be confirmed)
    /// </summary>
    Unk_0xfc = 0xfc,

    /// <summary>
    /// Indicates that the following data is for an alternate
    /// <see cref="PtHeader"/> that could be used by the game as a fallback
    /// stream, a simultaneous playback stream, etc.
    /// </summary>
    /// <remarks>
    /// While some BNKs in NFS3 include alt streams, it is not verified that
    /// this data is actually being used. Speculation suggests that it might be
    /// used if the other primary PT header values instruct the game to do so,
    /// but this is also unconfirmed.
    /// </remarks>
    AlternateStream = 0xfe,

    /// <summary>
    /// Indicates the end of the PT header data, and further parsing should be
    /// stopped at this point.
    /// </summary>
    EndOfHeader = 0xff,
}