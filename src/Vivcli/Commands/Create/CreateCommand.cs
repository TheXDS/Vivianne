using System.CommandLine;
using TheXDS.Vivianne.Commands.Base;

namespace TheXDS.Vivianne.Commands.Create;

/// <summary>
/// Defines a command to be used to create different kinds of files.
/// </summary>
public partial class CreateCommand : VivianneCommand
{
    private static readonly IEnumerable<Func<Argument<FileInfo>, Command>> CreateCommands = [CreateAsfCommand];

    /// <inheritdoc/>
    public override Command GetCommand()
    {
        var cmd = new Command("create", "Creates different kinds of files.");
        var fileArg = new Argument<FileInfo>("file", "Specifies the name of the file to be created.").LegalFilePathsOnly();
        foreach (var j in CreateCommands)
        {
            cmd.AddCommand(j.Invoke(fileArg));
        }
        return cmd;
    }
}
