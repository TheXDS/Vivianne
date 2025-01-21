#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Tools;

public class FceCleanup : IVivianneTool
{
    private static ISerializer<FceFile> s = new FceSerializer();
    public async Task Run(IDialogService dialogService, INavigationService navigationService)
    {
        var file = await dialogService.GetFileOpenPath(FileFilters.FceFileFilter);
        if (!file.Success) return;
        (var fce, var problems) = await dialogService.RunOperation((progress) => ParseAndVerifyFce(progress, File.OpenRead(file.Result)));
        if (problems.Length > 0)
        {
        ask:
            switch ((await dialogService.SelectOption(CommonDialogTemplates.Question with { Text = $"{problems.Length} problems found. Would you like to solve them?" }, "Yes", "No", "More details")).Result)
            {
                case 0: break;
                case 2:
                    await dialogService.Message($"{problems.Length} problems found...", string.Join($"{Environment.NewLine}{Environment.NewLine}", problems.Select(p => p.Details).ToArray()));
                    goto ask;
                default: return;
            }
            await dialogService.RunOperation((progress) => FixFce(progress, fce, problems, file.Result));
            await dialogService.Message("FCE cleanup", "FCE data has been cleaned up.");
        }
        else
        {
            await dialogService.Message("No problems found", "The FCE file data is valid.");
        }
    }

    private static async Task<(FceFile, FceCleanupResult[])> ParseAndVerifyFce(IProgress<Ganymede.Models.ProgressReport> progress, Stream fceStream)
    {
        List<FceCleanupResult> problems = [];
        progress.Report("Parsing FCE file...");
        var fce = await s.DeserializeAsync(fceStream);
        progress.Report("Verifying FCE data...");
        foreach (var j in FceCleanupTool.GetWarnings(fce))
        {
            if (j is not null)
            {
                problems.Add(j);
                progress.Report($"{problems.Count} problems found...");
            }
        }
        fceStream.Close();
        await fceStream.DisposeAsync();
        return (fce, problems.NotNull().ToArray());
    }

    private static async Task FixFce(IProgress<Ganymede.Models.ProgressReport> progress, FceFile fce, FceCleanupResult[] problems, string savePath)
    {
        foreach ((var index, var j) in problems.WithIndex())
        {
            progress.Report(new(index * 100.0 / problems.Length, $"Fixing: {j.Title}..."));
            await Task.Run(() => j.CleanupAction.Invoke(fce));
        }
        progress.Report("Saving file...");
        using var fs = File.OpenWrite(savePath);
        fs.SetLength(0);
        s.SerializeTo(fce, fs);
    }
}
