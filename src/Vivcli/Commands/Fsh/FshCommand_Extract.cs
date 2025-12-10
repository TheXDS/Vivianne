using System.CommandLine;
using System.Text.RegularExpressions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers.Fsh;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fsh;


public partial class FshCommand
{
    private static Command BuildExtractCommand(Argument<FileInfo> fshFile)
    {
        var cmd = new Command("extract", St.Extract_Help);
        var regexArg = new Argument<string>(St.Extract_RegexArg, () => "^.*$", St.Extract_RegexArgHelp);
        var outDirOption = new Option<DirectoryInfo>(["--directory", "-d"], () => new DirectoryInfo(Environment.CurrentDirectory), St.Extract_OutdirOptionHelp);
        var formatOption = new Option<ImageFormat>(["--format", "-f"], () => ImageFormat.Png, St.Extract_FormatOptionHelp);
        cmd.AddArgument(regexArg);
        cmd.AddOption(outDirOption);
        cmd.AddOption(formatOption);
        cmd.SetHandler(ExtractCommand, fshFile, regexArg, outDirOption, formatOption);
        cmd.AddAlias("xf");
        return cmd;
    }

    private static Task ExtractCommand(FileInfo fshFile, string regex, DirectoryInfo outDir, ImageFormat format)
    {
        return ReadOnlyFileTransaction<FshFile, FshSerializer>(fshFile, fsh =>
        {
            if (!outDir.Exists) outDir.Create();
            foreach (var j in fsh.Entries.Keys.Where(p => Regex.IsMatch(p, regex)))
            {
                var blob = fsh.Entries[j];
                if (blob.ToImage(blob?.ReadLocalPalette() ?? fsh.GetPalette()) is { } img)
                {
                    using var fs = File.OpenWrite(Path.Combine(outDir.FullName, $"{j}.{format.ToString().ToLower()}"));
                    img.Save(fs, Mappings.ImageFormatEnconder[format]);
                }
                if (blob?.Footer is not null && blob.Footer.Length > 0)
                {
                    File.WriteAllBytes(Path.Combine(outDir.FullName, $"{j}.footer"), blob.Footer);
                }
            }
        });
    }
}
