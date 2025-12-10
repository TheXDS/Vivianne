using System.CommandLine;
using TheXDS.Vivianne.Info.Map;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers.Audio.Mus;

namespace TheXDS.Vivianne.Commands.Map;

public partial class MapCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", "Gets information on a LIN/MAP file.");
        cmd.AddAlias("nfo");
        cmd.SetHandler(InfoCommand, fileArg);
        return cmd;
    }

    private static Task InfoCommand(FileInfo file) => ReadOnlyFileTransaction<MapFile, MapSerializer>(file, map =>
    {
        foreach (var j in new MapFileInfoExtractor().GetInfo(map))
        {
            Console.WriteLine(j);
        }
    });
}
