using System.IO;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a launcher to create and/or edit VIV files.
/// </summary>
public class VivFileEditorLauncher() : FileEditorViewModelLauncher<VivEditorState, VivFile, VivSerializer, VivEditorViewModel>("VIV", FileFilters.VivFileFilter)
{
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
        return Path.GetFileName(Path.GetDirectoryName(path)) ?? Path.GetFileName(path);
    }
}