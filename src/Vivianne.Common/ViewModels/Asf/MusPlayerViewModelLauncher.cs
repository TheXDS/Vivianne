using System.IO;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Vivianne.Component;
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

    /// <summary>
    /// Gets a reference to the command used to create a new file, which for this class is an invalid command.
    /// </summary>
    public ICommand NewFileCommand { get; } = CommandBuilder.For<MusPlayerViewModelLauncher>(null!).BuildInvalid();

    /// <summary>
    /// Gets a value that determines if the "New" button should be available.
    /// </summary>
    public bool CanCreateNew => false;

    /// <inheritdoc/>
    protected override MusPlayerViewModel CreateViewModel(string? friendlyName, MusFile file, string filePath)
    {
        return new MusPlayerViewModel()
        {
            Title = friendlyName,
            Mus = file,
            FileName = Path.GetFileName(filePath),
            BackingStore = new FileSystemBackingStore(DialogService!, [], filePath)
        };
    }
}