using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using System.IO;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ViewModels;
using St = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that lets the user open an FCE file directly.
/// </summary>
public class FcePreviewTool : IVivianneTool
{
    /// <inheritdoc/>
    public async Task Run(IDialogService dialogService, INavigationService navigationService)
    {
        var fin = await dialogService.GetFileOpenPath(St.Open, "", FileFilters.FceFileFilter);
        if (!fin.Success) return;
        navigationService.Navigate(CreateViewModel(await LoadFce(fin.Result), dialogService, fin.Result));
    }

    private static async Task<FceFile> LoadFce(string fileName)
    {
        var rawData = await File.ReadAllBytesAsync(fileName);
        var data = QfsCodec.Decompress(rawData);
        return await ((ISerializer<FceFile>)new FceSerializer()).DeserializeAsync(data);
    }

    private static FcePreviewViewModel CreateViewModel(FceFile fce, IDialogService dialogService, string fileName)
    {
        return new FcePreviewViewModel(fce);
    }
}