using System.CommandLine;
using System.Text.RegularExpressions;

namespace TheXDS.Vivianne.Commands;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildExtractCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("extract", "Extracts a file from within the specified VIV.");
        var regexArg = new Argument<string>("file(s) regex", () => "^.*$", "File name/regex that indicates the file(s) to be extracted from within the specified VIV. If omitted, all files will be extracted.");
        var outDirOption = new Option<DirectoryInfo>(["--directory", "-d"], () => new DirectoryInfo(Environment.ProcessPath ?? "."), "Output directory to write the file(s) into. Defaults to the current path.");
        cmd.AddArgument(regexArg);
        cmd.AddOption(outDirOption);
        cmd.SetHandler(ExtractCommand, vivFile, regexArg, outDirOption);
        return cmd;
    }

    private static Task ExtractCommand(FileInfo vivFile, string regex, DirectoryInfo outDir)
    {
        return FileTransaction(vivFile, viv =>
        {
            foreach (var j in viv.Keys.Where(p => Regex.IsMatch(p, regex)))
            {
                using var fs = File.OpenWrite(Path.Combine(outDir.FullName, j));
                using var bw = new BinaryWriter(fs);
                bw.Write(viv[j]);
            }
        }, true);
    }
}