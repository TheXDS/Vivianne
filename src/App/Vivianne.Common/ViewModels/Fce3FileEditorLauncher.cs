using System;
using TheXDS.Ganymede.Services;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a launcher to create and/or edit FCE files for NFS3.
/// </summary>
public class Fce3FileEditorLauncher(Func<IDialogService> dialogSvc) : FileEditorViewModelLauncher<FceEditorState, FceFile, FceSerializer, Fce3EditorViewModel>(dialogSvc, "FCE3", FileFilters.FceFileFilter)
{
    /// <inheritdoc/>
    public override RecentFileInfo[] RecentFiles
    {
        get => Settings.Current.RecentFce3Files;
        set => Settings.Current.RecentFce3Files = value;
    }
}