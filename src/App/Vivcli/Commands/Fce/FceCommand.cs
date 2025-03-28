using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Serializers.Fce.Nfs3;

namespace TheXDS.Vivianne.Commands.Fce;

public partial class FceCommand() : FileCommandBase<FceFile, FceSerializer>(
    "fce",
    "Performs operations on FCE files.",
    "fce file",
    "Path to the FCE file",
    [
        BuildInfoCommand,
    ]){ }