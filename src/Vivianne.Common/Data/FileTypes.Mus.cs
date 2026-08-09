using System;
using System.Threading.Tasks;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.ViewModels.Asf;

namespace TheXDS.Vivianne.Data;

public static partial class FileTypes
{
    private static Task<IViewModel?> CreateMusPlayerViewModel(Func<IBackingStore> backingStoreFactory, string name)
    {
        return TryCreateViewModel(backingStoreFactory, name, async (data, store) => new MusPlayerViewModel()
        {
            Title = name,
            Mus = await ((ISerializer<MusFile>)new MusSerializer()).DeserializeAsync(data),
            FileName = name,
            BackingStore = store
        });
    }
}
