using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers.Fce.Common;
using TheXDS.Vivianne.Serializers.Fsh;

namespace TheXDS.Vivianne.Commands.Fce;

public partial class FceCommand
{
    private enum FceObjectType
    {
        All,
        Parts,
        Dummies,
    }

    private static Command BuildLsCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("ls", "Enumerates the objects that exist inside the FCE file.");
        var typeOption = new Option<FceObjectType>(["--type", "-t"], () => FceObjectType.All, "Specifies the kind of object to be listed.");

        cmd.AddOption(typeOption);
        cmd.SetHandler(LsCommand, fileArg, typeOption);
        return cmd;
    }

    private static Task LsCommand(FileInfo fshFile, FceObjectType typeOpt)
    {
        return ReadOnlyFileTransaction<IFceFile<FcePart>, FceCommonSerializer>(fshFile, fsh =>
        {
            if (typeOpt == FceObjectType.All || typeOpt == FceObjectType.Parts) {
                foreach (var j in fsh.Parts) Console.WriteLine(j.Name);
            }
            if (typeOpt == FceObjectType.All || typeOpt == FceObjectType.Dummies)
            {
                foreach (var j in fsh.Dummies) Console.WriteLine(j.Name);
            }
        });
    }
}
