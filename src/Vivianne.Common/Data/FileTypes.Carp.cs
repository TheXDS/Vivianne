using System;
using System.Threading.Tasks;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Info;
using Cp2 = TheXDS.Vivianne.Models.Carp.Nfs2.CarPerf;
using Cp3 = TheXDS.Vivianne.Models.Carp.Nfs3.CarPerf;
using Cp4 = TheXDS.Vivianne.Models.Carp.Nfs4.CarPerf;
using SCp2 = TheXDS.Vivianne.Serializers.Carp.Nfs2.CarpSerializer;
using SCp3 = TheXDS.Vivianne.Serializers.Carp.Nfs3.CarpSerializer;
using SCp4 = TheXDS.Vivianne.Serializers.Carp.Nfs4.CarpSerializer;
using VCp2 = TheXDS.Vivianne.ViewModels.Carp.Nfs2.CarpEditorViewModel;
using VCp3 = TheXDS.Vivianne.ViewModels.Carp.Nfs3.CarpEditorViewModel;
using VCp4 = TheXDS.Vivianne.ViewModels.Carp.Nfs4.CarpEditorViewModel;
using VsCp2 = TheXDS.Vivianne.Models.Carp.Nfs2.CarpEditorState;
using VsCp3 = TheXDS.Vivianne.Models.Carp.Nfs3.CarpEditorState;
using VsCp4 = TheXDS.Vivianne.Models.Carp.Nfs4.CarpEditorState;

namespace TheXDS.Vivianne.Data;

public static partial class FileTypes
{
    private static Task<IViewModel?> CreateCarpEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return TryCreateViewModel(backingStoreFactory, name, (data, store) => VersionIdentifier.CarpVersion(data) switch
        {
            NfsVersion.Nfs2 => CreateEditorViewModel<VCp2, VsCp2, Cp2, SCp2>(data, store, name),
            NfsVersion.Nfs3 => CreateEditorViewModel<VCp3, VsCp3, Cp3, SCp3>(data, store, name),
            NfsVersion.Nfs4 => CreateEditorViewModel<VCp4, VsCp4, Cp4, SCp4>(data, store, name),
            _ => Task.FromResult<IViewModel?>(null)
        });
    }
}
