using NAudio.Wave;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Provides abstract audio wave stream functionality for EA audio formats,
/// implementing the <see cref="IWaveProvider"/> interface with support for
/// sample rate, bits per sample, and channel configuration.
/// </summary>
/// <param name="sampleRate">The sample rate of the audio stream in samples per second.</param>
/// <param name="bytesPerSample">The number of bytes per sample (bits depth).</param>
/// <param name="channels">The number of audio channels.</param>
public abstract class EaAudioWaveStream(ushort sampleRate, byte bytesPerSample, byte channels) : IWaveProvider
{
    /// <summary>
    /// Represents the number of seconds corresponding to each sample.
    /// </summary>
    protected readonly double SecondsPerSample = 1.0 / sampleRate / channels / bytesPerSample;

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; } = new WaveFormat(sampleRate, bytesPerSample * 8, channels);

    /// <summary>
    /// Gets the current position in seconds within the audio stream, calculated
    /// based on the number of samples processed and the audio format.
    /// </summary>
    public double CurrentPositionInSeconds { get; protected set; }

    /// <summary>
    /// Resets the stream to the beginning, allowing for replaying from the start.
    /// </summary>
    public abstract void Reset();

    /// <summary>
    /// Resets the current position to the start of the defined loop segment.
    /// </summary>
    /// <remarks>
    /// Use this method to return playback or processing to the beginning of
    /// the loop as defined by the current loop start position. This is
    /// typically used in scenarios where repeated playback or processing of a
    /// specific segment is required.
    /// </remarks>
    public abstract void ResetToLoopStart();

    int IWaveProvider.Read(byte[] buffer, int offset, int count)
    {
        return Read(buffer, offset, count);
    }

    /// <summary>
    /// When overridden in a derived class, reads a sequence of bytes from the
    /// stream and advances the position within the stream by the number of bytes
    /// read.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer
    /// contains the specified byte array with the values between <paramref name="offset"/>
    /// and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes
    /// read from the current source.</param>
    /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin
    /// writing data from the stream.</param>
    /// <param name="count">The maximum number of bytes to be read from the current source.</param>
    /// <returns>The total number of bytes read into the buffer. This can be less than the
    /// number of bytes requested if that many bytes are not currently available, or zero
    /// (0) if the end of the stream has been reached.</returns>
    protected abstract int Read(byte[] buffer, int offset, int count);
}
