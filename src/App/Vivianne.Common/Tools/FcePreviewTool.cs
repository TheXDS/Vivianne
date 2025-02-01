using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using System.IO;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ViewModels;
using TheXDS.Ganymede.Types.Extensions;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that lets the user open an FCE file directly.
/// </summary>
public class FcePreviewTool : IVivianneTool
{
    /// <inheritdoc/>
    public async Task Run(IDialogService dialogService, INavigationService navigationService)
    {
        var fin = await dialogService.GetFileOpenPath(FileFilters.FceFileFilter);
        if (!fin.Success) return;
        navigationService.Navigate(CreateViewModel(await LoadFce(fin.Result!), fin.Result!));
    }

    private static async Task<FceFile> LoadFce(string fileName)
    {
        return await ((ISerializer<FceFile>)new FceSerializer()).DeserializeAsync(await File.ReadAllBytesAsync(fileName));
    }

    private static FceEditorViewModel CreateViewModel(FceFile fce, string fileName)
    {
        return new FceEditorViewModel(fce) { Title = fileName };
    }
}