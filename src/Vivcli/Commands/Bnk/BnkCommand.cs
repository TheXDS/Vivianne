using TheXDS.Vivianne.Commands.Base;
using St = TheXDS.Vivianne.Resources.Strings.BnkCommand;

namespace TheXDS.Vivianne.Commands.Bnk;

/// <summary>
/// Defines a command that allows the user to interact with a BNK file.
/// </summary>
public partial class BnkCommand() : FileCommandBase(
    "bnk",
    St.Description,
    St.Arg1,
    St.Arg1Description,
    [
        BuildInfoCommand,
        BuildBlobInfoCommand,
        BuildExtractCommand,
        BuildReplaceCommand,
    ]) { }