using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers.Viv;
using St = TheXDS.Vivianne.Resources.Strings.VivCommand;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand() : FileCommandBase(
    "viv",
    St.Description,
    St.Arg1,
    St.Arg1Description,
    [
        BuildAddCommand, 
        BuildExtractCommand,
        BuildInfoCommand,
        BuildLsCommand,
        BuildReadCommand,
        BuildRenameCommand,
        BuildRmCommand
    ]) { }