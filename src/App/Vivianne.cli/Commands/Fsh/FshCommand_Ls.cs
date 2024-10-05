using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildLsCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("ls", "Enumerates all blobs on the FSH/QFS file.");
        var sizeOption = new Option<bool>(["--size", "-s"], "Includes the blob size in the listing.");
        var offsetOption = new Option<bool>(["--offset", "-o"], "Includes the blob offset in the listing.");
        var humanOption = new Option<bool>(["--human", "-H"], "Blob sizes will be formatted in human-readable format.");
        var decOption = new Option<bool>(["--dec", "-d"], "The blob offsets will be presented in decimal format.");
        cmd.AddOption(sizeOption);
        cmd.AddOption(offsetOption);
        cmd.AddOption(humanOption);
        cmd.AddOption(decOption);
        cmd.SetHandler(LsCommand, fileArg, sizeOption, offsetOption, humanOption, decOption);
        return cmd;
    }
    
    private static async Task LsCommand(FileInfo fshFile, bool sizeOpt, bool offsetOpt, bool humanOpt, bool decOpt)
    {
        ISerializer<FshFile> parser = new FshSerializer();
        var fsh = await parser.DeserializeAsync(QfsCodec.Decompress(File.ReadAllBytes(fshFile.FullName)));
        foreach (var j in fsh.Entries)
        {
            Console.WriteLine(string.Join("\t", ((string?[])[
                j.Key.PadRight(4),
                // offsetOpt ? (decOpt ? j.Value.Offset.ToString().PadRight(10): $"0x{j.Value.Offset:X8}") : null,
                 sizeOpt ? GetSize(EstimateSize(j.Value), humanOpt) : null
            ]).NotEmpty()));
        }
    }
}