using NAudio.Wave;
using System;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio.Mus;
using St = TheXDS.Vivianne.Resources.Strings.Component.MusWaveStream;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Provides a stream of audio data sourced from a MUS file, supporting
/// playback and looping functionality.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IWaveProvider"/> interface to enable
/// audio playback from MUS files. It supports resetting the stream to the
/// beginning or to a defined loop start position, making it suitable for
/// scenarios requiring repeated playback of specific audio segments. The
/// <see cref="PlayLooping"/> property can be used to enable automatic looping
/// between the defined loop start and end positions, given that a
/// <see cref="MapFile"/> is provided.
/// </remarks>
public class MusWaveStream : IWaveProvider
{
    private readonly MusFile mus;
    private readonly MapFile? map;
    private readonly int[]? mapIndices;
    private readonly int? loopStart;

    private int currentAsfSubStreamIndex;
    private int currentAudioBlock;
    private int currentPosition;
    private bool playLooping;

    /// <summary>
    /// Gets or sets a value indicating whether playback repeats continuously
    /// after reaching the end.
    /// </summary>
    public bool PlayLooping
    {
        get => playLooping;
        set
        {
            if (map is null)
            {
                throw new InvalidOperationException(St.MapFileRequiredForLooping);
            }
            playLooping = value;
        }
    }

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusWaveStream"/> class.
    /// </summary>
    /// <param name="mus">The MUS file to be played.</param>
    /// <param name="map">The optional map file for loop points.</param>
    public MusWaveStream(MusFile mus, MapFile? map = null)
    {
        ArgumentNullException.ThrowIfNull(mus);
        var audioPropsSourceOfTruth = mus.AsfSubStreams.Values.First();

        mus.AsfSubStreams.Values.Select(p => p.SampleRate).Quorum(mus.AsfSubStreams.Count);

        WaveFormat = new WaveFormat(
            GetAudioProp(mus, p => p.SampleRate),
            GetAudioProp(mus, p => p.BytesPerSample) * 8,
            GetAudioProp(mus, p => p.Channels));
        this.mus = mus;
        if ((this.map = map) is not null)
        {
            playLooping = true;
            (mapIndices, loopStart) = MapStitcher.Stitch(map!);
        }
    }

    /// <summary>
    /// Resets the stream to the beginning, allowing for replaying from the start.
    /// </summary>
    public void Reset()
    {
        currentAsfSubStreamIndex = 0;
        currentAudioBlock = 0;
        currentPosition = 0;
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
        if (map is null)
        {
            throw new InvalidOperationException(St.MapFileRequiredForReset);
        }
        currentAsfSubStreamIndex = loopStart!.Value;
        currentAudioBlock = 0;
        currentPosition = 0;
    }

    private static T GetAudioProp<T>(MusFile mus, Func<AsfFile, T> propSelector)
    {
        var values = mus.AsfSubStreams.Values.Select(propSelector).ToArray();
        if (values.Distinct().Count() != 1)
        {
            throw new InvalidOperationException(St.MultiFormatMusNotSupported);
        }
        return values.First();
    }

    int IWaveProvider.Read(byte[] buffer, int offset, int count)
    {
        int bytesRead = 0;
        while (count-- > 0)
        {
            var currentSubStream = mus.AsfSubStreams.Values.ElementAt(mapIndices is null ? currentAsfSubStreamIndex : mapIndices[currentAsfSubStreamIndex]);
            if (currentAudioBlock >= currentSubStream.AudioBlocks.Count)
            {
                break;
            }
            var chunk = currentSubStream.AudioBlocks[currentAudioBlock];
            buffer[offset++] = chunk[currentPosition++];
            bytesRead++;
            if (currentPosition >= chunk.Length)
            {
                currentPosition = 0;
                currentAudioBlock++;
            }
            if (currentAudioBlock >= currentSubStream.AudioBlocks.Count)
            {
                currentAsfSubStreamIndex++;
                currentAudioBlock = 0;
                if (currentAsfSubStreamIndex >= (mapIndices is null ? mus.AsfSubStreams.Count : mapIndices.Length))
                {
                    if (PlayLooping && map is not null)
                    {
                        ResetToLoopStart();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                CheckLooping();
            }
        }
        return bytesRead;
    }

    private void CheckLooping()
    {
        if (PlayLooping && mapIndices is not null)
        {
            if (currentAsfSubStreamIndex == mapIndices.Length)
            {
                ResetToLoopStart();
            }
        }
    }
}
