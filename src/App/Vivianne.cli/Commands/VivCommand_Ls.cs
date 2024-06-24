using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Commands;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildLsCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("ls", "Enumerates all files on the VIV file.");
        var sizeOption = new Option<bool>(["--size", "-s"], "Includes the file size in the listing.");
        var offsetOption = new Option<bool>(["--offset", "-o"], "Includes the file offset in the listing.");
        var humanOption = new Option<bool>(["--human", "-H"], "File sizes will be formatted in human-readable format.");
        var decOption = new Option<bool>(["--dec", "-d"], "The file offsets will be preented in decimal format.");
        cmd.AddOption(sizeOption);
        cmd.AddOption(offsetOption);
        cmd.AddOption(humanOption);
        cmd.AddOption(decOption);
        cmd.SetHandler(LsCommand, fileArg, sizeOption, offsetOption, humanOption, decOption);
        return cmd;
    }
    
    private static async Task LsCommand(FileInfo vivFile, bool sizeOpt, bool offsetOpt, bool humanOpt, bool decOpt)
    {
        ISerializer<VivFileHeader> parser = new VivHeaderSerializer();
        using var fs = vivFile.OpenRead();
        var viv = await parser.DeserializeAsync(fs);
        int fLen = viv.Entries.Max(p => p.Key.Length);
        foreach (var j in viv.Entries)
        {
            Console.WriteLine(string.Join("\t", ((string?[])[
                j.Key.PadRight(fLen),
                offsetOpt ? (decOpt ? j.Value.Offset.ToString().PadRight(10): $"0x{j.Value.Offset:X8}") : null,
                sizeOpt ? (humanOpt ? ((long)j.Value.Length).ByteUnits() : j.Value.Length.ToString()) : null]).NotEmpty()));
        }
    }
}