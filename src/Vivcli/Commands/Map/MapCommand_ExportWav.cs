using System.CommandLine;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.Commands.Map;

public partial class MapCommand
{
    private static Command BuildExportWavCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("exportwav", "Exports the entire mapped audio sequence from a .LIN/.MAP file and its corresponding .MUS file into a single .WAV file.");
        var musArg = new Argument<FileInfo?>(".mus file", () => null, "MUS file containing the audio streams. Defaults to a file with the same name in the same directory.");
        var outFile = new Option<FileInfo>(["--out", "-o"], "Specifies the path to write the new .WAV file to.").LegalFilePathsOnly();
        
        cmd.AddArgument(musArg);
        cmd.AddOption(outFile);
        cmd.SetHandler(ExportWavCommand, fileArg, musArg, outFile);
        return cmd;
    }

    private static Task ExportWavCommand(FileInfo mapFile, FileInfo? musFile, FileInfo? outFile)
    {
        return ReadOnlyFileTransaction<MapFile, MapSerializer>(mapFile, async map =>
        {
            var musFilePath = musFile?.FullName ?? Path.Combine(mapFile.DirectoryName!, $"{Path.GetFileNameWithoutExtension(mapFile.Name)}.mus");
            var musFileInfo = new FileInfo(musFilePath);
            
            if (!musFileInfo.Exists)
            {
                Fail($"MUS file not found: {musFilePath}");
                return;
            }

            using var musStream = musFileInfo.OpenRead();
            var mus = await ((ISerializer<MusFile>)new MusSerializer()).DeserializeAsync(musStream);
            
            var (header, data) = AudioRender.JoinAllStreams(mus, map);
            var wavBytes = AudioRender.RenderData(header, data);
            
            var outputPath = outFile?.FullName ?? $"{Path.GetFileNameWithoutExtension(mapFile.FullName)}.wav";
            await File.WriteAllBytesAsync(outputPath, wavBytes);
            Console.WriteLine($"Exported to: {outputPath}");
        });
    }
}
