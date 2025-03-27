using System.CommandLine;
using TheXDS.Vivianne.Info.Bnk;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Bnk;

public partial class BnkCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", "Gets information on the BNK file");
        var humanOption = new Option<bool>(["--human", "-H"], St.Common_HumanOptionHelp);
        cmd.AddOption(humanOption);
        cmd.SetHandler(InfoCommand, fileArg, humanOption);
        return cmd;
    }

    private static Task InfoCommand(FileInfo bnkFile, bool humanOpt)
    {
        return FileTransaction(bnkFile, bnk =>
        {
            foreach (var j in new BnkFileInfoExtractor().GetInfo(bnk))
            {
                Console.WriteLine(j);
            }
        }, true);
    }
}