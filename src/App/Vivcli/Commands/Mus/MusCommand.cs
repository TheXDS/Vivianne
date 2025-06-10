using TheXDS.Vivianne.Commands.Base;

namespace TheXDS.Vivianne.Commands.Mus;

/// <summary>
/// Defines a command that allows the user to interact with a BNK file.
/// </summary>
public partial class MusCommand() : FileCommandBase(
    "mus",
    "Performs operations on MUS files.",
    "MUS file",
    "Path to the MUS file.",
    [
        BuildInfoCommand,
        BuildExtractCommand,
        BuildExtractBlobCommand,
    ]);