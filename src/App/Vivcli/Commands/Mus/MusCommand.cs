using BCnEncoder.Shared.ImageFiles;
using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Info.Bnk;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers.Audio.Bnk;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.Commands.Mus;

/// <summary>
/// Defines a command that allows the user to interact with a BNK file.
/// </summary>
public partial class MusCommand() : FileCommandBase(
    "mus",
    "Performs operations on MUS files.",
    "MUS file",
    "Path to the MUS file.",
    [
        BuildInfoCommand,
        BuildExtractCommand,
        //BuildReplaceCommand,
    ])
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
                foreach (var (index, element) in mus.AsfElement.WithIndex())
                {
                    Console.WriteLine(string.Format("Index: {0}", index));
                    PrintBlobInfo(element, humanOpt);
                    Console.WriteLine();
                }
            }
            else if (mus.AsfElement.ElementAtOrDefault(blobArg.Value) is { } blob)
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

    private static Command BuildExtractCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("xf", "Extracts a MUS blob into a .WAV file.");
        var outFile = new Option<FileInfo>(["--out", "-o"], "Specifies the path th write the new .WAV file to.").LegalFilePathsOnly();
        cmd.AddOption(outFile);
        cmd.SetHandler(ExtractCommand, fileArg, outFile);
        return cmd;
    }

    private static Task ExtractCommand(FileInfo musFile, FileInfo outFile)
    {
        return ReadOnlyFileTransaction<MusFile, MusSerializer>(musFile, async mus =>
        {
            var rawSteram = new List<byte>();
            AsfFile? j = mus.AsfElement.FirstOrDefault();
            foreach (var k in mus.AsfElement)
            {
                rawSteram.AddRange(k.AudioBlocks.SelectMany(p => p).ToArray());
                
            }
            using var output = outFile.OpenWrite();
            await output.WriteAsync(AudioRender.RenderData(j, rawSteram.ToArray()));
            await output.FlushAsync();
        });
    }

}