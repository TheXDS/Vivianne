using System.CommandLine;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildBlobInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("blobinfo", "Displays general information about the specified FSH blob.");
        var blobArg = new Argument<string?>("blob name", () => null, "Specifies the name of the blob to get the information about. If omitted, ths tool will print the information of all blobs in the FSH file.");
        var humanOption = new Option<bool>(["--human", "-H"], "Blob sizes will be formatted in human-readable format.");
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
                    Console.WriteLine($"Blob name: {j.Key}");
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
                Fail("No such blob entry has been found in the specified FSH file.");
            }
        }, true);
    }

    private static void PrintBlobInfo(FshBlob blob, bool humanOpt)
    {
        Console.WriteLine($"Blob resolution: {blob.Width}x{blob.Height}");
        Console.WriteLine($"Pixel format: {(Mappings.FshBlobToLabel.TryGetValue(blob.Magic, out var label) ? label : $"Unknown (0x{(byte)blob.Magic:X}")}");
        Console.WriteLine($"Rotation axis: X:{blob.XRotation}, Y:{blob.YRotation}");
        Console.WriteLine($"Image offset: X:{blob.XPosition}, Y:{blob.YPosition}");
        Console.WriteLine($"Pixel data size: {GetSize(blob.PixelData.Length, humanOpt)}");
        Console.WriteLine($"Footer data: {InferFshBlobFooter(blob, humanOpt)}");
    }

    private static string InferFshBlobFooter(FshBlob blob, bool humanOpt)
    {
        foreach (var j in Mappings.FshBlobFooterIdentifier)
        {
            if (j.Value.Invoke(blob.Footer!))
            {
                return Mappings.FshBlobFooterToLabel.TryGetValue(j.Key, out var label)
                    ? label
                    : $"Other ({j.Key})";
            }
        }
        return $"Unknown ({GetSize(blob.Footer?.Length ?? 0, humanOpt)})";
    }
}