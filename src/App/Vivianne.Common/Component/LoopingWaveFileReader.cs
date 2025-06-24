using NAudio.Wave;
using System;
using System.IO;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Extends <see cref="WaveFileReader"/> to allow looping playback of audio
/// files.
/// </summary>
/// <param name="inputStream">
/// Input stream containing the audio data to be read. It must contain a valid
/// .WAV file.
/// </param>
public class LoopingWaveFileReader(Stream inputStream) : WaveFileReader(inputStream)
{
    private static readonly object lockObject = new();
    private long _loopStartBytePosition = -1;
    private long _loopEndBytePosition = -1;
    private long? loopEndSample;
    private long? loopStartSample;

    /// <summary>
    /// Indicates the start sample of the loop in the audio file.
    /// </summary>
    /// <value>
    /// If set, the loop will start at this sample position in the audio file.
    /// If it is <see langword="null"/>, looping is disabled.
    /// </value>
    public long? LoopStartSample
    {
        get => loopStartSample;
        set
        {
            loopStartSample = value;
            if (value is not null) _loopStartBytePosition = value.Value * BlockAlign;
        }
    }

    /// <summary>
    /// Indicates the end sample of the loop in the audio file.
    /// </summary>
    /// <value>
    /// If set, the loop will end at this sample position in the audio file. If
    /// it is <see langword="null"/>, looping is disabled.
    /// </value>
    public long? LoopEndSample
    {
        get => loopEndSample;
        set
        {
            loopEndSample = value;
            if (value is not null) _loopEndBytePosition = value.Value * BlockAlign;
        }
    }

    /// <inheritdoc/>
    public override int Read(byte[] array, int offset, int count)
    {
        if (!LoopStartSample.HasValue || !LoopEndSample.HasValue)
        {
            return base.Read(array, offset, count);
        }
        long position = Position;
        if (position < _loopEndBytePosition)
        {
            return base.Read(array, offset, count);
        }
        else
        {
            long bytesRead = 0;
            while (bytesRead < count)
            {
                int bytesToReadFromLoop = Math.Min(count - (int)bytesRead, (int)(_loopEndBytePosition - _loopStartBytePosition));
                Position = _loopStartBytePosition + bytesRead;
                int readCount = base.Read(array, offset + (int)bytesRead, bytesToReadFromLoop);
                if (readCount == 0)
                {
                    break;
                }
                bytesRead += readCount;
            }
            return (int)bytesRead;
        }
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        long position = Position;

        if (LoopStartSample.HasValue && LoopEndSample.HasValue)
        {
            position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => position + offset,
                SeekOrigin.End => Length - 1 + offset,
                _ => throw new InvalidOperationException("Invalid SeekOrigin specified.")
            };
            if (position >= _loopEndBytePosition)
            {
                position = _loopStartBytePosition + (position - _loopEndBytePosition);
            }
        }
        return base.Seek(position, SeekOrigin.Begin);
    }
}
