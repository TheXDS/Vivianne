using System.CommandLine;
using System.Text.RegularExpressions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.Commands.Fsh;


public partial class FshCommand
{
    private static Command BuildExtractCommand(Argument<FileInfo> fshFile)
    {
        var cmd = new Command("extract", "Extracts an image blob from within the specified FSH.");
        var regexArg = new Argument<string>("blob(s) regex", () => "^.*$", "Expression/regex that indicates the blob(s) to be extracted from within the specified FSH. If omitted, all blobs will be extracted.");
        var outDirOption = new Option<DirectoryInfo>(["--directory", "-d"], () => new DirectoryInfo(Environment.CurrentDirectory), "Output directory to write the blob(s) into. Defaults to the current path.");
        var formatOption = new Option<ImageFormat>(["--format", "-f"], () => ImageFormat.Png, "File format to use when exporting the blobs to images.");
        cmd.AddArgument(regexArg);
        cmd.AddOption(outDirOption);
        cmd.AddOption(formatOption);
        cmd.SetHandler(ExtractCommand, fshFile, regexArg, outDirOption, formatOption);
        return cmd;
    }

    private static Task ExtractCommand(FileInfo fshFile, string regex, DirectoryInfo outDir, ImageFormat format)
    {
        return FileTransaction(fshFile, fsh =>
        {
            foreach (var j in fsh.Entries.Keys.Where(p => Regex.IsMatch(p, regex)))
            {
                var blob = fsh.Entries[j];
                if (blob.ToImage(blob?.LocalPalette ?? fsh.GetPalette()) is {} img)
                {
                    using var fs = File.OpenWrite(Path.Combine(outDir.FullName, $"{j}.{format.ToString().ToLower()}"));             
                    img.Save(fs,Mappings.ImageFormatEnconder[format]);
                }
                if (blob?.Footer is not null && blob.Footer.Length > 0)
                {
                    File.WriteAllBytes(Path.Combine(outDir.FullName, $"{j}.footer"), blob.Footer);
                }
            }
        }, true);
    }
}
