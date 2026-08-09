using System;
using System.Threading.Tasks;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Serializers.Viv;
using TheXDS.Vivianne.ViewModels.Viv;

namespace TheXDS.Vivianne.Data;

public static partial class FileTypes
{
    private static Task<IViewModel?> CreateVivEditorViewModel(Func<IBackingStore> backingStoreFactory, string fileName)
    {
        return CreateEditorViewModel<VivEditorViewModel, VivEditorState, VivFile, VivSerializer>(backingStoreFactory, fileName, s => s.Sort = Settings.Current.Viv_FileSorting);
    }
}
