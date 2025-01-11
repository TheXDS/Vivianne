using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Vivianne.Resources;
using St = TheXDS.Vivianne.Resources.Strings.Common;
using St2 = TheXDS.Vivianne.Resources.Strings.Tools.QfsDecompressTool;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that allows the user to decompress a QFS file.
/// </summary>
public class QfsDecompressTool : IVivianneTool
{
    /// <inheritdoc/>
    public string ToolName => St2.ToolName;

    /// <inheritdoc/>
    public async Task Run(IDialogService dialogService, INavigationService _)
    {
        var fin = await dialogService.GetFileOpenPath(St2.OpnMessage, FileFilters.QfsFileFilter);
        if (!fin.Success) return;
        var fout = await dialogService.GetFileSavePath(St2.SveMessage, FileFilters.FshFileFilter);
        if (!fout.Success) return;
        await dialogService.RunOperation(async p =>
        {
            p.Report(St2.ProcessMsg);
            var qfs = await File.ReadAllBytesAsync(fin.Result);
            var fsh = await Task.Run(() => QfsCodec.Decompress(qfs));
            await File.WriteAllBytesAsync(fout.Result, fsh);
        });
    }
}
