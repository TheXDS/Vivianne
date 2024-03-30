using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Tools;

public static class SerialNumberAnalyzer
{
    private class SnEntry
    {
        public readonly Dictionary<string, FeData> FeDatas = [];
        public string FilePath;
        public VivFile VivFile;
        public ushort? SerialNumber;
        public ushort NewSerial;
    }

    private static readonly EnumerationOptions EnumOpts = new() { MatchCasing = MatchCasing.CaseInsensitive };
    private static readonly ISerializer<FeData> serializer = new FeDataSerializer();

    private static SnEntry? Load(FileInfo file)
    {
        try
        {
            var parser = new VivSerializer();
            using var f = file.OpenRead();
            return new SnEntry()
            {
                FilePath = file.FullName,
                
                VivFile = parser.Deserialize(f),
            };
        }
        catch (Exception ex)
        {
            /* Could not load viv. Skip. */
            return null;
        }
    }

    private static IEnumerable<SnEntry> LoadAll(string rootDirectory)
    {
        return Directory
            .GetDirectories(rootDirectory)
            .Select(p => new DirectoryInfo(p).GetFiles("car.viv", EnumOpts).FirstOrDefault())
            .NotNull()
            .Select(Load)
            .NotNull();
    }

    private static FeData? LoadFeData(VivFile viv, string feName)
    {
        try
        {
            if (viv.Directory.TryGetValue(feName, out var feContents))
            {
                return serializer.Deserialize(feContents);
            }
        }
        catch (Exception ex)
        { 
            _ = ex.Message;
        }
        return null;
    }

    private static async IAsyncEnumerable<SnEntry> LoadEntries(string rootDirectory)
    {
        foreach (SnEntry entry in LoadAll(rootDirectory))
        {
            HashSet<ushort> serials = [];
            foreach (var fe in FeData.KnownExtensions)
            {
                var feName = $"fedata{fe}";
                if (await Task.Run(() => LoadFeData(entry.VivFile, feName)) is { } feData)
                {
                    entry.FeDatas.Add(feName, feData);
                    serials.Add(feData.SerialNumber);
                }
            }
            entry.SerialNumber = serials.Count == 1 ? serials.Single() : null;
            yield return entry;
        }
    }

    public static async Task RunTool(IDialogService dlg)
    {
        var r = await dlg.GetDirectoryPath("CarModel path", "Select the 'CarModel' rootDirectory inside NFS3/Gamedata folder.");
        if (r.Success)
        {
            Random rnd = new();
            List<ushort> uniqueSerials = [];
            List<SnEntry> entries = [];
            await foreach (var entry in LoadEntries(r.Result))
            {
                if (entry.SerialNumber is not null && !uniqueSerials.Contains(entry.SerialNumber.Value))
                {
                    uniqueSerials.Add(entry.SerialNumber.Value);
                }
                else
                {
                    entries.Add(entry);
                }
            }
            foreach (var entry in entries)
            {
                ushort newSerial;
                do
                {
                    newSerial = (ushort)rnd.Next(1, ushort.MaxValue);
                } while (uniqueSerials.Contains(newSerial));
                entry.NewSerial = newSerial;
                foreach (var fe in entry.FeDatas)
                {
                    fe.Value.SerialNumber = newSerial;
                    entry.VivFile.Directory[fe.Key] = serializer.Serialize(fe.Value);
                }
                File.Delete(entry.FilePath);
                using var vivF = File.OpenWrite(entry.FilePath);
                var parser = new VivSerializer();
                parser.SerializeTo(entry.VivFile, vivF);
            }
            if (entries.Count == 0)
            {
                await (dlg.Message("Serial number analysis", "No duplicate/conflicting/missing serial numbers found."));
            }
            else if (await dlg.GetOption("Serial number analysis", $"Operation completed successfully. Number of fixed serial numbers: {entries.Count}", "Ok", "Details") == 1)
            {
                await (dlg.Message("Serial number analysis", string.Join(Environment.NewLine, entries.Select(p => $"{p.FilePath}: {p.SerialNumber.ToString() ?? "undefined"} -> {p.NewSerial}"))));
            }
        }
    }
}
