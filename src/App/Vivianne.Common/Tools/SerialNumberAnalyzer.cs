using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using St = TheXDS.Vivianne.Resources.Strings.Tools.SerialNumberAnalyzer;
using Stc = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that will analyze the 'CarModel' directory and fix serial
/// number duplication between VIV files.
/// </summary>
public class SerialNumberAnalyzer : IVivianneTool
{
    private class SnEntry
    {
        public readonly Dictionary<string, FeData> FeDatas = [];
        public Carp? Carp;
        public string FilePath = null!;
        public VivFile VivFile = null!;
        public ushort? SerialNumber;
        public ushort NewSerial;
    }

    private static readonly EnumerationOptions EnumOpts = new() { MatchCasing = MatchCasing.CaseInsensitive };
    private static readonly ISerializer<FeData> feSerializer = new FeDataSerializer();
    private static readonly ISerializer<VivFile> vivSerializer = new VivSerializer();
    private static readonly ISerializer<Carp> carpSerializer = new CarpSerializer();

    private static async Task<SnEntry?> LoadViv(FileInfo file)
    {
        try
        {
            using var f = file.OpenRead();
            return new SnEntry()
            {
                FilePath = file.FullName,
                VivFile = await vivSerializer.DeserializeAsync(f),
            };
        }
        catch (Exception ex)
        {
            Debug.Print(string.Format(St.ErrorLoadingVIVFile,file.Name ,ex.Message));
            return null;
        }
    }

    private static async Task<FeData?> LoadFeData(VivFile viv, string feName)
    {
        try
        {
            if (viv.Directory.TryGetValue(feName, out var feContents))
            {
                return await feSerializer.DeserializeAsync(feContents);
            }
        }
        catch (Exception ex)
        {
            Debug.Print(string.Format(St.ErrorLoadingFeData, feName,ex.Message));
        }
        return null;
    }

    private static async Task<Carp?> LoadCarp(VivFile viv)
    {
        try
        {
            if (viv.Directory.TryGetValue("carp.txt", out var carp))
            {
                return await carpSerializer.DeserializeAsync(carp);
            }
        }
        catch (Exception ex)
        {
            Debug.Print(string.Format(St.ErrorLoadingCarpTxt, ex.Message));
        }
        return null;
    }

    private static async Task ParseSnFromViv(SnEntry entry)
    {
        HashSet<ushort> serials = [];
        foreach (var fe in FeData.KnownExtensions)
        {
            var feName = $"fedata{fe}";
            if (await LoadFeData(entry.VivFile, feName) is { } feData)
            {
                entry.FeDatas.Add(feName, feData);
                serials.Add(feData.SerialNumber);
            }
        }
        if (await LoadCarp(entry.VivFile) is { } carp)
        {
            entry.Carp = carp;
            serials.Add(carp.SerialNumber);
        }

        entry.SerialNumber = serials.Count == 1 ? serials.Single() : null;
    }

    private static async IAsyncEnumerable<SnEntry> LoadEntries(FileInfo[] files, IProgress<ProgressReport> progress, [EnumeratorCancellation] CancellationToken cancel)
    {
        var c = 0;
        foreach (var file in files)
        {
            progress.Report(new(c++ * 100 / files.Length, string.Format(St.LoadingX, file.FullName)));
            if (await LoadViv(file) is { } entry)
            {
                await ParseSnFromViv(entry);
                if (cancel.IsCancellationRequested) yield break;
                yield return entry;
            }
        }
    }

    private static async Task<(ushort[] uniqueSerials, SnEntry[] badEntries)> GetEntriesToProcess(IProgress<ProgressReport> progress, string rootDirectory, CancellationToken cancel)
    {
        IEnumerable<FileInfo> files = Directory
            .GetDirectories(rootDirectory)
            .Select(p => new DirectoryInfo(p).GetFiles("car.viv", EnumOpts).FirstOrDefault())
            .NotNull();
        List<ushort> serials = [];
        List<SnEntry> entries = [];

        await foreach (var entry in LoadEntries(files.ToArray(), progress, cancel))
        {
            if (entry.SerialNumber is not null && !serials.Contains(entry.SerialNumber.Value))
            {
                serials.Add(entry.SerialNumber.Value);
            }
            else
            {
                entries.Add(entry);
            }
        }
        return (serials.ToArray(), entries.ToArray());
    }

    private static async Task FixSerials(ushort[] serials, SnEntry[] entries, IProgress<ProgressReport> progress, CancellationToken cancel)
    {
        Random rnd = new();
        var c = 0;
        foreach (var entry in entries)
        {
            if (cancel.IsCancellationRequested) return;
            ushort newSerial;
            do
            {
                newSerial = (ushort)rnd.Next(1, ushort.MaxValue);
            } while (serials.Contains(newSerial));
            entry.NewSerial = newSerial;
            foreach (var fe in entry.FeDatas)
            {
                fe.Value.SerialNumber = newSerial;
                entry.VivFile.Directory[fe.Key] = feSerializer.Serialize(fe.Value);
            }
            if (entry.Carp is not null) entry.Carp.SerialNumber = newSerial;
            progress.Report(new(c++ * 100 / entries.Length, string.Format(St.WritingX, entry.FilePath)));
            using var vivF = File.Open(entry.FilePath, FileMode.Truncate);
            await vivSerializer.SerializeToAsync(entry.VivFile, vivF);
        }
    }

    private static async Task<SnEntry[]> RunSnAnalysis(IProgress<ProgressReport> progress, string rootDirectory, CancellationToken cancel)
    {
        (var serials, var entries) = await GetEntriesToProcess(progress, rootDirectory, cancel);
        await FixSerials(serials, entries, progress, cancel);
        return entries;
    }

    /// <inheritdoc/>
    public string ToolName => St.SerialNumberAnalyzer;

    /// <inheritdoc/>
    public async Task Run(IDialogService dlg, INavigationService _)
    {
        var dirSelection = await dlg.GetDirectoryPath(CommonDialogTemplates.DirectorySelect with { Title = St.CarModelPath, Text = St.SelectTheCarModelDirectoryInsideNFS3GamedataFolder });
        if (!dirSelection.Success) return;

        var operation = await dlg.RunOperation(ToolName, (c, p) => RunSnAnalysis(p, dirSelection.Result!, c));
        if (!operation.Success) return;
        if (operation.Result.Length == 0)
        {
            await dlg.Message(ToolName, St.NoDuplicateConflictingMissingSerialNumbersFound);
        }
        else
        {
            static string GetSnEntryMessage(SnEntry p) => $"{p.FilePath}: {p.SerialNumber.ToString().OrNull() ?? St.Undefined} -> {p.NewSerial}";
            await dlg.SelectAction(CommonDialogTemplates.Success with
            {
                Title = ToolName,
                Text = string.Format(St.OperationCompletedSuccessfully,operation.Result.Length)
            },
            [
                (Stc.Ok, () => Task.CompletedTask),
                (St.Details, () => dlg.Message(ToolName, string.Join(Environment.NewLine, operation.Result.Select(GetSnEntryMessage))))
            ]);
        }
    }
}
