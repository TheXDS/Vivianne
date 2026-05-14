using NAudio.Wave;
using TheXDS.Vivianne.Models.Audio.Bnk;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Provides a stream of audio data sourced from a BNK stream, supporting
/// playback and looping functionality.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IWaveProvider"/> interface to enable
/// audio playback from BNK streams. It supports resetting the stream to the
/// beginning or to a defined loop start position, making it suitable for
/// scenarios requiring repeated playback of specific audio segments. The
/// <see cref="PlayLooping"/> property can be used to enable automatic looping
/// between the defined loop start and end positions.
/// </remarks>
/// <param name="bnk">
/// The BNK stream to be used as the source of audio data.
/// </param>
public class BnkWaveStream(BnkStream bnk) : IWaveProvider
{
    private int currentAbsolutePosition;

    /// <summary>
    /// Gets or sets a value indicating whether playback repeats continuously
    /// after reaching the end.
    /// </summary>
    public bool PlayLooping { get; set; }

    WaveFormat IWaveProvider.WaveFormat { get; } = new WaveFormat(bnk.SampleRate, bnk.BytesPerSample * 8, bnk.Channels);

    /// <summary>
    /// Resets the stream to the beginning, allowing for replaying from the start.
    /// </summary>
    public void Reset()
    {
        currentAbsolutePosition = 0;
    }

    /// <summary>
    /// Resets the current position to the start of the defined loop segment.
    /// </summary>
    /// <remarks>
    /// Use this method to return playback or processing to the beginning of
    /// the loop as defined by the current loop start position. This is
    /// typically used in scenarios where repeated playback or processing of a
    /// specific segment is required.
    /// </remarks>
    public void ResetToLoopStart()
    {
        currentAbsolutePosition = bnk.LoopStart * bnk.BytesPerSample * bnk.Channels;
    }

    int IWaveProvider.Read(byte[] buffer, int offset, int count)
    {
        int bytesRead = 0;
        while (count-- > 0)
        {
            if (currentAbsolutePosition >= bnk.SampleData.Length)
            {
                break;
            }
            buffer[offset++] = bnk.SampleData[currentAbsolutePosition++];
            bytesRead++;
            CheckLooping();
        }
        return bytesRead;
    }

    private void CheckLooping()
    {
        if (PlayLooping && currentAbsolutePosition >= bnk.LoopEnd * bnk.BytesPerSample * bnk.Channels)
        {
            ResetToLoopStart();
        }
    }
}
