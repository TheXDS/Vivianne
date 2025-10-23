using System.CommandLine;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio.Mus;

namespace TheXDS.Vivianne.Commands.Map;

file record struct MapMusCheck(string CheckName, Func<MapFile, MusFile, bool> CheckFails, Func<MapFile, MusFile, string> FailureMessage, bool ShowStopper = false)
{
    public static readonly MapMusCheck[] Checks =
    [
        new("Section/stream count", CheckStreamCount, CheckStreamCountFail, true),
        new("Section/stream offsets", CheckOffsets,CheckOffsetsFail)
    ];

    private static bool CheckStreamCount(MapFile map, MusFile mus)
    {
        return map.Items.Count != mus.AsfSubStreams.Count;
    }

    private static string CheckStreamCountFail(MapFile map, MusFile mus)
    {
        return $"MAP<->MUS sections/streams mismatch ({map.Items.Count} vs. {mus.AsfSubStreams.Count})";
    }

    private static bool CheckOffsets(MapFile map, MusFile mus)
    {
        return map.Items.Select(p => p.MusOffset).Distinct().Except(mus.AsfSubStreams.Keys).Any();
    }

    private static string CheckOffsetsFail(MapFile map, MusFile mus)
    {
        return $"{map.Items.Select(p => p.MusOffset).Distinct().Except(mus.AsfSubStreams.Keys).Count()} sections with mismatched streams.";
    }
}

public partial class MapCommand
{
    private static Command BuildCheckCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("check", "Checks that the LIN/MAP file corresponds to the specified .MUS file.");
        var musArg = new Argument<FileInfo>(".mus file", "MUS file to check against.");
        cmd.AddArgument(musArg);
        cmd.AddAlias("chk");
        cmd.SetHandler(CheckCommand, fileArg, musArg);
        return cmd;
    }

    private static Task CheckCommand(FileInfo mapFile, FileInfo musFile) => ReadOnlyFileTransaction<MapFile, MapSerializer>(mapFile, async map =>
    {
        MusFile mus;
        using (var musStream = musFile.OpenRead())
        {
            mus = await ((ISerializer<MusFile>)new MusSerializer()).DeserializeAsync(musStream);
        }
        foreach (var j in MapMusCheck.Checks)
        {
            Console.Write($"{j.CheckName}...");
            var result = j.CheckFails.Invoke(map, mus);
            Console.WriteLine(result ? $"❌ {j.FailureMessage.Invoke(map, mus)}" : "✔️ OK");
            if (result && j.ShowStopper) break;
        }
    });
}
