using System;
using System.Threading.Tasks;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Serializers.Audio.Bnk;
using TheXDS.Vivianne.ViewModels.Bnk;

namespace TheXDS.Vivianne.Data;

public static partial class FileTypes
{
    private static Task<IViewModel?> CreateBnkEditorViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return CreateEditorViewModel<BnkEditorViewModel, BnkEditorState, BnkFile, BnkSerializer>(backingStoreFactory, name, s => s.EnableStreamDedup = Settings.Current.Bnk_EnableStreamDeduplication);
    }
}
