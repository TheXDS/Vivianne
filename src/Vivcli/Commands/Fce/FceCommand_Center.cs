using System.CommandLine;
using static TheXDS.Vivianne.Tools.Fce.FceCenter;
using St = TheXDS.Vivianne.Resources.Strings.FceCommand;

namespace TheXDS.Vivianne.Commands.Fce;

public partial class FceCommand
{
    private static Command BuildCenterCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("center", St.Center_Help);
        cmd.SetHandler(CenterCommand, fileArg);
        return cmd;
    }

    private static Task CenterCommand(FileInfo fceFile)
    {
        return FceTransaction(fceFile, Center, Center);
    }
}
