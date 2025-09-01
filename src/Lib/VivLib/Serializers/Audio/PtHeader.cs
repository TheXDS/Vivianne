using TheXDS.Vivianne.Models.Audio.Base;

namespace TheXDS.Vivianne.Serializers.Audio;

/// <summary>
/// Represents a Property Table (PT) header, which includes metadata about an
/// audio stream.
/// </summary>
public class PtHeader
{
    /// <summary>
    /// Gets a reference to the default set of properties for a PT header.
    /// </summary>
    public static readonly PtHeader Default = new();

    /// <summary>
    /// Gets a dictionary that includes all generic metadata values for the PT
    /// header.
    /// </summary>
    public readonly Dictionary<PtHeaderField, PtHeaderValue> Values = [];
    /// <summary>
    /// Gets a dictionary that includes all audio-specific values present in
    /// the audio sub-header in a PT header.
    /// </summary>
    public readonly Dictionary<PtAudioHeaderField, PtHeaderValue> AudioValues = new()
    {
        { PtAudioHeaderField.Channels, new(1, 1) },
        { PtAudioHeaderField.Compression, new(1, 0) },
        { PtAudioHeaderField.SampleRate, new(2, 22050) },
        { PtAudioHeaderField.NumSamples, new(4, 0) },
        { PtAudioHeaderField.LoopOffset, new(1, 0) },
        { PtAudioHeaderField.LoopEnd, new(1, 0) },
        { PtAudioHeaderField.DataOffset, new(0, 0) },
        { PtAudioHeaderField.BytesPerSample, new(1, 2) },
        { PtAudioHeaderField.EndOfHeader, new(4, 0) },
    };

    /// <summary>
    /// Gets or sets a generic metadata value on this PT header.
    /// </summary>
    /// <param name="field">Field to get or set.</param>
    /// <returns>
    /// A <see cref="PtHeaderValue"/> structure that contains information on
    /// the requested value.
    /// </returns>
    public PtHeaderValue this[PtHeaderField field]
    {
        get => Values[field];
        set => Values[field] = value;
    }

    /// <summary>
    /// Gets or sets an audio-specific property on the audio sub-header on this
    /// PT header.
    /// </summary>
    /// <param name="field">Field to get or set.</param>
    /// <returns>
    /// A <see cref="PtHeaderValue"/> structure that contains information on
    /// the requested value.
    /// </returns>
    public PtHeaderValue this[PtAudioHeaderField field]
    {
        get => AudioValues[field];
        set => AudioValues[field] = value;
    }

    /// <summary>
    /// Gets or sets a reference to an secondary PT header used by an
    /// alternative audio stream that might be present on the audio stream.
    /// </summary>
    /// <remarks>
    /// This feature is used exclusively by the BNK audio format. Its usage
    /// with ASF, MUS and other audio formats is not supoported.
    /// </remarks>
    public PtHeader? AltStream { get; set; }
}
