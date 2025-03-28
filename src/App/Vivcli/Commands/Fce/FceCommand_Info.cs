using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheXDS.Vivianne.Info.Bnk;
using TheXDS.Vivianne.Info.Fce;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fce;

public partial class FceCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", "Gets information on the BNK file");
        var humanOption = new Option<bool>(["--human", "-H"], St.Common_HumanOptionHelp);
        var rsvdContentOption = new Option<bool>(["--dump", "-d"], "Dumps the contents of the data tables inside the FCE file.");
        cmd.AddOption(humanOption);
        cmd.AddOption(rsvdContentOption);
        cmd.SetHandler(InfoCommand, fileArg, humanOption, rsvdContentOption);
        return cmd;
    }
    private static Task InfoCommand(FileInfo bnkFile, bool humanOpt, bool dump)
    {
        return FileTransaction(bnkFile, bnk =>
        {
            foreach (var j in new Fce3InfoExtractor(humanOpt, dump).GetInfo(bnk))
            {
                Console.WriteLine(j);
            }
        }, true);
    }
}
