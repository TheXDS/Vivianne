using System.CommandLine;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio;
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
        var chunksOpt = new Option<int?>(["--chunks", "-C"], () => null, "Specifies the number of chunks to subdivide the ASF stream into.");
        asfCmd.AddArgument(wavArg);
        asfCmd.AddOption(codecOpt);
        asfCmd.AddOption(chunksOpt);
        asfCmd.SetHandler(AsfCommand, fileArg, wavArg, codecOpt, chunksOpt);
        return asfCmd;
    }

    private static async Task AsfCommand(FileInfo outputFile, FileInfo inputFile, CompressionMethod codec, int? chunks)
    {
        if (!Mappings.AudioCodecSelector.TryGetValue(codec, out var selector)) throw new Exception("");
        using var inputStream = inputFile.OpenRead();
        var asf = AudioRender.AsfFromWav(inputStream);
        AudioRender.ReSliceAsf(asf, chunks ?? InferChunkCount(asf));
        if (codec != CompressionMethod.None)
        {
            var c = selector.Invoke();
            for (var j = 0; j < asf.AudioBlocks.Count; j++)
            {
                asf.AudioBlocks[j] = c.Encode(asf.AudioBlocks[j], new PtHeader());
            }
            asf.Compression = codec;
        }
        ISerializer<AsfFile> serializer = new MusSerializer();
        using var outputStream = outputFile.OpenWrite();
        await serializer.SerializeToAsync(asf, outputStream);
    }

    private static int InferChunkCount(AsfFile asf)
    {
        return asf.AudioBlocks[0].Length / (3000 * asf.Channels);
    }
}
