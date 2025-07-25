﻿using System.CommandLine;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Serializers.Audio.Bnk;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.Commands.Bnk;

public partial class BnkCommand
{
    private static Command BuildExtractCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("xf", "Extracts a BNK blob into a .WAV file.");
        var blobArg = new Argument<int>("blob index", "Index of the blob to read.");
        var outFile = new Option<FileInfo>(["--out", "-o"], "Specifies the path th write the new .WAV file to.").LegalFilePathsOnly();
        cmd.AddArgument(blobArg);
        cmd.AddOption(outFile);
        cmd.SetHandler(ExtractCommand, fileArg, blobArg, outFile);
        return cmd;
    }

    private static Task ExtractCommand(FileInfo bnkFile, int blobArg, FileInfo outFile)
    {
        return ReadOnlyFileTransaction<BnkFile, BnkSerializer>(bnkFile, async bnk => {
            if (bnk.Streams[blobArg] is not { } bnkStream) return;
            using var output = outFile.OpenWrite();
            await output.WriteAsync(AudioRender.RenderBnk(bnkStream));
            await output.FlushAsync();
        });
    }
}
