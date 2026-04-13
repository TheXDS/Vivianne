using System.CommandLine;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers.Fsh;
using St = TheXDS.Vivianne.Resources.Strings.FshCommand;

namespace TheXDS.Vivianne.Commands.Fsh;

public partial class FshCommand
{
    private static Command BuildFooterCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("footer", St.Footer_help);
        var blobArg = new Argument<string>(St.BlobInfo_Arg1, St.Footer_nameArg);
        cmd.AddArgument(blobArg);
        cmd.AddCommand(BuildFooterInfoCommand(fileArg, blobArg));
        cmd.AddCommand(BuildFooterPrintCommand(fileArg, blobArg));
        cmd.AddCommand(BuildFooterNewCommand(fileArg, blobArg));
        cmd.AddCommand(BuildFooterRmCommand(fileArg, blobArg));
        return cmd;
    }

    private static Command BuildFooterInfoCommand(Argument<FileInfo> fileArg, Argument<string> blobArg)
    {
        var cmd = new Command("info", St.Footer_info);
        cmd.AddAlias("show");
        cmd.AddAlias("details");
        cmd.SetHandler(FooterInfoCommand, fileArg, blobArg);
        return cmd;
    }

    private static Command BuildFooterPrintCommand(Argument<FileInfo> fileArg, Argument<string> blobArg)
    {
        var cmd = new Command("print", St.Footer_print);
        var outputOption = new Option<FileInfo?>(["--output", "-o"], () => null, St.Footer_print_outputArg);
        cmd.AddAlias("cat");
        cmd.AddAlias("dump");
        cmd.AddOption(outputOption);
        cmd.SetHandler(FooterPrintCommand, fileArg, blobArg, outputOption);
        return cmd;
    }

    private static Command BuildFooterNewCommand(Argument<FileInfo> fileArg, Argument<string> blobArg)
    {
        var cmd = new Command("new", St.Footer_new);
        var footerTypeArg = new Argument<FshBlobFooterType>("footer type", St.Footer_new_typeArg);
        cmd.AddAlias("add");
        cmd.AddArgument(footerTypeArg);
        cmd.SetHandler(FooterNewCommand, fileArg, blobArg, footerTypeArg);
        return cmd;
    }

    private static Command BuildFooterRmCommand(Argument<FileInfo> fileArg, Argument<string> blobArg)
    {
        var cmd = new Command("remove", St.Footer_remove);
        cmd.AddAlias("rm");
        cmd.AddAlias("del");
        cmd.SetHandler(FooterRemoveCommand, fileArg, blobArg);
        return cmd;
    }

    private static Task FooterInfoCommand(FileInfo file, string blob)
    {
        return ReadOnlyFshBlobOp(file, blob, (fsh, blob) => Console.WriteLine(GetFshBlobFooterLabel(blob, true)));
    }

    private static void FooterNewCommand(FileInfo file, string blob, FshBlobFooterType footerType) => FshBlobOp(file, blob, (f, b) =>
    {
        if (!Mappings.FshFooterBuilder.TryGetValue(footerType, out var footerFactory)) Fail(St.Footer_new_unkFooterType);
        b.Footer = [.. b.Footer, .. footerFactory.Invoke()];
    });

    private static Task FooterRemoveCommand(FileInfo file, string blob) => FshBlobOp(file, blob, (_, b) => b.Footer = []);

    private static Task FooterPrintCommand(FileInfo file, string blob, FileInfo? outputFile) => ReadOnlyFileTransaction<FshFile, FshSerializer>(file, fsh =>
    {
        return FshBlobOp(blob, file, async (_, b) =>
        {
            using var outStream = outputFile?.OpenWrite() ?? Console.OpenStandardOutput();
            await outStream.WriteAsync(b.Footer);
            await outStream.FlushAsync();
        });
    });

    private static Task FshBlobOp(string blob, FileInfo file, Func<FshFile, FshBlob, Task> op)
    {
        return FshBlobOp(blob, file, FileTransaction<FshFile, FshSerializer>, op);
    }

    private static Task ReadOnlyFshBlobOp(string blob, FileInfo file, Func<FshFile, FshBlob, Task> op)
    {
        return FshBlobOp(blob, file, ReadOnlyFileTransaction<FshFile, FshSerializer>, op);
    }

    private static Task FshBlobOp(FileInfo file, string blob, Action<FshFile, FshBlob> op) => FshBlobOp(blob, file, FileTransaction<FshFile, FshSerializer>, (f, b) =>
    {
        op.Invoke(f, b);
        return Task.CompletedTask;
    });

    private static Task ReadOnlyFshBlobOp(FileInfo file, string blob, Action<FshFile, FshBlob> op) => FshBlobOp(blob, file, ReadOnlyFileTransaction<FshFile, FshSerializer>, (f, b) =>
    {
        op.Invoke(f, b);
        return Task.CompletedTask;
    });

    private static Task FshBlobOp(string blob, FileInfo file, Func<FileInfo, Func<FshFile, Task>, Task> transactionRunner, Func<FshFile, FshBlob, Task> op)
    {
        if (blob.IsEmpty()) Fail(St.BlobIdRequired);
        return transactionRunner.Invoke(file, async fsh =>
        {
            if (fsh.Entries.TryGetValue(blob, out var b))
            {
                await op.Invoke(fsh, b);
            }
            else
            {
                Fail(St.BlobInfo_Fail);
            }
        });
    }
}
