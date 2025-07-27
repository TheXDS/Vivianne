using System.CommandLine;

namespace TheXDS.Vivianne.Commands.Base;

/// <summary>
/// Base class for all commands that receive a file as its primary argument.
/// </summary>
/// <param name="alias">Alias of the command.</param>
/// <param name="help">Description of the command.</param>
/// <param name="fileArgName">Alias of the file argument.</param>
/// <param name="fileArgHelp">Description of the file argument.</param>
/// <param name="exposedCommands">
/// Collection of internal methods to search for sub-command definition methods
/// to be invoked.
/// </param>
public abstract class FileCommandBase(
    string alias,
    string help,
    string fileArgName,
    string fileArgHelp,
    IEnumerable<Func<Argument<FileInfo>, Command>> exposedCommands) : VivianneCommand
{
    private readonly string _alias = alias;
    private readonly string _help = help;
    private readonly string _fileArgName = fileArgName;
    private readonly string _fileArgHelp = fileArgHelp;
    private readonly IEnumerable<Func<Argument<FileInfo>, Command>> _commands = exposedCommands;

    /// <inheritdoc/>
    public override Command GetCommand()
    {
        var cmd = new Command(_alias, _help);
        var file = new Argument<FileInfo>(_fileArgName, _fileArgHelp).ExistingOnly();
        cmd.AddArgument(file);
        foreach (var j in _commands.Select(p => p.Invoke(file))) cmd.AddCommand(j);
        return cmd;
    }
}
