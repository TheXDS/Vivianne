using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Viv;
using St = TheXDS.Vivianne.Resources.Strings.VivCommand;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", St.Info_Help);
        cmd.SetHandler(InfoCommand, fileArg);
        return cmd;
    }

    private static Task InfoCommand(FileInfo vivFile)
    {
        return ReadOnlyFileTransaction<VivFileHeader, VivHeaderSerializer>(vivFile, viv =>
        {
            var calcFileSize = VivSerializer.GetFileSize(viv.Entries.Select(p => new KeyValuePair<string, int>(p.Key, p.Value.Length)));
            Console.WriteLine(string.Format(St.Info_1, string.Join("", viv.Header.Magic.Select(p => p.ToString("X"))), System.Text.Encoding.Latin1.GetString(viv.Header.Magic)));
            Console.WriteLine(string.Format(St.Info_2, viv.Header.VivLength, ((long)viv.Header.VivLength).ByteUnits()));
            Console.WriteLine(string.Format(St.Info_3, calcFileSize, ((long)calcFileSize).ByteUnits()));
            Console.WriteLine(string.Format(St.Info_4, vivFile.Length, vivFile.Length.ByteUnits()));
            Console.WriteLine(string.Format(St.Info_5, viv.Header.Entries));
            Console.WriteLine(string.Format(St.Info_6, viv.Header.PoolOffset));
        });
    }
}