using System;
using System.Threading.Tasks;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.ViewModels.Fe;
using Fe3 = TheXDS.Vivianne.Models.Fe.Nfs3.FeData;
using Fe4 = TheXDS.Vivianne.Models.Fe.Nfs4.FeData;
using SFe3 = TheXDS.Vivianne.Serializers.Fe.Nfs3.FeDataSerializer;
using SFe4 = TheXDS.Vivianne.Serializers.Fe.Nfs4.FeDataSerializer;

namespace TheXDS.Vivianne.Data;

public static partial class FileTypes
{
    private static Task<IViewModel?> CreateFeDataEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return TryCreateViewModel(backingStoreFactory, name, (data, store) => VersionIdentifier.FeDataVersion(data) switch
        {
            NfsVersion.Nfs3 => CreateEditorViewModel<FeData3EditorViewModel, FeData3EditorState, Fe3, SFe3>(data, store, name),
            NfsVersion.Nfs4 => CreateEditorViewModel<FeData4EditorViewModel, FeData4EditorState, Fe4, SFe4>(data, store, name),
            _ => Task.FromResult<IViewModel?>(null)
        });
    }
}
