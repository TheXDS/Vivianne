using System.CommandLine;
using System.Text.RegularExpressions;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers.Viv;
using St = TheXDS.Vivianne.Resources.Strings.VivCommand;

namespace TheXDS.Vivianne.Commands.Viv;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public partial class VivCommand
{
    private static Command BuildExtractCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("extract", St.Extract_Help);
        var regexArg = new Argument<string>(St.Extract_Arg1, () => "^.*$", St.Extract_Arg1Help);
        var outDirOption = new Option<DirectoryInfo>(["--directory", "-d"], () => new DirectoryInfo(Environment.CurrentDirectory), St.Extract_DirectoryHelp);
        cmd.AddArgument(regexArg);
        cmd.AddOption(outDirOption);
        cmd.SetHandler(ExtractCommand, vivFile, regexArg, outDirOption);
        cmd.AddAlias("xf");
        return cmd;
    }

    private static Task ExtractCommand(FileInfo vivFile, string regex, DirectoryInfo outDir)
    {
        return ReadOnlyFileTransaction<VivFile, VivSerializer>(vivFile, viv =>
        {
            if (!outDir.Exists) outDir.Create();
            foreach (var j in viv.Keys.Where(p => Regex.IsMatch(p, regex)))
            {
                using var fs = File.OpenWrite(Path.Combine(outDir.FullName, j));
                using var bw = new BinaryWriter(fs);
                bw.Write(viv[j]);
            }
        });
    }
}