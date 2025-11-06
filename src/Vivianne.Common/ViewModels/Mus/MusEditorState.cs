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