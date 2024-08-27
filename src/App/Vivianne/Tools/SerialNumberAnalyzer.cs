using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Services;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

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
        public string FilePath = null!;
        public VivFile VivFile = null!;
        public ushort? SerialNumber;
        public ushort NewSerial;
    }

    private static readonly EnumerationOptions EnumOpts = new() { MatchCasing = MatchCasing.CaseInsensitive };
    private static readonly ISerializer<FeData> feSerializer = new FeDataSerializer();
    private static readonly ISerializer<VivFile> vivSerializer = new VivSerializer();

    private static async Task<SnEntry?> Load(FileInfo file)
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
            Debug.Print($"Error loading VIV file '{file.Name}': {ex.Message}. Skipping...");
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
            Debug.Print($"Error loading FeData '{feName}': {ex.Message}. Skipping...");
            _ = ex.Message;
        }
        return null;
    }

    private static async IAsyncEnumerable<SnEntry> LoadEntries(FileInfo[] files, IProgress<ProgressReport> progress, [EnumeratorCancellation] CancellationToken cancel)
    {
        var c = 0;
        foreach (var file in files)
        {
            progress.Report(new(c++ * 100 / files.Length, $"Loading '{file.FullName}'..."));
            if (await Load(file) is { } entry)
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
                entry.SerialNumber = serials.Count == 1 ? serials.Single() : null;
                if (cancel.IsCancellationRequested) yield break;
                yield return entry;
            };
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
            progress.Report(new(c++ * 100 / entries.Length, $"Writing '{entry.FilePath}'..."));
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
    public string ToolName => "Serial number analyzer";

    /// <inheritdoc/>
    public async Task Run(IDialogService dlg, INavigationService _)
    {
        var dirSelection = await dlg.GetDirectoryPath("CarModel path", "Select the 'CarModel' directory inside NFS3/Gamedata folder.");
        if (!dirSelection.Success) return;

        var operation = await dlg.RunOperation(ToolName, (c, p) => RunSnAnalysis(p, dirSelection.Result, c));
        if (!operation.Success) return;
        if (operation.Result.Length == 0)
        {
            await dlg.Message(ToolName, "No duplicate/conflicting/missing serial numbers found.");
        }
        else if (await dlg.GetOption(ToolName, $"Operation completed successfully. Number of fixed serial numbers: {operation.Result.Length}", "Ok", "Details") == 1)
        {
            await dlg.Message(ToolName, string.Join(Environment.NewLine, operation.Result.Select(p => $"{p.FilePath}: {p.SerialNumber.ToString() ?? "undefined"} -> {p.NewSerial}")));
        }
    }
}
