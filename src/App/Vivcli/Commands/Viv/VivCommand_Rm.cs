using System.CommandLine;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildRmCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("rm", "Removes a file from the VIV file.");
        var name = new Argument<string>("file", "File to be removed.").LegalFileNamesOnly();
        cmd.AddArgument(name);
        cmd.SetHandler(RmCommand, vivFile,name);
        return cmd;
    }

    private static Task RmCommand(FileInfo vivFile, string name)
    {
        return FileTransaction(vivFile, viv => {
            if (!viv.Remove(name))
            {
                Fail($"'{name}' does not exist in the specified VIV file.");
            }
        });
    }
}