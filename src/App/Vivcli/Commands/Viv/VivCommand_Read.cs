using System.CommandLine;
using TheXDS.MCART.Types.Extensions;
using St = TheXDS.Vivianne.Resources.Strings.VivCommand;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildReadCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("read", St.Read_Help);
        var fileNameArg = new Argument<string>(St.Read_Arg1, St.Read_FileHelp).LegalFileNamesOnly();
        var outOption = new Option<FileInfo?>(["--output", "-o"], () => null, St.Read_OutputHelp);
        cmd.AddArgument(fileNameArg);
        cmd.AddOption(outOption);
        cmd.SetHandler(ReadCommand, vivFile, fileNameArg, outOption);
        return cmd;
    }

    private static Task ReadCommand(FileInfo vivFile, string fileName, FileInfo? outputStream)
    {
        if (fileName.IsEmpty()) Fail(St.Read_Fail1);
        return FileTransaction(vivFile, viv =>
        {
            if (viv.TryGetValue(fileName, out var contents))
            {
                using var bw = new BinaryWriter(outputStream?.Create() ?? Console.OpenStandardOutput());
                bw.Write(contents);
            }
            else
            {
                Fail(string.Format(St.Read_Fail2, fileName));
            }
        }, true);
    }
}