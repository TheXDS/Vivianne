using System.CommandLine;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers.Viv;
using St = TheXDS.Vivianne.Resources.Strings.VivCommand;

namespace TheXDS.Vivianne.Commands.Viv;

public partial class VivCommand
{
    private static Command BuildRenameCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("rename", St.Rename_Help);
        var oldName = new Argument<string>(St.Rename_Arg1, St.Rename_Arg1Help).LegalFileNamesOnly();
        var newName = new Argument<string>(St.Rename_Arg2, St.Rename_Arg2Help).LegalFileNamesOnly();
        cmd.AddArgument(oldName);
        cmd.AddArgument(newName);
        cmd.AddAlias("mv");
        cmd.AddAlias("ren");
        cmd.SetHandler(RenameCommand, vivFile, oldName, newName);
        return cmd;
    }

    private static Task RenameCommand(FileInfo vivFile, string oldName, string newName)
    {
        return FileTransaction<VivFile, VivSerializer>(vivFile, viv =>
        {
            var contents = viv[oldName];
            if (viv.Remove(oldName))
            {
                viv.Add(newName, contents);
            }
            else
            {
                Fail(string.Format(St.Rm_Fail, oldName));
            }
        });
    }
}
