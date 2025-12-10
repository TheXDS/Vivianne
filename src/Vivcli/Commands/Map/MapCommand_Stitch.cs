using System.CommandLine;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers.Audio.Mus;

namespace TheXDS.Vivianne.Commands.Map;

public partial class MapCommand
{
    private static Command BuildStitchCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("stitch", "Stitches together the playback sequence from a LIN file.");
        cmd.SetHandler(StitchCommand, fileArg);
        return cmd;
    }

    private static Task StitchCommand(FileInfo fileArg) => ReadOnlyFileTransaction<MapFile, MapSerializer>(fileArg, map =>
    {
        List<int> sequence = [];
        int current = map.FirstItem;
        while (!sequence.Contains(current))
        {
            sequence.Add(current);
            current = map.Items[current].Jumps.FirstOrDefault()?.NextItem ?? 0;
        }

        foreach (var item in sequence)
        { 
            Console.Write($"{item}, ");
        }
        Console.WriteLine($"🔁 Loop to {current}");
    });
}
