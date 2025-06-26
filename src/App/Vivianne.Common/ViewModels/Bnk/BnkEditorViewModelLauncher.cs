using System;
using TheXDS.Ganymede.Services;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers.Audio.Bnk;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Bnk;

/// <summary>
/// Provides functionality to launch and manage the BNK file editor view model.
/// </summary>
/// <param name="dialogSvc">
/// Dialog service used to create dialogs for the launcher.
/// </param>
public class BnkEditorViewModelLauncher(Func<IDialogService> dialogSvc) : FileEditorViewModelLauncher<BnkEditorState, BnkFile, BnkSerializer, BnkEditorViewModel>(dialogSvc, "BNK", FileFilters.BnkFileFilter)
{
    /// <inheritdoc/>
    public override RecentFileInfo[] RecentFiles
    {
        get => Settings.Current.RecentBnkFiles;
        set => Settings.Current.RecentBnkFiles = value;
    }
}