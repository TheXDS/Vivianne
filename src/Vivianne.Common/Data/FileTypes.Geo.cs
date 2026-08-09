using System;
using System.Threading.Tasks;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models.Geo;
using TheXDS.Vivianne.Serializers.Geo;
using TheXDS.Vivianne.ViewModels.Geo;

namespace TheXDS.Vivianne.Data;

public static partial class FileTypes
{
    private static Task<IViewModel?> CreateGeoEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return CreateEditorViewModel<GeoEditorViewModel, GeoEditorState, GeoFile, GeoSerializer>(backingStoreFactory, name);
    }
}
