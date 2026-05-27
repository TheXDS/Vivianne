using NAudio.Wave;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Provides a stream of audio data sourced from an ASF file, supporting
/// playback and looping functionality.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IWaveProvider"/> interface to enable
/// audio playback from ASF files. It supports resetting the stream to the
/// beginning or to a defined loop start position, making it suitable for
/// scenarios requiring repeated playback of specific audio segments. The
/// <see cref="PlayLooping"/> property can be used to enable automatic looping
/// between the defined loop start and end positions.
/// </remarks>
/// <param name="asf">
/// The ASF file to be used as the source of audio data.
/// </param>
public class AsfWaveStream(AsfFile asf) : EaAudioWaveStream(asf.SampleRate, asf.BytesPerSample, asf.Channels)
{
    private int currentAbsolutePosition;
    private int currentPosition;
    private int currentAudioBlock;

    /// <summary>
    /// Gets or sets a value indicating whether playback repeats continuously
    /// after reaching the end.
    /// </summary>
    public bool PlayLooping { get; set; }

    /// <summary>
    /// Resets the stream to the beginning, allowing for replaying from the start.
    /// </summary>
    public override void Reset()
    {
        currentAbsolutePosition = 0;
        currentPosition = 0;
        currentAudioBlock = 0;
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
    public override void ResetToLoopStart()
    {
        GoToAbsolutePosition(asf.LoopStart);
    }

    /// <inheritdoc/>
    protected override int Read(byte[] buffer, int offset, int count)
    {
        int bytesRead = 0;
        while (count-- > 0)
        {
            if (currentAudioBlock >= asf.AudioBlocks.Count)
            {
                break;
            }
            var chunk = asf.AudioBlocks[currentAudioBlock];
            buffer[offset++] = chunk[currentPosition++];
            currentAbsolutePosition++;
            bytesRead++;
            if (currentPosition >= chunk.Length)
            {
                currentPosition = 0;
                currentAudioBlock++;
            }
            CheckLooping();
        }
        return bytesRead;
    }

    private void CheckLooping()
    {
        if (PlayLooping && currentAbsolutePosition == asf.LoopEnd)
        {
            ResetToLoopStart();
        }
    }

    private void GoToAbsolutePosition(int position)
    {
        currentAbsolutePosition = position;
        currentAudioBlock = 0;
        currentPosition = 0;
        int bytesToSkip = currentAbsolutePosition;
        while (bytesToSkip > 0)
        {
            int bytesInChunk = asf.AudioBlocks[currentAudioBlock].Length;
            if (bytesToSkip < bytesInChunk)
            {
                currentPosition = bytesToSkip;
                bytesToSkip = 0;
            }
            else
            {
                bytesToSkip -= bytesInChunk;
                currentAudioBlock++;
            }
        }
    }
}