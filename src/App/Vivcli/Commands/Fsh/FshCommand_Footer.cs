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
        var cmd = new Command("footer", "Manages the footer data for a single blob in a FSH/QFS file.");
        var blobArg = new Argument<string>(St.BlobInfo_Arg1, "Name of the blob to manage.");
        cmd.AddArgument(blobArg);
        cmd.AddCommand(BuildFooterPrintCommand(fileArg, blobArg));
        cmd.AddCommand(BuildFooterNewCommand(fileArg, blobArg));
        cmd.AddCommand(BuildFooterRmCommand(fileArg, blobArg));
        return cmd;
    }

    private static Command BuildFooterPrintCommand(Argument<FileInfo> fileArg, Argument<string> blobArg)
    {
        var cmd = new Command("print", "Prints the raw contents of the footer.");
        var outputOption = new Option<FileInfo?>(["--output", "-o"], () => null,"Specifies a file to write the footer contents to. If not specified, the contents will be written to stdout.");
        cmd.AddAlias("cat");
        cmd.AddAlias("dump");
        cmd.AddOption(outputOption);
        cmd.SetHandler(FooterPrintCommand, fileArg, blobArg, outputOption);
        return cmd;
    }

    private static Command BuildFooterNewCommand(Argument<FileInfo> fileArg, Argument<string> blobArg)
    {
        var cmd = new Command("new", "Creates a new footer for a single blob in a FSH/QFS file. The new data will be attached at the end of the footer data if present.");
        var footerTypeArg = new Argument<FshBlobFooterType>("footer type", "Type of footer to create.");
        cmd.AddAlias("add");
        cmd.AddArgument(footerTypeArg);
        cmd.SetHandler(FooterNewCommand, fileArg, blobArg, footerTypeArg);
        return cmd;
    }

    private static Command BuildFooterRmCommand(Argument<FileInfo> fileArg, Argument<string> blobArg)
    {
        var cmd = new Command("remove", "Removes the footer data for a single blob in a FSH/QFS file.");
        cmd.AddAlias("rm");
        cmd.AddAlias("del");
        cmd.SetHandler(FooterRemoveCommand, fileArg, blobArg);
        return cmd;
    }

    private static void FooterNewCommand(FileInfo file, string blob, FshBlobFooterType footerType) => FshBlobOp(blob, file, (f, b) =>
    {
        if (!Mappings.FshFooterBuilder.TryGetValue(footerType, out var footerFactory)) Fail("Unknown footer type");
        b.Footer = [.. b.Footer, ..footerFactory.Invoke()];
    });

    private static Task FooterRemoveCommand(FileInfo file, string blob) => FshBlobOp(blob, file, (_, b) => b.Footer = []);

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

    private static Task FshBlobOp(string blob, FileInfo file, Action<FshFile, FshBlob> op) => FshBlobOp(blob, file, FileTransaction<FshFile, FshSerializer>, (f, b) =>
    { 
        op.Invoke(f, b);
        return Task.CompletedTask;
    });

    private static Task ReadOnlyFshBlobOp(string blob, FileInfo file, Action<FshFile, FshBlob> op) => FshBlobOp(blob, file, ReadOnlyFileTransaction<FshFile, FshSerializer>, (f, b) =>
    {
        op.Invoke(f, b);
        return Task.CompletedTask;
    });

    private static Task FshBlobOp(string blob, FileInfo file, Func<FileInfo, Func<FshFile, Task>, Task> transactionRunner, Func<FshFile, FshBlob, Task> op)
    {
        if (blob.IsEmpty()) Fail("A blob ID is required.");
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
