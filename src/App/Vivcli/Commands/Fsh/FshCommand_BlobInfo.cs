using System.CommandLine;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildBlobInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("blobinfo", St.BlobInfo_Help);
        var blobArg = new Argument<string?>(St.BlobInfo_Arg1, () => null, St.BlobInfo_Arg1Help);
        var humanOption = new Option<bool>(["--human", "-H"], St.Common_HumanOptionHelp);
        cmd.AddArgument(blobArg);
        cmd.AddOption(humanOption);
        cmd.SetHandler(BlobInfoCommand, fileArg, blobArg, humanOption);
        return cmd;
    }

    private static Task BlobInfoCommand(FileInfo fshFile, string? blobArg, bool humanOpt)
    {
        return FileTransaction(fshFile, fsh =>
        {
            if (blobArg is null)
            {
                foreach (var j in fsh.Entries)
                {
                    Console.WriteLine(string.Format(St.BlobInfo_BlobName, j.Key));
                    PrintBlobInfo(j.Value, humanOpt);
                    Console.WriteLine();
                }
            }
            else if (fsh.Entries.TryGetValue(blobArg, out var blob))
            {
                PrintBlobInfo(blob, humanOpt);
            }
            else
            {
                Fail(St.BlobInfo_Fail);
            }
        }, true);
    }

    private static void PrintBlobInfo(FshBlob blob, bool humanOpt)
    {
        Console.WriteLine(string.Format(St.BlobInfo_PrintBlobInfo1, blob.Width, blob.Height));
        Console.WriteLine(string.Format(St.BlobInfo_PrintBlobInfo2, Mappings.GetFshBlobLabel(blob.Magic)));
        Console.WriteLine(string.Format(St.BlobInfo_PrintBlobInfo3, blob.XRotation, blob.YRotation));
        Console.WriteLine(string.Format(St.BlobInfo_PrintBlobInfo4, blob.XPosition, blob.YPosition));
        Console.WriteLine(string.Format(St.BlobInfo_PrintBlobInfo5, blob.PixelData.Length.GetSize(humanOpt)));
        Console.WriteLine(string.Format(St.BlobInfo_PrintBlobInfo6, Mappings.GetFshBlobFooterLabel(blob, humanOpt)));
    }
}