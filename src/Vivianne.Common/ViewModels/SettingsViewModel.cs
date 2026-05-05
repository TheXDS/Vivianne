using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.SettingsViewModel;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to edit the settings for
/// Vivianne.
/// </summary>
public class SettingsViewModel : EditorViewModelBase<SettingsState>
{
    /// <summary>
    /// Gets a string that describes the current app version.
    /// </summary>
    public string Version
    {
        get
        {
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return asm.GetAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
                asm.GetAttribute<AssemblyVersionAttribute>()?.Version ??
                "1.0.0";
        }
    }

    /// <summary>
    /// Gets a command used to browse for the NFS3 main directory.
    /// </summary>
    public ICommand BrowseNfs2PathCommand { get; }

    /// <summary>
    /// Gets a command used to browse for the NFS3 main directory.
    /// </summary>
    public ICommand BrowseNfs3PathCommand { get; }

    /// <summary>
    /// Gets a command used to browse for the NFS3 main directory.
    /// </summary>
    public ICommand BrowseNfs4PathCommand { get; }

    /// <summary>
    /// Gets a command used to set file associations for Vivianne.
    /// </summary>
    public ICommand SetFileAssociationsCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/>
    /// class.
    /// </summary>
    public SettingsViewModel() : base(Settings.Current)
    {
        var cb = CommandBuilder.For(this);
        BrowseNfs2PathCommand = cb.BuildSimple(OnBrowseNfs2Path);
        BrowseNfs3PathCommand = cb.BuildSimple(OnBrowseNfs3Path);
        BrowseNfs4PathCommand = cb.BuildSimple(OnBrowseNfs4Path);
        SetFileAssociationsCommand = cb.BuildBusyOperation(OnSetFileAssociations);
    }

    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        State?.Subscribe(p => p.Nfs2Path, (o, p, n) => Notify(nameof(AvailableNfs2ExeNames)));
        return base.OnCreated();
    }

    /// <summary>
    /// Gets the collection of available Need for Speed II executable file names, including the ".exe" extension.
    /// </summary>
    public IEnumerable<string> AvailableNfs2ExeNames => GlobalConstants.KnownNfs2ProcesNames.Select(p => $"{p}.exe").Where(p => File.Exists(Path.Combine(State.Nfs2Path ?? Environment.CurrentDirectory, p)));

    private Task OnSetFileAssociations()
    {
        return PlatformServices.OperatingSystem.InvokeSelfCallback("a8d0e6c8-2410-460c-ab29-7682c351a313");
    }

    private async Task OnBrowseNfs2Path()
    {
        try
        {
            IsBusy = true;
            var result = await DialogService!.GetFileOpenPath(
                CommonDialogTemplates.DirectorySelect with { Title = St.RootNFS2GamePath },
                [new FileFilterItem("nfs2 / nfs2se", ["nfs2.exe", "nfs2se.exe"])],
                Path.Combine(State.Nfs2Path ?? Environment.CurrentDirectory, "nfs2.exe"));
            if (result.Success)
            {
                State.Nfs2Path = Path.GetDirectoryName(result.Result);
            }
        }
        catch
        {
            State.Nfs2Path = null;
            await OnBrowseNfs2Path();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnBrowseNfs3Path()
    {
        try
        {
            IsBusy = true;
            var result = await DialogService!.GetFileOpenPath(
                CommonDialogTemplates.DirectorySelect with { Title = St.RootNFS3GamePath },
                [new FileFilterItem("nfs3.exe", "nfs3.exe")],
                Path.Combine(State.Nfs3Path ?? Environment.CurrentDirectory, "nfs3.exe"));
            if (result.Success)
            {
                State.Nfs3Path = Path.GetDirectoryName(result.Result);
            }
        }
        catch
        {
            State.Nfs3Path = null;
            await OnBrowseNfs3Path();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnBrowseNfs4Path()
    {
        try
        {
            IsBusy = true;
            var result = await DialogService!.GetFileOpenPath(
            CommonDialogTemplates.DirectorySelect with { Title = St.RootToNFS4GamePath },
            [new FileFilterItem("nfs4.exe / nfshs.exe", ["nfs4.exe", "nfshs.exe"])],
            Path.Combine(State.Nfs4Path ?? Environment.CurrentDirectory, "nfs4.exe"));
            if (result.Success)
            {
                State.Nfs4Path = Path.GetDirectoryName(result.Result);
            }
        }
        catch
        {
            State.Nfs4Path = null;
            await OnBrowseNfs3Path();
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        var recentVivFiles = Settings.Current.RecentVivFiles;
        var recentFshFiles = Settings.Current.RecentFshFiles;
        var recentFce3Files = Settings.Current.RecentFceFiles;
        Settings.Current = State;
        Settings.Current.RecentVivFiles = recentVivFiles;
        Settings.Current.RecentFshFiles = recentFshFiles;
        Settings.Current.RecentFceFiles = recentFce3Files;
        return Settings.SaveAsync();
    }
}
