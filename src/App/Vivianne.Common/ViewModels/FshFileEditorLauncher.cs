using System.Collections.Generic;
using TheXDS.Ganymede.Types;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Tools;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a launcher to create and/or edit FSH and QFS files.
/// </summary>
public class FshFileEditorLauncher() : FileEditorViewModelLauncher<FshEditorState, FshFile, FshSerializer, FshEditorViewModel>("FSH/QFS", FileFilters.FshQfsOpenFileFilter, FileFilters.FshQfsSaveFileFilter)
{
    /// <inheritdoc/>
    public override RecentFileInfo[] RecentFiles
    {
        get => Settings.Current.RecentFshFiles;
        set => Settings.Current.RecentFshFiles = value;
    }

    /// <inheritdoc/>
    public override IEnumerable<ButtonInteraction> AdditionalInteractions => [
        IVivianneTool.ToInteraction<QfsDecompressTool>(DialogService!, NavigationService!),
        IVivianneTool.ToInteraction<FshCompressTool>(DialogService!, NavigationService!)
        ];

    /// <inheritdoc/>
    protected override void BeforeSave(FshFile state, string fileExtension)
    {
        base.BeforeSave(state, fileExtension);
        state.IsCompressed = System.IO.Path.GetExtension(fileExtension).Equals(".qfs", System.StringComparison.InvariantCultureIgnoreCase);
    }
}
