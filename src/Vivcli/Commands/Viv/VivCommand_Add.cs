using System.CommandLine;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers.Viv;
using St = TheXDS.Vivianne.Resources.Strings.VivCommand;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildAddCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("add", St.Add_Help);
        var fileArg = new Argument<FileInfo>(St.Add_Arg1, St.Add_Arg1Help).ExistingOnly();
        var forceOption = new Option<bool>(["--force", "-f"], St.Add_ForceHelp);
        var altName = new Option<string?>(["--name", "-n"], () => null, St.Add_NameHelp).LegalFileNamesOnly();
        cmd.AddArgument(fileArg);
        cmd.AddOption(forceOption);
        cmd.AddOption(altName);
        cmd.SetHandler(AddCommand, vivFile, fileArg, forceOption, altName);
        return cmd;
    }
    
    private static Task AddCommand(FileInfo vivFile, FileInfo fileToAdd, bool force, string? name)
    {
        return FileTransaction<VivFile, VivSerializer>(vivFile, async viv => {
            name ??= fileToAdd.Name;
            if (!viv.ContainsKey(name) || force)
            {
                using var ms = new MemoryStream();
                using var fs = fileToAdd.OpenRead();
                await fs.CopyToAsync(ms);
                viv[name] = ms.ToArray();
            }
            else
            {
                Fail(string.Format(St.Add_Fail, name));
            }
        });
    }
}