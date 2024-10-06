using System.CommandLine;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", "Displays general information about the FSH/QFS file.");
        var humanOption = new Option<bool>(["--human", "-H"], "Blob sizes will be formatted in human-readable format.");
        cmd.AddOption(humanOption);
        cmd.SetHandler(InfoCommand, fileArg, humanOption);
        return cmd;
    }
    
    private static async Task InfoCommand(FileInfo fshFile, bool humanOpt)
    {
        ISerializer<FshFile> parser = new FshSerializer();
        var rawData = File.ReadAllBytes(fshFile.FullName);
        var uncompressed = QfsCodec.Decompress(rawData);
        var fsh = await parser.DeserializeAsync(uncompressed);
        fsh.IsCompressed = QfsCodec.IsCompressed(rawData);
        Console.WriteLine($"Blobs in FSH: {fsh.Entries.Count}");
        Console.WriteLine($"Directory ID: {fsh.DirectoryId}");
        Console.WriteLine($"File size: {GetSize(rawData.Length, humanOpt)}");
        if (fsh.IsCompressed)
        {
            Console.WriteLine($"Is FSH file compressed: yes");
            Console.WriteLine($"Uncompressed payload size: {GetSize(uncompressed.Length, humanOpt)}");
            Console.WriteLine($"Compression ratio: {(double)uncompressed.Length / rawData.Length:f2}:1");
        }
        else
        {
            Console.WriteLine($"Is FSH file compressed: no");
        }
    }
}