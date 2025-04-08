using System.CommandLine;
using TheXDS.Vivianne.Info.Fce;
using Nfs3File = TheXDS.Vivianne.Models.Fce.Nfs3.FceFile;
using Nfs3Srlz = TheXDS.Vivianne.Serializers.Fce.Nfs3.FceSerializer;
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
        return ReadOnlyFileTransaction<Nfs3File, Nfs3Srlz>(fceFile, fce =>
        {
            foreach (var j in new Fce3InfoExtractor(humanOpt, dump).GetInfo(fce))
            {
                Console.WriteLine(j);
            }
        });
    }
}
