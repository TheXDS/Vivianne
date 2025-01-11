using System.CommandLine;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildCompressCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("compress", St.Compress_Help);
        var outputOption = new Option<FileInfo?>(["--output", "-o"], () => null, St.Compress_OutputOptionHelp);
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
            Fail(string.Format(St.Compress_Fail, ex.Message));
            return;
        }
        using var outputStream = outOpt?.OpenWrite() ?? Console.OpenStandardOutput();
        await outputStream.WriteAsync(QfsCodec.Compress(rawContents));
    }
}