using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.ViewModels.Mus;

/// <summary>
/// Represents the state of the MUS file editor, managing the association between
/// MUS audio streams and MAP file items for editing operations.
/// </summary>
public class MusEditorState : EditorViewModelStateBase
{
    private MusFile _mus;
    private MapFile _map;

    /// <summary>
    /// Gets the dictionary mapping stream indices to their corresponding ASF file data.
    /// </summary>
    public IDictionary<int, AsfFile> MusStreams { get; }

    /// <summary>
    /// Gets the list of editable map items associated with this MUS editor state.
    /// </summary>
    public IList<EditableMapItem> MapItems { get; }



    /// <summary>
    /// Initializes a new instance of the <see cref="MusEditorState"/> class with the
    /// specified MUS file and MAP file.
    /// </summary>
    /// <param name="mus">The MUS file to be edited.</param>
    /// <param name="map">The MAP file associated with the MUS file.</param>
    public MusEditorState(MusFile mus, MapFile map)
    {
        _mus = mus;
        _map = map;
        MusStreams = GetObservable(mus.AsfSubStreams);
        MapItems = GetObservable(map.Items.Select(p => new EditableMapItem(p)).ToList());
    }

}

/// <summary>
/// Represents a mapped MUS element with its index, associated map item, and audio stream.
/// </summary>
/// <param name="index">The index of the element within the map.</param>
/// <param name="map">The map item associated with this MUS element.</param>
/// <param name="musSubStream">The ASF file sub-stream containing the audio data.</param>
public class MapMusElement(int index, MapItem map, AsfFile musSubStream)
{

}

/// <summary>
/// Represents a map item that can be edited within the MUS editor context.
/// </summary>
/// <param name="item">The underlying map item to be edited.</param>
public class EditableMapItem(MapItem item)
{

}