using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Bnk;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Bnk;

public partial class BnkCommand
{
    private static Command BuildBlobInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("blobinfo", "Gets information on a specific BNK blob");
        var blobArg = new Argument<int?>("blob index",() => null, "Index of the blob to read.");
        var humanOption = new Option<bool>(["--human", "-H"], St.Common_HumanOptionHelp);
        cmd.AddArgument(blobArg);
        cmd.AddOption(humanOption);
        cmd.SetHandler(BlobInfoCommand, fileArg, blobArg, humanOption);
        return cmd;
    }

    private static Task BlobInfoCommand(FileInfo bnkFile, int? blobArg, bool humanOpt)
    {
        return FileTransaction(bnkFile, bnk =>
        {
            if (blobArg is null)
            {
                foreach (var j in bnk.Streams.WithIndex())
                {
                    Console.WriteLine(string.Format(St.BlobInfo_BlobName, j.index));
                    PrintBlobInfo(j.element, humanOpt);
                    Console.WriteLine();
                }
            }
            else if (bnk.Streams.ElementAtOrDefault(blobArg.Value) is { } blob)
            {
                PrintBlobInfo(blob, humanOpt);
            }
            else
            {
                Fail("No such blob exists inside the BNK file.");
            }
        }, true);
    }

    private static void PrintBlobInfo(BnkBlob blob, bool humanOpt)
    {
        Console.WriteLine(string.Format("Samplig rate: {0} Hz", blob.SampleRate));
        Console.WriteLine(string.Format("Channels: {0}", blob.Channels));
        Console.WriteLine(string.Format("Raw sample size: {0}", blob.SampleData.Length.GetSize(humanOpt)));
    }
}