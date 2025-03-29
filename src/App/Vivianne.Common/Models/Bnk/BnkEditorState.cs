using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.ViewModels.Bnk;

namespace TheXDS.Vivianne.Models.Bnk;

/// <summary>
/// Represents the current state of the <see cref="BnkEditorViewModel"/>.
/// </summary>
public class BnkEditorState : FileStateBase<BnkFile>
{
    private ObservableListWrap<BnkStream?>? _streams;
    private BnkStream? _selectedStream;
    private bool _showInfo = true;
    private int _loopStart;
    private int _loopEnd;

    /// <summary>
    /// Gets a reference to the collection of streams available in the BNK file
    /// as laid out on it, allowing the addition and removal of streams.
    /// </summary>
    public ObservableListWrap<BnkStream?> Streams => _streams ??= GetObservable(File.Streams);

    /// <summary>
    /// Enumerates all available streams on the BNK file (including alternate
    /// streams) on a flat list for ease of access.
    /// </summary>
    public IEnumerable<BnkStream> AllStreams => Streams.Concat(File.Streams.Select(p => p?.AltStream)).NotNull();

    /// <summary>
    /// Gets or sets the currently selected stream for editing.
    /// </summary>
    public BnkStream? SelectedStream
    {
        get => _selectedStream;
        set
        {
            if (Change(ref _selectedStream, value) && value is { LoopStart: int ls, LoopEnd: int le })
            {
                LoopStart = ls;
                LoopEnd = le;
                Refresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the starting position (as a sample index) of the looping
    /// audio for the selected audio stream.
    /// </summary>
    public int LoopStart
    {
        get => _loopStart;
        set
        {
            if (Change(ref _loopStart, value) && SelectedStream is not null) SelectedStream.LoopStart = value;
        }
    }

    /// <summary>
    /// Gets or sets the duration (in samples) of the looping audio for the
    /// selected audio stream.
    /// </summary>
    public int LoopEnd
    {
        get => _loopEnd;
        set
        {
            if (Change(ref _loopEnd, value) && SelectedStream is not null) SelectedStream.LoopEnd = value;
        }
    }

    /// <summary>
    /// Gets or sets a value that either enables or disables the information
    /// panel overlay.
    /// </summary>
    public bool ShowInfo
    {
        get => _showInfo;
        set => Change(ref _showInfo, value);
    }
}
