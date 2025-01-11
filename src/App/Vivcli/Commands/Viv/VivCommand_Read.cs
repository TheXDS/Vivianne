using System.CommandLine;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildReadCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("read", "Reads a file from within the specified VIV.");
        var fileNameArg = new Argument<string>("file", "File to read from within the specified VIV.").LegalFileNamesOnly();
        var outOption = new Option<FileInfo?>(["--output", "-o"], () => null, "If specified, indicates the path to write the file into. If omitted, the file will be read to standard output.");
        cmd.AddArgument(fileNameArg);
        cmd.AddOption(outOption);
        cmd.SetHandler(ReadCommand, vivFile, fileNameArg, outOption);
        return cmd;
    }

    private static Task ReadCommand(FileInfo vivFile, string fileName, FileInfo? outputStream)
    {
        if (fileName.IsEmpty()) Fail("You must specify a file name.");
        return FileTransaction(vivFile, viv =>
        {
            if (viv.TryGetValue(fileName, out var contents))
            {
                using var bw = new BinaryWriter(outputStream?.Create() ?? Console.OpenStandardOutput());
                bw.Write(contents);
            }
            else
            {
                Fail($"The specified VIV file does not contain '{fileName}'.");
            }
        }, true);
    }
}