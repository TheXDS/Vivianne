using System.CommandLine;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.Commands.Mus;

/// <summary>
/// Defines a command that allows the user to interact with a BNK file.
/// </summary>
public partial class MusCommand
{
    private static Command BuildExtractBlobCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("extract-blob", "Extracts a MUS blob into a .WAV file.");
        var blobArg = new Argument<string?>("blob index/offset", () => null, "Index or offset of the blob to read. An index can be specified as a simple integer value. An offset is prefixed with '0x'. If not specified the entire MUS file will be exported as a single .WAV file.");
        var outFile = new Option<FileInfo>(["--out", "-o"], "Specifies the path th write the new .WAV file to.").LegalFilePathsOnly();
        cmd.AddArgument(blobArg);
        cmd.AddOption(outFile);
        cmd.AddAlias("xfb");
        cmd.SetHandler(ExtractBlobCommand, fileArg, blobArg, outFile);
        return cmd;
    }

    private static Task ExtractBlobCommand(FileInfo musFile, string? blobArg, FileInfo outFile)
    {
        return ReadOnlyFileTransaction<MusFile, MusSerializer>(musFile, async mus =>
        {
            (AudioStreamBase audioHeader, byte[] rawStream) = blobArg is not null
                ? InferBlob(blobArg, mus)
                : JoinAllStreams(mus);
            using var output = outFile.OpenWrite();
            await output.WriteAsync(AudioRender.RenderData(audioHeader, rawStream));
            await output.FlushAsync();
        });
    }

    private static (AudioStreamBase, byte[]) InferBlob(string unparsedValue, MusFile mus)
    {
        var isOffset = unparsedValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        var value = Convert.ToInt32(unparsedValue, isOffset ? 16 : 10);
        return isOffset ? GetByOffset(mus, value) : GetByIndex(mus, value);
    }

    private static (AudioStreamBase, byte[]) GetByIndex(MusFile mus, int index)
    {
        if (mus.AsfSubStreams.Values.ElementAtOrDefault(index) is not { } blob)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "No such ASF blob exists inside the MUS file.");
        }
        return (blob, [.. blob.AudioBlocks.SelectMany(p => p)]);
    }

    private static (AudioStreamBase, byte[]) GetByOffset(MusFile mus, int offset)
    {
        if (!mus.AsfSubStreams.TryGetValue(offset, out var blob))
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "No such ASF blob exists inside the MUS file.");
        }
        return (blob, [.. blob.AudioBlocks.SelectMany(p => p)]);
    }

    private static (AudioStreamBase, byte[]) JoinAllStreams(MusFile mus)
    {
        var rawSteram = new List<byte>();
        AsfFile commonHeader = mus.AsfSubStreams.Values.First();
        foreach (var k in mus.AsfSubStreams.Values)
        {
            rawSteram.AddRange([.. k.AudioBlocks.SelectMany(p => p)]);
        }
        return (commonHeader, [.. rawSteram]);
    }
}
