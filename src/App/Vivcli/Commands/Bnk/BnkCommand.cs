using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers.Bnk;
using TheXDS.Vivianne.Serializers.Fsh;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

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
        BuildBlobInfoCommand,
        BuildExtractCommand,
    ]) { }