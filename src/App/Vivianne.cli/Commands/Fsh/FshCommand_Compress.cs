using System.CommandLine;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildCompressCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("compress", "Converts a FSH file into a QFS file.");
        var outputOption = new Option<FileInfo?>(["--output", "-o"], () => null, "Specifies a file to write the compressed FSH into. If omitted, the compressed data will be written to stdout.");
        cmd.AddOption(outputOption);
        cmd.SetHandler(CompressCommand, fileArg, outputOption);
        return cmd;
    }
    
    private static async Task CompressCommand(FileInfo fshFile, FileInfo? outOpt)
    {
        ISerializer<FshFile> parser = new FshSerializer();
        var rawContents = await File.ReadAllBytesAsync(fshFile.FullName);
        try
        {
            _ = await parser.DeserializeAsync(rawContents);
        }
        catch (Exception ex)
        {
            Fail($"Could not load the specified file as a FSH file: {ex.Message}");
            return;
        }
        using var outputStream = outOpt?.OpenWrite() ?? Console.OpenStandardOutput();
        await outputStream.WriteAsync(QfsCodec.Compress(rawContents));
    }
}