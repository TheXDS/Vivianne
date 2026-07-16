using System;
using System.Threading.Tasks;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.Data;

public static partial class FileTypes
{
    private static Task<IViewModel?> CreateTexturePreviewViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return CreateEditorViewModel<TexturePreviewViewModel, RawFileEditorState, RawFile, RawFileSerializer>(backingStoreFactory, name);
    }
}
