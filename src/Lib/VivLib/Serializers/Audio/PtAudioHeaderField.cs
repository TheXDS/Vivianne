namespace TheXDS.Vivianne.Serializers.Audio;

/// <summary>
/// Enumeration of audio header fields used to describe audio streams.
/// </summary>
/// <remarks>
/// Custom values are permitted, audio parsers and renderers may ignore those
/// not defined in this enum. 
/// </remarks>
public enum PtAudioHeaderField : byte
{
    /// <summary>
    /// Indicates the number of channels in the audio stream.
    /// </summary>
    Channels = 0x82,
    /// <summary>
    /// Indicates the compression type used for the audio stream.
    /// </summary>
    Compression = 0x83,
    /// <summary>
    /// Indicates the sample rate of the audio stream in Hz.
    /// </summary>
    SampleRate = 0x84,
    /// <summary>
    /// Indicates the total number of samples in the audio stream.
    /// </summary>
    NumSamples = 0x85,
    /// <summary>
    /// Indicates the offset in the audio stream where looping begins.
    /// </summary>
    LoopOffset = 0x86,
    /// <summary>
    /// Indicates the offset in the audio stream where looping ends.
    /// </summary>
    LoopEnd = 0x87,
    /// <summary>
    /// Indicates the offset in the audio stream where the actual audio data begins.
    /// </summary>
    DataOffset = 0x88,
    /// <summary>
    /// Indicates the number of bytes per sample in the audio stream.
    /// </summary>
    BytesPerSample = 0x92,
    /// <summary>
    /// Special marker that indicates the end of the audio header section.
    /// </summary>
    EndOfHeader = 0x8A,

    /// <summary>
    /// Unknown audio property.
    /// </summary>
    Unk_0x91 = 0x91
}
