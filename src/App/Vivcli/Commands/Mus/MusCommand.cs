using System.CommandLine;
using TheXDS.Vivianne.Commands.Base;

namespace TheXDS.Vivianne.Commands.Mus;

/// <summary>
/// Defines a command that allows the user to interact with an ASF/MUS file.
/// </summary>
public partial class MusCommand() : FileCommandBase(
    "asf",
    "Performs operations on ASF/MUS files.",
    "ASF/MUS file",
    "Path to the ASF/MUS file.",
    [
        BuildInfoCommand,
        BuildExtractCommand,
        BuildExtractBlobCommand,
    ])
{
    /// <inheritdoc/>
    public override Command GetCommand()
    {
        var cmd = base.GetCommand();
        cmd.AddAlias("mus");
        return cmd;
    }
}
