﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.ViewModels.Base;

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
    public ICommand BrowseNfs3PathCommand { get; }

    /// <summary>
    /// Gets a command used to browse for the NFS3 main directory.
    /// </summary>
    public ICommand BrowseNfs4PathCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/>
    /// class.
    /// </summary>
    public SettingsViewModel() : base(Settings.Current)
    {
        var cb = CommandBuilder.For(this);
        BrowseNfs3PathCommand = cb.BuildSimple(OnBrowseNfs3Path);
        BrowseNfs4PathCommand = cb.BuildSimple(OnBrowseNfs4Path);
    }

    private async Task OnBrowseNfs3Path()
    {
        var result = await DialogService!.GetFileOpenPath(
            CommonDialogTemplates.DirectorySelect with { Title = TheXDS.Vivianne.Resources.Strings.ViewModels.SettingsViewModel.RootNFS3GamePath },
            [new FileFilterItem("nfs3.exe", "nfs3.exe")],
            Path.Combine(State.Nfs3Path ?? Environment.CurrentDirectory, "nfs3.exe"));
        if (result.Success)
        {
            State.Nfs3Path = Path.GetDirectoryName(result.Result);
        }
    }
    private async Task OnBrowseNfs4Path()
    {
        var result = await DialogService!.GetFileOpenPath(
            CommonDialogTemplates.DirectorySelect with { Title = TheXDS.Vivianne.Resources.Strings.ViewModels.SettingsViewModel.RootToNFS4GamePath },
            [new FileFilterItem("nfs4.exe / nfshs.exe", ["nfs4.exe", "nfshs.exe"])],
            Path.Combine(State.Nfs4Path ?? Environment.CurrentDirectory, "nfs4.exe"));
        if (result.Success)
        {
            State.Nfs4Path = Path.GetDirectoryName(result.Result);
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
        return Settings.Save();
    }
}
