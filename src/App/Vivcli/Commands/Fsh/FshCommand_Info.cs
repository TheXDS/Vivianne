using System.CommandLine;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.Tools;
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
    
    private static async Task InfoCommand(FileInfo fshFile, bool humanOpt)
    {
        ISerializer<FshFile> parser = new FshSerializer();
        var rawData = File.ReadAllBytes(fshFile.FullName);
        var uncompressed = QfsCodec.Decompress(rawData);
        var fsh = await parser.DeserializeAsync(uncompressed);
        fsh.IsCompressed = QfsCodec.IsCompressed(rawData);
        Console.WriteLine(string.Format(St.Info_Info1, fsh.Entries.Count));
        Console.WriteLine(string.Format(St.Info_Info2, fsh.DirectoryId));
        Console.WriteLine(string.Format(St.Info_Info3, rawData.Length.GetSize(humanOpt)));
        if (fsh.IsCompressed)
        {
            Console.WriteLine(St.Info_Info4);
            Console.WriteLine(string.Format(St.Info_Info5, uncompressed.Length.GetSize(humanOpt)));
            Console.WriteLine(string.Format(St.Info_Info6, (double)uncompressed.Length / rawData.Length));
        }
        else
        {
            Console.WriteLine(St.Info_Info7);
        }
    }
}