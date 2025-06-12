using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Viv;
using St = TheXDS.Vivianne.Resources.Strings.VivCommand;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildLsCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("ls", St.Ls_Help);
        var sizeOption = new Option<bool>(["--size", "-s"], St.Ls_SizeHelp);
        var offsetOption = new Option<bool>(["--offset", "-o"], St.Ls_OffsetHelp);
        var humanOption = new Option<bool>(["--human", "-H"], St.Ls_HumanHelp);
        var decOption = new Option<bool>(["--dec", "-d"], St.Ls_DecHelp);
        cmd.AddOption(sizeOption);
        cmd.AddOption(offsetOption);
        cmd.AddOption(humanOption);
        cmd.AddOption(decOption);
        cmd.SetHandler(LsCommand, fileArg, sizeOption, offsetOption, humanOption, decOption);
        return cmd;
    }
    
    private static Task LsCommand(FileInfo vivFile, bool sizeOpt, bool offsetOpt, bool humanOpt, bool decOpt)
    {
        return ReadOnlyFileTransaction<VivFileHeader, VivHeaderSerializer>(vivFile, viv =>
        {
            int fLen = viv.Entries.Max(p => p.Key.Length);
            foreach (var j in viv.Entries)
            {
                Console.WriteLine(string.Join("\t", ((string?[])[
                    j.Key.PadRight(fLen),
                    offsetOpt ? (decOpt ? j.Value.Offset.ToString().PadRight(10): $"0x{j.Value.Offset:X8}") : null,
                    sizeOpt ? (humanOpt ? ((long)j.Value.Length).ByteUnits() : j.Value.Length.ToString()) : null]).NotEmpty()));
            }
        });
    }
}