using System;
using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.ViewModels;
using St = TheXDS.Vivianne.Resources.Strings.Common;
using St2 = TheXDS.Vivianne.Resources.Strings.Tools.FshEditorTool;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that allows the user to edit FSH and QFS files directly.
/// </summary>
public class FshEditorTool : IVivianneTool
{
    /// <inheritdoc/>
    public string ToolName => St2.ToolName;

    /// <inheritdoc/>
    public async Task Run(IDialogService dialogService, INavigationService navigationService)
    {
        var fin = await dialogService.GetFileOpenPath(St2.OpnMessage, FileFilters.FshQfsOpenFileFilter);
        if (!fin.Success) return;
        var (fsh, isCompressed) = await LoadFsh(fin.Result);
        navigationService.Navigate(CreateViewModel(fsh, dialogService, fin.Result, isCompressed));
    }

    private static FshEditorViewModel CreateViewModel(FshFile fsh, IDialogService dialogService, string fileName, bool isCompressed)
    {
        return new FshEditorViewModel(fsh, f => dialogService.RunOperation(p => SaveFsh(isCompressed, p, f, fileName)));
    }

    private static async Task<(FshFile fsh, bool isCompressed)> LoadFsh(string fileName)
    {
        var rawData = await File.ReadAllBytesAsync(fileName);
        var data = QfsCodec.Decompress(rawData);
        return (await ((ISerializer<FshFile>)new FshSerializer()).DeserializeAsync(data), QfsCodec.IsCompressed(rawData));
    }

    private static async Task SaveFsh(bool isCompressed, IProgress<ProgressReport> p, FshFile fsh, string fileName)
    {
        p.Report(string.Format(St.SavingX, Path.GetFileName(fileName)));
        var rawContent = ((ISerializer<FshFile>)new FshSerializer()).Serialize(fsh);
        await File.WriteAllBytesAsync(fileName, isCompressed ? QfsCodec.Compress(rawContent) : rawContent);
    }
}
