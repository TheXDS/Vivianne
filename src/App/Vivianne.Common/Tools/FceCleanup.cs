//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using TheXDS.Ganymede.Resources;
//using TheXDS.Ganymede.Services;
//using TheXDS.Ganymede.Types.Extensions;
//using TheXDS.MCART.Helpers;
//using TheXDS.MCART.Types;
//using TheXDS.MCART.Types.Extensions;
//using TheXDS.Vivianne.Models;
//using TheXDS.Vivianne.Resources;
//using TheXDS.Vivianne.Serializers;
//using St = TheXDS.Vivianne.Resources.Strings.Tools.FceCleanupTool;

//namespace TheXDS.Vivianne.Tools;

///// <summary>
///// Implements a tool to clean up an FCE file.
///// </summary>
//public class FceCleanup : IVivianneTool
//{
//    private static ISerializer<Fce3File> s = new Fce3Serializer();

//    /// <inheritdoc/>
//    public async Task Run(IDialogService dialogService, INavigationService navigationService)
//    {
//        var file = await dialogService.GetFileOpenPath(FileFilters.FceFileFilter);
//        if (!file.Success) return;
//        (var fce, var problems) = await dialogService.RunOperation((progress) => ParseAndVerifyFce(progress, File.OpenRead(file.Result)));
//        if (problems.Length > 0)
//        {
//            bool questionLoop;
//            NamedObject<Func<Task>>[] actions =
//            [
//                (St.Yes, async () =>
//                {
//                    await dialogService.RunOperation((progress) => FixFce(progress, fce, problems, file.Result));
//                    await dialogService.Message(St.FCECleanup, St.FCEDataHasBeenCleanedUp);
//                }),
//                (St.No, () => Task.CompletedTask),
//                (St.MoreDetails, async () => {
//                    await dialogService.Message(string.Format(St.XProblemsFound, problems.Length), string.Join($"{Environment.NewLine}{Environment.NewLine}", problems.Select(p => p.Details).ToArray()));
//                    questionLoop = true;
//                })
//            ];
//            do
//            {
//                questionLoop = false;
//                await dialogService.SelectAction(CommonDialogTemplates.Question with { Text = string.Format(St.XProblemsFoundQuestion, problems.Length) }, actions);
//            } while (questionLoop);
//        }
//        else
//        {
//            await dialogService.Message(St.NoProblemsFound, St.TheFCEFileDataIsValid);
//        }
//    }

//    private static async Task<(Fce3File, FceCleanupResult[])> ParseAndVerifyFce(IProgress<Ganymede.Models.ProgressReport> progress, Stream fceStream)
//    {
//        List<FceCleanupResult> problems = [];
//        progress.Report(St.ParsingFCEFile);
//        var fce = await s.DeserializeAsync(fceStream);
//        progress.Report(St.VerifyingFCEData);
//        foreach (var j in FceCleanupTool.GetWarnings(fce))
//        {
//            if (j is not null)
//            {
//                problems.Add(j);
//                progress.Report(string.Format(St.XProblemsFound,problems.Count));
//            }
//        }
//        fceStream.Close();
//        await fceStream.DisposeAsync();
//        return (fce, problems.NotNull().ToArray());
//    }

//    private static async Task FixFce(IProgress<Ganymede.Models.ProgressReport> progress, Fce3File fce, FceCleanupResult[] problems, string savePath)
//    {
//        foreach ((var index, var j) in problems.WithIndex())
//        {
//            progress.Report(new(index * 100.0 / problems.Length, string.Format(St.FixingX,j.Title)));
//            await Task.Run(() => j.CleanupAction.Invoke(fce));
//        }
//        progress.Report(St.SavingFile);
//        using var fs = File.OpenWrite(savePath);
//        fs.SetLength(0);
//        s.SerializeTo(fce, fs);
//    }
//}
