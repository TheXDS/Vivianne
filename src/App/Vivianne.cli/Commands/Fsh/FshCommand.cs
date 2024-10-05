using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using static System.Reflection.BindingFlags;

namespace TheXDS.Vivianne.Commands.Fsh;

/// <summary>
/// Defines a command that allows the user to interact with a FSH or QFS file.
/// </summary>
public partial class FshCommand() : FileCommandBase<FshFile, FshSerializer>(
    "fsh",
    "Performs operations on FSH and QFS files.",
    "fsh/qfs file",
    "Path to the FSH/QFS file.",
    typeof(FshCommand).GetMethods(Static | NonPublic))
{
    private static long EstimateSize(FshBlob blob)
    {
        return 13 + blob.PixelData.Length + blob.Footer.Length;
    }

    private static string GetSize(long bytes, bool humanReadable)
    {
        return humanReadable ? bytes.ByteUnits() : bytes.ToString();
    }
 }