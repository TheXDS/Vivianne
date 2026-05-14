using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.ViewModels.Mus;

public class MusEditorState : EditorViewModelStateBase
{
    private MusFile _mus;
    private MapFile _map;

    public IDictionary<int, AsfFile> MusStreams { get; }

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

public class MapMusElement(int index, MapItem map, AsfFile musSubStream)
{

}

public class EditableMapItem(MapItem item)
{

}