using TheXDS.Vivianne.Commands.Base;

namespace TheXDS.Vivianne.Commands.Bnk;

/// <summary>
/// Defines a command that allows the user to interact with a BNK file.
/// </summary>
public partial class BnkCommand() : FileCommandBase(
    "bnk",
    "Performs operations on BNK files.",
    "bnk file",
    "Path to the BNK file",
    [
        BuildInfoCommand,
        BuildBlobInfoCommand,
        BuildExtractCommand,
        BuildReplaceCommand,
    ]) { }