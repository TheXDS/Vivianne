using System.CommandLine;
using TheXDS.Vivianne.Commands.Base;

namespace TheXDS.Vivianne.Commands.Map;

/// <summary>
/// Defines a command that allows the user to interact with a MAP or LIN file.
/// </summary>
public partial class MapCommand() : FileCommandBase(
    "map",
    "",
    "LIN/MAP",
    "",
    [
        BuildInfoCommand,
        BuildCheckCommand,
        BuildStitchCommand,
    ])
{
    /// <inheritdoc/>
    public override Command GetCommand()
    {
        var cmd = base.GetCommand();
        cmd.AddAlias("lin");
        return cmd;
    }
}