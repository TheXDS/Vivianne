using System.CommandLine;
using TheXDS.Vivianne.Codecs;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers.Fsh;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", St.Info_Help);
        var humanOption = new Option<bool>(["--human", "-H"], St.Common_HumanOptionHelp);
        cmd.AddOption(humanOption);
        cmd.SetHandler(InfoCommand, fileArg, humanOption);
        return cmd;
    }
    
    private static Task InfoCommand(FileInfo fshFile, bool humanOpt)
    {
        return ReadOnlyFileTransaction<FshFile, FshSerializer>(fshFile, async fsh =>
        {
            Console.WriteLine(string.Format(St.Info_Info1, fsh.Entries.Count));
            Console.WriteLine(string.Format(St.Info_Info2, fsh.DirectoryId));
            Console.WriteLine(string.Format(St.Info_Info3, fshFile.Length.GetSize(humanOpt)));
            if (fsh.IsCompressed)
            {
                var uncompressedDataLength = LzCodec.Decompress(await File.ReadAllBytesAsync(fshFile.FullName)).Length;
                Console.WriteLine(St.Info_Info4);
                Console.WriteLine(string.Format(St.Info_Info5, uncompressedDataLength.GetSize(humanOpt)));
                Console.WriteLine(string.Format(St.Info_Info6, (double)uncompressedDataLength / fshFile.Length));
            }
            else
            {
                Console.WriteLine(St.Info_Info7);
            }
        });
    }
}