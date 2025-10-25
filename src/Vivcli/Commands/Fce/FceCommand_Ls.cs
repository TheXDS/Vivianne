using System.CommandLine;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Serializers.Fce.Common;
using St = TheXDS.Vivianne.Resources.Strings.FceCommand;

namespace TheXDS.Vivianne.Commands.Fce;

public partial class FceCommand
{
    private static Command BuildLsCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("ls", St.Ls_help);
        var typeOption = new Option<FceObjectType>(["--type", "-t"], () => FceObjectType.Any, St.TypeOption_Help);

        cmd.AddOption(typeOption);
        cmd.SetHandler(LsCommand, fileArg, typeOption);
        return cmd;
    }

    private static Task LsCommand(FileInfo fshFile, FceObjectType typeOpt)
    {
        return ReadOnlyFileTransaction<IFceFile<FcePart>, FceCommonSerializer>(fshFile, fsh =>
        {
            if (typeOpt.HasFlag(FceObjectType.Part))
            {
                foreach (var j in fsh.Parts) Console.WriteLine(j.Name);
            }
            if (typeOpt .HasFlag(FceObjectType.Dummy))
            {
                foreach (var j in fsh.Dummies) Console.WriteLine(j.Name);
            }
        });
    }
}
