using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Properties;

namespace TheXDS.Vivianne.ViewModels;

public class SettingsViewModel : EditorViewModelBase<SettingsState>
{
    public ICommand BrowseNfs3PathCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/>
    /// class.
    /// </summary>
    public SettingsViewModel() : base(Settings.Current)
    {
        var cb = CommandBuilder.For(this);
        BrowseNfs3PathCommand = cb.BuildSimple(OnBrowseNfs3Path);
    }

    private async Task OnBrowseNfs3Path()
    {
        var result = await DialogService!.GetFileOpenPath(
            CommonDialogTemplates.DirectorySelect with { Title = "Root NFS3 game path" },
            [new FileFilterItem("nfs3.exe", "nfs3.exe")],
            Path.Combine(State.Nfs3Path ?? Environment.CurrentDirectory, "nfs3.exe"));
        if (result.Success)
        {
            State.Nfs3Path = Path.GetDirectoryName(result.Result);
        }
    }

    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        Settings.Current = State;
        return Settings.Save();
    }
}
