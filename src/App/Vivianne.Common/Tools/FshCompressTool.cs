using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Vivianne.Resources;
using St = TheXDS.Vivianne.Resources.Strings.Tools.FshCompressTool;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that allows the user to compress a FSH file.
/// </summary>
public class FshCompressTool : IVivianneTool
{
    /// <inheritdoc/>
    public string ToolName => St.ToolName;

    /// <inheritdoc/>
    public async Task Run(IDialogService dialogService, INavigationService _)
    {
        var fin = await dialogService.GetFileOpenPath(St.OpnMessage, FileFilters.FshFileFilter);
        if (!fin.Success) return;
        var fout = await dialogService.GetFileSavePath(St.SveMessage, FileFilters.QfsFileFilter);
        if (!fout.Success) return;
        await dialogService.RunOperation(async p =>
        {
            p.Report(St.ProcessMsg);
            var fsh = await File.ReadAllBytesAsync(fin.Result);
            var qfs = await Task.Run(() => QfsCodec.Compress(fsh));
            await File.WriteAllBytesAsync(fout.Result, qfs);
        });
    }
}

