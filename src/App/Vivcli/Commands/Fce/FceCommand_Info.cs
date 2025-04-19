using System.CommandLine;
using TheXDS.Vivianne.Info.Fce;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Serializers.Fce.Common;
using St = TheXDS.Vivianne.Resources.Strings.FceCommand;
using Stc = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fce;

public partial class FceCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", St.Info_help);
        var humanOption = new Option<bool>(["--human", "-H"], Stc.Common_HumanOptionHelp);
        var rsvdContentOption = new Option<bool>(["--dump", "-d"], St.Fce_dumpOption_help);
        cmd.AddOption(humanOption);
        cmd.AddOption(rsvdContentOption);
        cmd.SetHandler(InfoCommand, fileArg, humanOption, rsvdContentOption);
        return cmd;
    }

    private static Task InfoCommand(FileInfo fceFile, bool humanOpt, bool dump)
    {
        return ReadOnlyFileTransaction<IFceFile<FcePart>, FceCommonSerializer>(fceFile, fce =>
        {
            foreach (var j in new FceInfoExtractor<FcePart>(humanOpt, dump).GetInfo(fce))
            {
                Console.WriteLine(j);
            }
        });
    }
}
