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
/// scenarios requiring repeated playback of specific audio segments, given
/// that a <see cref="MapFile"/> is provided.
/// </remarks>
public class MusWaveStream : EaAudioWaveStream, INotifyPropertyChanged
{
    /* The full NotifyPropertyChanged implementation was not used in this class
     * because WAV playback could be very latency-sensitive. I'd rather have a
     * very lightweight implementation that only notifies when the
     * CurrentAsfSubStreamIndexForMap changes, which is the only property that
     * is likely to be observed during playback. This way, we avoid unnecessary
     * overhead of the full base class, which has support for many (useful)
     * features we're not gonna use here.
     */
    private readonly MusFile _mus;
    private readonly MapFile? _map;
    private readonly int[]? _mapIndices;
    private readonly int? _loopStart;
    private readonly int? _loopEnd;
    private readonly int _loopBlock;
    private readonly int _loopPosition;

    private int _currentAsfSubStreamIndex;
    private int _currentAudioBlock;
    private int _currentPosition;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the index of the current ASF substream being played within
    /// the current MUS file.
    /// </summary>
    public int CurrentAsfSubStreamIndexForMap => _mapIndices is null ? CurrentAsfSubStreamIndex : _mapIndices[CurrentAsfSubStreamIndex];

    /// <summary>
    /// Initializes a new instance of the <see cref="MusWaveStream"/> class.
    /// </summary>
    /// <param name="mus">The MUS file to be played.</param>
    /// <param name="map">The optional map file for loop points.</param>
    public MusWaveStream(MusFile mus, MapFile? map = null) : base(
        GetAudioProp(mus, p => p.SampleRate),
        GetAudioProp(mus, p => p.BytesPerSample),
        GetAudioProp(mus, p => p.Channels))
    {
        ArgumentNullException.ThrowIfNull(mus);
        _mus = mus;
        if ((_map = map) is not null)
        {
            (_mapIndices, _loopStart) = MapStitcher.Stitch(map!);
        }
        else
        {
            _loopEnd = GetAudioProp(mus, p => p.LoopEnd);
            var loopOffset = GetAudioProp(mus, p => p.LoopStart);
            _loopBlock = CalculateAudioBlockForOffset(loopOffset);
        }
    }

    /// <summary>
    /// Resets the stream to the beginning, allowing for replaying from the start.
    /// </summary>
    public override void Reset()
    {
        CurrentAsfSubStreamIndex = 0;
        _currentAudioBlock = 0;
        _currentPosition = 0;
        CurrentPositionInSeconds = 0;
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
        if (_map is null)
        {
            throw new InvalidOperationException(St.MapFileRequiredForReset);
        }
        CurrentAsfSubStreamIndex = _loopStart!.Value;
        _currentAudioBlock = _loopBlock;
        _currentPosition = 0;
    }

    private int CurrentAsfSubStreamIndex
    {
        get => _currentAsfSubStreamIndex;
        set
        {
            _currentAsfSubStreamIndex = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentAsfSubStreamIndexForMap)));
        }
    }

    private int CalculateAudioBlockForOffset(int offset)
    {
        int block = 0;
        while ((offset -= _mus.AsfSubStreams.Values.ElementAt(CurrentAsfSubStreamIndex).AudioBlocks[block].Length) > 0) block++;
        return block;
    }

    private static T GetAudioProp<T>(MusFile mus, Func<AsfFile, T> propSelector)
    {
        ArgumentNullException.ThrowIfNull(mus);
        var values = mus.AsfSubStreams.Values.Select(propSelector).ToArray();
        if (values.Distinct().Count() != 1)
        {
            throw new InvalidOperationException(St.MultiFormatMusNotSupported);
        }
        return values.First();
    }

    /// <inheritdoc/>
    protected override int Read(byte[] buffer, int offset, int count)
    {
        int bytesRead = 0;
        while (count-- > 0)
        {
            var currentSubStream = _mus.AsfSubStreams.Values.ElementAtOrDefault(_mapIndices is null ? CurrentAsfSubStreamIndex : _mapIndices[CurrentAsfSubStreamIndex]);
            if (currentSubStream is null || _currentAudioBlock >= currentSubStream.AudioBlocks.Count)
            {
                break;
            }
            var chunk = currentSubStream.AudioBlocks[_currentAudioBlock];
            buffer[offset++] = chunk[_currentPosition++];
            bytesRead++;
            CurrentPositionInSeconds += SecondsPerSample;
            if (_currentPosition >= chunk.Length)
            {
                _currentPosition = 0;
                _currentAudioBlock++;
            }
            if (_currentAudioBlock >= currentSubStream.AudioBlocks.Count)
            {
                CurrentAsfSubStreamIndex++;
                _currentAudioBlock = 0;
                if (CurrentAsfSubStreamIndex >= (_mapIndices is null ? _mus.AsfSubStreams.Count : _mapIndices.Length))
                {
                    if (_map is not null)
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
        if (_mapIndices is not null && CurrentAsfSubStreamIndex == _mapIndices.Length)
        {
            ResetToLoopStart();
        }
    }
}
