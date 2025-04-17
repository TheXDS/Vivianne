using System.CommandLine;
using TheXDS.Vivianne.Info.Fce;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Serializers.Fce.Common;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fce;

public partial class FceCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", "Gets information on the FCE file");
        var humanOption = new Option<bool>(["--human", "-H"], St.Common_HumanOptionHelp);
        var rsvdContentOption = new Option<bool>(["--dump", "-d"], "Dumps the contents of the data tables inside the FCE file.");
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
