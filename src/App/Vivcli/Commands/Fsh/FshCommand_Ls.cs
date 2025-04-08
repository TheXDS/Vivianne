using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.Tools.Fsh;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildLsCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("ls", St.Ls_Help);
        var sizeOption = new Option<bool>(["--size", "-s"], St.Ls_SizeOptionHelp);
        var humanOption = new Option<bool>(["--human", "-H"], St.Common_HumanOptionHelp);
        cmd.AddOption(sizeOption);
        cmd.AddOption(humanOption);
        cmd.SetHandler(LsCommand, fileArg, sizeOption, humanOption);
        return cmd;
    }

    private static Task LsCommand(FileInfo fshFile, bool sizeOpt, bool humanOpt)
    {
        static int EstimateSize(FshBlob blob) => 13 + blob.PixelData.Length + blob.Footer.Length;

        return ReadOnlyFileTransaction<FshFile, FshSerializer>(fshFile, fsh =>
        {
            foreach (var j in fsh.Entries)
            {
                Console.WriteLine(string.Join("\t", ((string?[])[
                    j.Key.PadRight(4),
                    sizeOpt ? EstimateSize(j.Value).GetSize(humanOpt) : null
                ]).NotEmpty()));
            }
        });
    }
}