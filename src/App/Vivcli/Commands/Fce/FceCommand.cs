using TheXDS.Vivianne.Commands.Base;

namespace TheXDS.Vivianne.Commands.Fce;

/// <summary>
/// Defines a command that allows the user to interact with an FCE file.
/// </summary>
public partial class FceCommand() : FileCommandBase(
    "fce",
    "Performs operations on FCE files.",
    "fce file",
    "Path to the FCE file",
    [
        BuildInfoCommand,
        BuildLsCommand,
    ]){ }