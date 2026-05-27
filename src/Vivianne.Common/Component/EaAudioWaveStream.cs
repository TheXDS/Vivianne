using NAudio.Wave;

namespace TheXDS.Vivianne.Component;

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

    protected abstract int Read(byte[] buffer, int offset, int count);
}
