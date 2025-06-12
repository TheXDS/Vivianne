using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.Commands.Mus;

/// <summary>
/// Defines a command that allows the user to interact with a BNK file.
/// </summary>
public partial class MusCommand
{
    private static Command BuildExtractCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("extract", "Extracts all blobs in a MUS file into .WAV files.");
        var offsetNaming = new Option<bool>(["--offset-naming", "-n"], "Use offset-based naming for output files. Defaults to index-based naming.");
        var outFile = new Option<DirectoryInfo>(["--out", "-o"], "Specifies the path to write the new .WAV files to.").LegalFilePathsOnly().ExistingOnly();
        cmd.AddAlias("xf");
        cmd.AddOption(offsetNaming);
        cmd.AddOption(outFile);
        cmd.SetHandler(ExtractCommand, fileArg, offsetNaming, outFile);
        return cmd;
    }

    private static Task ExtractCommand(FileInfo musFile, bool offsetNaming, DirectoryInfo outDir)
    {
        return ReadOnlyFileTransaction<MusFile, MusSerializer>(musFile, async mus =>
        {
            foreach(var (index, asf) in mus.AsfSubStreams.WithIndex())
            {
                using var output = new FileStream(
                    Path.Combine(outDir.FullName, offsetNaming ? $"0x{asf.Key:X8}.wav" : $"{index:D3}.wav"),
                    FileMode.Create, FileAccess.Write, FileShare.None);
                await output.WriteAsync(AudioRender.RenderData(asf.Value, [.. asf.Value.AudioBlocks.SelectMany(p => p)]));
                await output.FlushAsync();
            }
        });
    }
}
