using System;
using System.IO;
using TheXDS.Ganymede.Services;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers.Viv;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Viv;

/// <summary>
/// Implements a launcher to create and/or edit VIV files.
/// </summary>
public class VivFileEditorLauncher : FileEditorViewModelLauncher<VivEditorState, VivFile, VivSerializer, VivEditorViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VivFileEditorLauncher"/> class.
    /// </summary>
    public VivFileEditorLauncher(Func<IDialogService> dialogSvc) : base(dialogSvc, "VIV", FileFilters.VivFileFilter)
    {
        Serializer.Sort = () => Settings.Current.Viv_FileSorting;
    }

    /// <inheritdoc/>
    public override RecentFileInfo[] RecentFiles
    {
        get => Settings.Current.RecentVivFiles;
        set => Settings.Current.RecentVivFiles = value;
    }

    /// <inheritdoc/>
    protected override RecentFileInfo CreateRecentFileInfo(string path, VivFile viv)
    {
        return new()
        {
            FilePath = path,
            FriendlyName = viv.GetFriendlyName() ?? InferFromPath(path)
        };
    }

    private static string InferFromPath(string path)
    {
        return path.Equals("car.viv", StringComparison.InvariantCultureIgnoreCase)
            ? Path.GetFileName(Path.GetDirectoryName(path)) ?? Path.GetFileName(path)
            : Path.GetFileName(path);
    }
}