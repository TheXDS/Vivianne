using System.CommandLine;
using TheXDS.Vivianne.Codecs;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fsh;
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
        var rawContents = await File.ReadAllBytesAsync(fshFile.FullName);
        try
        {
            _ = await ((ISerializer<FshFile>)new FshSerializer()).DeserializeAsync(rawContents);
        }
        catch (Exception ex)
        {
            Fail(string.Format(St.Compress_Fail, ex.Message));
            return;
        }
        using var outputStream = outOpt?.OpenWrite() ?? Console.OpenStandardOutput();
        await outputStream.WriteAsync(LzCodec.Compress(rawContents));
    }
}