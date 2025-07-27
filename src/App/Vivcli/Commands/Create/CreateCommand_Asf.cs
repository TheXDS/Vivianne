using System.CommandLine;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.Commands.Create;

public partial class CreateCommand
{
    private static Command CreateAsfCommand(Argument<FileInfo> fileArg)
    {
        var asfCmd = new Command("asf", "Creates a new .ASF file.");
        asfCmd.AddArgument(fileArg);
        var wavArg = new Argument<FileInfo>("wav file", ".WAV file to create the .ASF from.").ExistingOnly();
        var codecOpt = new Option<CompressionMethod>(["--codec", "-c"], () => CompressionMethod.None, "Specifies the compression codec to use.");
        asfCmd.AddArgument(wavArg);
        asfCmd.AddOption(codecOpt);
        asfCmd.SetHandler(AsfCommand, fileArg, wavArg, codecOpt);
        return asfCmd;
    }

    private static async Task AsfCommand(FileInfo outputFile, FileInfo inputFile, CompressionMethod codec)
    {
        using var inputStream = inputFile.OpenRead();
        var asf = AudioRender.AsfFromWav(inputStream);
        ISerializer<AsfFile> serializer = new MusSerializer();
        using var outputStream = outputFile.OpenWrite();
        await serializer.SerializeToAsync(asf, outputStream);
    }
}
