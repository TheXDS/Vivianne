using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Info.Bnk;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers.Audio.Mus;

namespace TheXDS.Vivianne.Commands.Asf;

/// <summary>
/// Defines a command that allows the user to interact with a BNK file.
/// </summary>
public partial class AsfCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", "Gets information on a MUS file.");
        var blobArg = new Argument<int?>("blob index", () => null, "Index of the blob to read.");
        var humanOption = new Option<bool>(["--human", "-H"], "St.Common_HumanOptionHelp");
        cmd.AddArgument(blobArg);
        cmd.AddOption(humanOption);
        cmd.SetHandler(BlobInfoCommand, fileArg, blobArg, humanOption);
        return cmd;
    }

    private static Task BlobInfoCommand(FileInfo musFile, int? blobArg, bool humanOpt)
    {
        return ReadOnlyFileTransaction<MusFile, MusSerializer>(musFile, mus =>
        {
            if (blobArg is null)
            {
                foreach (var (index, element) in mus.AsfSubStreams.WithIndex())
                {
                    Console.WriteLine(string.Format("Index: {0} (offset 0x{1:X8})", index, element.Key));
                    PrintBlobInfo(element.Value, humanOpt);
                    Console.WriteLine();
                }
            }
            else if (mus.AsfSubStreams.Values.ElementAtOrDefault(blobArg.Value) is { } blob)
            {
                PrintBlobInfo(blob, humanOpt);
            }
            else
            {
                Fail("No such ASF blob exists inside the MUS file.");
            }
        });
    }

    private static void PrintBlobInfo(AsfFile? blob, bool humanOpt)
    {
        if (blob is not null)
        {
            foreach (var j in new AsfFileInfoExtractor(humanOpt).GetInfo(blob))
            {
                Console.WriteLine(j);
            }
        }
        else
        {
            Console.WriteLine("Index points to <NULL>, no data present");
        }
    }
}
