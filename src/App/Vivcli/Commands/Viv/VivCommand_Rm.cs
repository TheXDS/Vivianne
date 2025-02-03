using System.CommandLine;
using St = TheXDS.Vivianne.Resources.Strings.VivCommand;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildRmCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("rm", St.Rm_Help);
        var name = new Argument<string>(St.Rm_Arg1, St.Rm_Arg1Help).LegalFileNamesOnly();
        cmd.AddArgument(name);
        cmd.SetHandler(RmCommand, vivFile, name);
        return cmd;
    }

    private static Task RmCommand(FileInfo vivFile, string name)
    {
        return FileTransaction(vivFile, viv =>
        {
            if (!viv.Remove(name))
            {
                Fail(string.Format(St.Rm_Fail, name));
            }
        });
    }
}