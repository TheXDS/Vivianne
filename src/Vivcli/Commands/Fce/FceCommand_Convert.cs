using System.CommandLine;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Tools.Fce;
using M3 = TheXDS.Vivianne.Models.Fce.Nfs3;
using M4 = TheXDS.Vivianne.Models.Fce.Nfs4;
using S3 = TheXDS.Vivianne.Serializers.Fce.Nfs3;
using S4 = TheXDS.Vivianne.Serializers.Fce.Nfs4;
using St = TheXDS.Vivianne.Resources.Strings.FceCommand;

namespace TheXDS.Vivianne.Commands.Fce;

public partial class FceCommand
{
    private static Command BuildConvertCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("convert", St.Convert_description);
        var outFile = new Option<FileInfo?>(["--out", "-o"], () => null, St.OutOptionHelp).LegalFilePathsOnly();
        cmd.AddOption(outFile);
        cmd.AddAlias("conv");
        cmd.SetHandler(ConvertCommand, fileArg, outFile);
        return cmd;
    }

    private static Task ConvertCommand(FileInfo fceFile, FileInfo? outFile)
    {
        return ReadOnlyFceTransaction(fceFile,
            fce3 => DoConvert<M3.FceFile, M4.FceFile, M4.Fce4Part, S4.FceSerializer>(fceFile, outFile, fce3, FceConverter.ToNfs4),
            fce4 => DoConvert<M4.FceFile, M3.FceFile, FcePart, S3.FceSerializer>(fceFile, outFile, fce4, FceConverter.ToNfs3));
    }

    private static async Task DoConvert<TInFce, TOutFce, TPart, TSerializer>(FileInfo fceFile, FileInfo? outFile, TInFce inFce, Func<TInFce, TOutFce> conversionCallback)
        where TPart : FcePart
        where TOutFce : IFceFile<TPart>
        where TSerializer : ISerializer<TOutFce>, new()
    {
        TOutFce outFce = conversionCallback.Invoke(inFce);
        ISerializer<TOutFce> serializer = new TSerializer();
        outFile ??= fceFile;
        if (outFile.Exists) outFile.Delete();
        using var fs = outFile.Create();
        await serializer.SerializeToAsync(outFce, fs);
    }
}
