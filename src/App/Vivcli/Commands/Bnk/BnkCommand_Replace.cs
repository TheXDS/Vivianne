using System.CommandLine;
using TheXDS.Vivianne.Info.Bnk;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Serializers.Audio.Bnk;
using TheXDS.Vivianne.Tools.Audio;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Bnk;

public partial class BnkCommand
{
    private static Command BuildReplaceCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("replace", "Replaces a blob in the BNK file with a new one.");
        var blobArg = new Argument<int>("blob index", "Index of the blob to replace.");
        var inFile = new Option<FileInfo>(["--in", "-i"], "Specifies the path to the .WAV file to read from.").LegalFilePathsOnly();
        cmd.AddArgument(blobArg);
        cmd.AddOption(inFile);
        cmd.SetHandler(ReplaceCommand, fileArg, blobArg, inFile);
        return cmd;
    }

    private static Task ReplaceCommand(FileInfo bnkFile, int blobArg, FileInfo inFile)
    {
        return FileTransaction<BnkFile, BnkSerializer>(bnkFile, async bnk =>
        {
            bnk.Streams[blobArg] = AudioRender.BnkFromWav(await File.ReadAllBytesAsync(inFile.FullName));;
        });
    }
}