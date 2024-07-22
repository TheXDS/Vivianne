using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using static System.Reflection.BindingFlags;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand() : FileCommandBase<VivFile, VivSerializer>(
    "fsh",
    "Performs operations on FSH and QFS files.",
    "fsh/qfs file",
    "Path to the FSH/QFS file.",
    typeof(FshCommand).GetMethods(Static | NonPublic))
{ }