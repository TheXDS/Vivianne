using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", "Gets basic information on the VIV file.");
        cmd.SetHandler(InfoCommand, fileArg);
        return cmd;
    }

    private static async Task InfoCommand(FileInfo vivFile)
    {
        ISerializer<VivFileHeader> parser = new VivHeaderSerializer();
        using var fs = vivFile.OpenRead();
        var viv = await parser.DeserializeAsync(fs);
        var calcFileSize = VivSerializer.GetFileSize(viv.Entries.Select(p => new KeyValuePair<string, int>(p.Key, p.Value.Length)));
        Console.WriteLine($"Header signature: 0x{string.Join("", viv.Header.Magic.Select(p => p.ToString("X")))} ({System.Text.Encoding.Latin1.GetString(viv.Header.Magic)})");
        Console.WriteLine($"File size in header: {viv.Header.VivLength} ({((long)viv.Header.VivLength).ByteUnits()})");
        Console.WriteLine($"Calculated file size: {calcFileSize} ({((long)calcFileSize).ByteUnits()})");
        Console.WriteLine($"Actual file size: {vivFile.Length} ({vivFile.Length.ByteUnits()})");
        Console.WriteLine($"Files: {viv.Header.Entries}");
        Console.WriteLine($"Data pool offset: {viv.Header.PoolOffset}");
    }
}