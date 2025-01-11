using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fsh;

/// <summary>
/// Defines a command that allows the user to interact with a FSH or QFS file.
/// </summary>
public partial class FshCommand() : FileCommandBase<FshFile, FshSerializer>(
    "fsh",
    St.Description,
    St.Arg1,
    St.Arg1Description,
    [
        BuildBlobInfoCommand,
        BuildCompressCommand,
        BuildDecompressCommand,
        BuildExtractCommand,
        BuildInfoCommand,
        BuildLsCommand,
    ]) { }