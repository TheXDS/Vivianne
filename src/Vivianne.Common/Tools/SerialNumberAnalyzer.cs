using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Services;
using St = TheXDS.Vivianne.Resources.Strings.Tools.SerialNumberAnalyzer;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that will analyze the 'CarModel' directory and fix serial
/// number duplication between VIV files.
/// </summary>
public class SerialNumberAnalyzer : IVivianneTool
{
    /// <inheritdoc/>
    public string ToolName => St.SerialNumberAnalyzerTitle;

    /// <inheritdoc/>
    public async Task Run(IDialogService dlg, INavigationService _)
    {
        var dirSelection = await dlg.GetDirectoryPath(CommonDialogTemplates.DirectorySelect with { Title = St.CarModelPath, Text = St.SelectTheCarModelDirectoryInsideNFS3GamedataFolder });
        if (!dirSelection.Success) return;
        await dlg.RunOperation(ToolName, p => Misc.SerialNumberAnalyzer.FixSerials(new DirectoryInfo(dirSelection.Result), new ProgressReportAdapter(p)));


        //if (!operation.Success) return;
        //if (operation.Result.Length == 0)
        //{
        //    await dlg.Message(ToolName, St.NoDuplicateConflictingMissingSerialNumbersFound);
        //}
        //else
        //{
        //    static string GetSnEntryMessage(SnEntry p) => $"{p.FilePath}: {p.SerialNumber.ToString().OrNull() ?? St.Undefined} -> {p.NewSerial}";
        //    await dlg.SelectAction(CommonDialogTemplates.Success with
        //    {
        //        Title = ToolName,
        //        Text = string.Format(St.OperationCompletedSuccessfully,operation.Result.Length)
        //    },
        //    [
        //        (Stc.Ok, () => Task.CompletedTask),
        //        (St.Details, () => dlg.Message(ToolName, string.Join(Environment.NewLine, operation.Result.Select(GetSnEntryMessage))))
        //    ]);
        //}
    }
}
