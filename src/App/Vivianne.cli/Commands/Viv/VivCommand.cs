using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using static System.Reflection.BindingFlags;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand() : FileCommandBase<VivFile, VivSerializer>(
    "viv",
    "Performs operations on VIV files.",
    "viv file",
    "Path to the VIV file.",
    typeof(VivCommand).GetMethods(Static | NonPublic))
{ }