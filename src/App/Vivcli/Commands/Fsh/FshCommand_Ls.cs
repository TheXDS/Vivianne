using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildLsCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("ls", St.Ls_Help);
        var sizeOption = new Option<bool>(["--size", "-s"], St.Ls_SizeOptionHelp);
        var offsetOption = new Option<bool>(["--offset", "-o"], St.Ls_OffsetOptionHelp);
        var humanOption = new Option<bool>(["--human", "-H"], St.Common_HumanOptionHelp);
        var decOption = new Option<bool>(["--dec", "-d"], St.Ls_DecimalOptionHelp);
        cmd.AddOption(sizeOption);
        cmd.AddOption(offsetOption);
        cmd.AddOption(humanOption);
        cmd.AddOption(decOption);
        cmd.SetHandler(LsCommand, fileArg, sizeOption, offsetOption, humanOption, decOption);
        return cmd;
    }

    private static async Task LsCommand(FileInfo fshFile, bool sizeOpt, bool offsetOpt, bool humanOpt, bool decOpt)
    {
        static int EstimateSize(FshBlob blob) => 13 + blob.PixelData.Length + blob.Footer.Length;

        ISerializer<FshFile> parser = new FshSerializer();
        var fsh = await parser.DeserializeAsync(QfsCodec.Decompress(File.ReadAllBytes(fshFile.FullName)));
        foreach (var j in fsh.Entries)
        {
            Console.WriteLine(string.Join("\t", ((string?[])[
                j.Key.PadRight(4),
                // offsetOpt ? (decOpt ? j.Value.Offset.ToString().PadRight(10): $"0x{j.Value.Offset:X8}") : null,
                 sizeOpt ? EstimateSize(j.Value).GetSize(humanOpt) : null
            ]).NotEmpty()));
        }
    }
}