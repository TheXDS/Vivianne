namespace TheXDS.Vivianne.Models.Audio.Base;

/// <summary>
/// Represents a base class for audio streams, which contain the common
/// properties that all audio streams share.
/// </summary>
public abstract class AudioStreamBase
{
    /// <summary>
    /// Gets or sets the number of audio channels contained in the BNK blob
    /// data.
    /// </summary>
    public byte Channels { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether the audio stream is compressed.
    /// </summary>
    public CompressionMethod Compression { get; set; }

    /// <summary>
    /// Gets or sets the sample rate of this BNK blob.
    /// </summary>
    public ushort SampleRate { get; set; }

    /// <summary>
    /// Gets or sets a value that determines the number of bytes that
    /// conform a single audio sample.
    /// </summary>
    public byte BytesPerSample { get; set; }

    /// <summary>
    /// Gets or sets a dictionary with all custom properties on the PT
    /// header for this stream.
    /// </summary>
    public IDictionary<byte, PtHeaderValue> Properties { get; set; } = new Dictionary<byte, PtHeaderValue>();
    
    /// <summary>
    /// Gets or sets a dictionary with all custom properties on the audio PT
    /// header for this stream.
    /// </summary>
    public IDictionary<byte, PtHeaderValue> CustomAudioProperties { get; set; } = new Dictionary<byte, PtHeaderValue>();

    /// <summary>
    /// Gets the estimated duration of the audio stream, based on the number of samples, sample rate, and number of channels.
    /// </summary>
    public TimeSpan CalculatedDuration => TimeSpan.FromSeconds(TotalSamples / Channels / SampleRate);

    /// <summary>
    /// Gets the total number of samples in the audio stream.
    /// </summary>
    public abstract int TotalSamples { get; }

    /// <summary>
    /// Gets or sets the offset where looping audio data begins.
    /// </summary>
    public int LoopStart { get; set; }

    /// <summary>
    /// Gets or sets the offset where looping audio data ends.
    /// </summary>
    public int LoopEnd { get; set; }
}
