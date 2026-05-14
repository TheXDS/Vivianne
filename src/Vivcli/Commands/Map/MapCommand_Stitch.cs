using System.CommandLine;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio.Mus;

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
        (var indices, var loopStart) = MapStitcher.Stitch(map);
        foreach (var item in indices)
        {
            Console.Write($"{item}, ");
        }
        Console.WriteLine($"🔁 Loop to {loopStart}");
    });
}
