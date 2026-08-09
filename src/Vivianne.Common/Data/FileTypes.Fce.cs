using System;
using System.Threading.Tasks;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.ViewModels.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Fce.Nfs4;
using MFce3 = TheXDS.Vivianne.Models.Fce.Nfs3;
using MFce4 = TheXDS.Vivianne.Models.Fce.Nfs4;
using SNfs3 = TheXDS.Vivianne.Serializers.Fce.Nfs3.FceSerializer;
using SNfs4 = TheXDS.Vivianne.Serializers.Fce.Nfs4.FceSerializer;
using VmFce3 = TheXDS.Vivianne.ViewModels.Fce.Nfs3.Fce3EditorViewModel;
using VmFce4 = TheXDS.Vivianne.ViewModels.Fce.Nfs4.Fce4EditorViewModel;

namespace TheXDS.Vivianne.Data;

public static partial class FileTypes
{
    private static Task<IViewModel?> CreateFceEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return TryCreateViewModel(backingStoreFactory, name, (data, store) =>  VersionIdentifier.FceVersion(data) switch
        {
            NfsVersion.Nfs3 => CreateEditorViewModel<VmFce3, Fce3EditorState, MFce3.FceFile, SNfs3>(data, store, name),
            NfsVersion.Nfs4 or NfsVersion.Mco => CreateEditorViewModel<VmFce4, Fce4EditorState, MFce4.FceFile, SNfs4>(data, store, name),
            _ => Task.FromResult<IViewModel?>(null)
        });
    }
}
