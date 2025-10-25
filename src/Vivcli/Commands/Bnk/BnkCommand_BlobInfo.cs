using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Info.Bnk;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Serializers.Audio.Bnk;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Bnk;

public partial class BnkCommand
{
    private static Command BuildBlobInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("blobinfo", "Gets information on a specific BNK blob");
        var blobArg = new Argument<int?>("blob index",() => null, "Index of the blob to read.");
        var humanOption = new Option<bool>(["--human", "-H"], St.Common_HumanOptionHelp);
        var altStreamOption = new Option<bool>(["--altStreams", "-a"], "Include alternate stream data as well on the output");
        cmd.AddArgument(blobArg);
        cmd.AddOption(humanOption);
        cmd.AddOption(altStreamOption);
        cmd.SetHandler(BlobInfoCommand, fileArg, blobArg, humanOption, altStreamOption);
        return cmd;
    }

    private static Task BlobInfoCommand(FileInfo bnkFile, int? blobArg, bool humanOpt, bool altOpt)
    {
        return ReadOnlyFileTransaction<BnkFile, BnkSerializer>(bnkFile, bnk =>
        {
            if (blobArg is null)
            {
                foreach (var (index, element) in bnk.Streams.WithIndex())
                {
                    Console.WriteLine(string.Format(St.BlobInfo_BlobName, index));
                    PrintBlobInfo(element, humanOpt, altOpt);
                    Console.WriteLine();
                }
            }
            else if (bnk.Streams.ElementAtOrDefault(blobArg.Value) is { } blob)
            {
                PrintBlobInfo(blob, humanOpt, altOpt);
            }
            else
            {
                Fail("No such blob exists inside the BNK file.");
            }
        });
    }

    private static void PrintBlobInfo(BnkStream? blob, bool humanOpt, bool altOpt)
    {
        if (blob is not null)
        {
            foreach (var j in new BnkStreamInfoExtractor(humanOpt).GetInfo(blob))
            {
                Console.WriteLine(j);
            }
            if (altOpt && blob.AltStream is not null)
            {
                PrintBlobInfo(blob.AltStream, humanOpt, false);
            }
        }
        else
        {
            Console.WriteLine("Index points to <NULL>, no data present");
        }
    }
}