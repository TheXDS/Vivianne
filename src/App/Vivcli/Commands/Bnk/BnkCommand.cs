using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Serializers.Bnk;

namespace TheXDS.Vivianne.Commands.Bnk;

/// <summary>
/// Defines a command that allows the user to interact with a FSH or QFS file.
/// </summary>
public partial class BnkCommand() : FileCommandBase<BnkFile, BnkSerializer>(
    "bnk",
    "Performs operations on BNK files.",
    "bnk file",
    "Path to the BNK file",
    [
        BuildInfoCommand,
        BuildBlobInfoCommand,
        BuildExtractCommand,
    ]) { }