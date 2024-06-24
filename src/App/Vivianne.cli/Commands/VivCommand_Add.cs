using System.CommandLine;

namespace TheXDS.Vivianne.Commands;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildAddCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("add", "Adds a file to the VIV.");
        var fileArg = new Argument<FileInfo>("file", "Indicates the file to be added.").ExistingOnly();
        var forceOption = new Option<bool>(["--force", "-f"], "Force add the file, even if it exists already on the VIV file.");
        var altName = new Option<string?>(["--name", "-n"], () => null, "New name of the file. If omitted, the current file name wil be used.").LegalFileNamesOnly();
        cmd.AddArgument(fileArg);
        cmd.AddOption(forceOption);
        cmd.AddOption(altName);
        cmd.SetHandler(AddCommand, vivFile, fileArg, forceOption, altName);
        return cmd;
    }
    
    private static Task AddCommand(FileInfo vivFile, FileInfo fileToAdd, bool force, string? name)
    {
        return FileTransaction(vivFile, async viv => {
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
                Fail($"The specified VIV file contains '{name}' already.");
            }
        });
    }
}