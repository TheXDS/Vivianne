using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Asf;

/// <summary>
/// Implements a launcher to play ASF/MUS files in a read-only file ViewModel.
/// </summary>
public class MusPlayerViewModelLauncher() : FileViewModelLauncherBase<MusFile, MusSerializer, MusPlayerViewModel>("ASF/MUS", FileFilters.AsfMusOpenFilter)
{
    /// <inheritdoc/>
    public override RecentFileInfo[] RecentFiles
    {
        get => Settings.Current.RecentAsfFiles;
        set => Settings.Current.RecentAsfFiles = value;
    }

    /// <inheritdoc/>
    protected override MusPlayerViewModel CreateViewModel(string? friendlyName, MusFile file, string _)
    {
        return new MusPlayerViewModel()
        {
            Title = friendlyName,
            Mus = file,
        };
    }
}