using NAudio.Wave;
using System;
using System.ComponentModel;
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
public class MusWaveStream : INotifyPropertyChanged, IWaveProvider
{
    /* The full NotifyPropertyChanged implementation was not used in this class
     * because WAV playback could be very latency-sensitive. I'd rather have a
     * very lightweight implementation that only notifies when the
     * CurrentAsfSubStreamIndex changes, which is the only property that is
     * likely to be observed during playback. This way, we avoid unnecessary
     * overhead of the full base class, which has support for many (useful)
     * features we're not gonna use here.
     */
    private readonly MusFile mus;
    private readonly MapFile? map;
    private readonly int[]? mapIndices;
    private readonly int? loopStart;

    private int _currentAsfSubStreamIndex;
    private int _currentAudioBlock;
    private int _currentPosition;
    private bool _playLooping;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets a value indicating whether playback repeats continuously
    /// after reaching the end.
    /// </summary>
    public bool PlayLooping
    {
        get => _playLooping;
        set
        {
            if (map is null)
            {
                throw new InvalidOperationException(St.MapFileRequiredForLooping);
            }
            _playLooping = value;
        }
    }

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    private int CurrentAsfSubStreamIndex
    {
        get => _currentAsfSubStreamIndex;
        set
        {
            _currentAsfSubStreamIndex = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentAsfSubStreamIndexForMap)));
        }
    }

    /// <summary>
    /// Gets or sets the index of the current ASF substream being played within
    /// the current MUS file.
    /// </summary>
    public int CurrentAsfSubStreamIndexForMap => mapIndices is null ? CurrentAsfSubStreamIndex : mapIndices[CurrentAsfSubStreamIndex];

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
            _playLooping = true;
            (mapIndices, loopStart) = MapStitcher.Stitch(map!);
        }
    }

    /// <summary>
    /// Resets the stream to the beginning, allowing for replaying from the start.
    /// </summary>
    public void Reset()
    {
        CurrentAsfSubStreamIndex = 0;
        _currentAudioBlock = 0;
        _currentPosition = 0;
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
        CurrentAsfSubStreamIndex = loopStart!.Value;
        _currentAudioBlock = 0;
        _currentPosition = 0;
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
            var currentSubStream = mus.AsfSubStreams.Values.ElementAt(mapIndices is null ? CurrentAsfSubStreamIndex : mapIndices[CurrentAsfSubStreamIndex]);
            if (_currentAudioBlock >= currentSubStream.AudioBlocks.Count)
            {
                break;
            }
            var chunk = currentSubStream.AudioBlocks[_currentAudioBlock];
            buffer[offset++] = chunk[_currentPosition++];
            bytesRead++;
            if (_currentPosition >= chunk.Length)
            {
                _currentPosition = 0;
                _currentAudioBlock++;
            }
            if (_currentAudioBlock >= currentSubStream.AudioBlocks.Count)
            {
                CurrentAsfSubStreamIndex++;
                _currentAudioBlock = 0;
                if (CurrentAsfSubStreamIndex >= (mapIndices is null ? mus.AsfSubStreams.Count : mapIndices.Length))
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
            if (CurrentAsfSubStreamIndex == mapIndices.Length)
            {
                ResetToLoopStart();
            }
        }
    }
}
