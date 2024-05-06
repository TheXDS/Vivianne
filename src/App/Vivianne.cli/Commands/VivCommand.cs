using System.CommandLine;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Commands;

/// <summary>
/// Defines a command that allows the user to interact with a VIV file.
/// </summary>
public class VivCommand : VivianneCommand
{
    private static Command BuildAddCommand(Argument<FileInfo> vivFile)
    {
        var cmd = new Command("add", "Adds a file to the VIV file.");
        var fileArg = new Argument<FileInfo>("file", "Indicates the file to be added.").ExistingOnly();
        var forceOption = new Option<bool>(["--force", "-f"], "Force add the file, even if it exists already on the VIV file.");
        var altName = new Option<string?>(["--name", "-n"], () => null, "New name of the file.").LegalFileNamesOnly();
        cmd.AddArgument(fileArg);
        cmd.AddOption(forceOption);
        cmd.AddOption(altName);
        cmd.SetHandler(AddCommand, vivFile, fileArg, forceOption, altName);
        return cmd;
    }

    private static Command BuildInfoCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("info", "Gets basic information on the VIV file.");
        cmd.SetHandler(InfoCommand, fileArg);
        return cmd;
    }

    private static Command BuildLsCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("ls", "Enumerates all files on the VIV file.");
        var sizeOption = new Option<bool>(["--size", "-s"], "Includes the file size in the listing.");
        var offsetOption = new Option<bool>(["--offset", "-o"], "Includes the file offset in the listing.");
        var humanOption = new Option<bool>(["--human", "-H"], "File sizes will be formatted in human-readable format.");
        var decOption = new Option<bool>(["--dec", "-d"], "The file offsets will be preented in decimal format.");
        cmd.AddOption(sizeOption);
        cmd.AddOption(offsetOption);
        cmd.AddOption(humanOption);
        cmd.AddOption(decOption);
        cmd.SetHandler(LsCommand, fileArg, sizeOption, offsetOption, humanOption, decOption);
        return cmd;
    }

    private static Command BuildReadToDirCommand(Argument<FileInfo> vivFile, Argument<string> fileNameOption)
    {
        var cmd = new Command("extract", "Extracts a file from within the specified VIV.");        
        var outputDirOption = new Option<DirectoryInfo>(["--directory", "-d"], "Output path to write the file into.");
        cmd.AddArgument(fileNameOption);
        cmd.SetHandler((viv, f, dOpt)=> ReadCommand(viv, f, File.OpenWrite(Path.Combine(dOpt.FullName, f))), vivFile, fileNameOption, outputDirOption);
        return cmd;
    }

    private static IEnumerable<Command> BuildReadCommands(Argument<FileInfo> vivFile)
    {
        var fileNameOption = new Argument<string>("file", "File to read from within the specified VIV.").LegalFileNamesOnly();
        yield return BuildReadToFileCommand(vivFile, fileNameOption);
        yield return BuildReadToDirCommand(vivFile, fileNameOption);
    }

    private static Command BuildReadToFileCommand(Argument<FileInfo> vivFile, Argument<string> fileNameOption)
    {
        var cmd = new Command("read", "Reads a file from within the specified VIV.");
        var outputFileOption = new Option<FileInfo?>(["--output", "-o"], "If specified, indicates the output file to write the file into.");
        cmd.AddArgument(fileNameOption);
        cmd.SetHandler((viv, f, fOpt)=> ReadCommand(viv, f, fOpt?.Create() ?? Console.OpenStandardOutput()), vivFile, fileNameOption, outputFileOption);
        return cmd;
    }

    private static async Task AddCommand(FileInfo vivFile, FileInfo fileToAdd, bool force, string? name)
    {
        ISerializer<VivFile> serializer = new VivSerializer();
        VivFile viv;
        using (var fs = vivFile.OpenRead())
        {
            viv = await serializer.DeserializeAsync(fs);
        }
        name ??=fileToAdd.Name;
        if (!viv.ContainsKey(name) || force)
        {
            using var ms = new MemoryStream();
            using var fs = fileToAdd.OpenRead();
            await fs.CopyToAsync(ms);
            viv[name] = ms.ToArray();
        }
        else
        {
            Fail($"The specified VIV file contains '{name}' already.");
        }
        using (var fs = vivFile.OpenWrite())
        {
            fs.Destroy();
            await serializer.SerializeToAsync(viv, fs);
        }
    }

    private static async Task ReadCommand(FileInfo vivFile, string fileName, Stream outputStream)
    {
        if (fileName.IsEmpty()) Fail("You must specify a file name.");        
        ISerializer<VivFile> parser = new VivSerializer();
        using var fs = vivFile.OpenRead();
        var viv = await parser.DeserializeAsync(fs);
        if(viv.TryGetValue(fileName, out var contents))
        {
            using var bw = new BinaryWriter(outputStream);
            bw.Write(contents);
        }
        else
        {
            Fail($"The specified VIV file does not contain '{fileName}'.");
        }
    }

    private static async Task InfoCommand(FileInfo vivFile)
    {
        ISerializer<VivFileHeader> parser = new VivHeaderSerializer();
        using var fs = vivFile.OpenRead();
        var viv = await parser.DeserializeAsync(fs);
        var calcFileSize = VivSerializer.GetFileSize(viv.Entries.Select(p => new KeyValuePair<string, int>(p.Key, p.Value.Length)));
        Console.WriteLine($"Header signature: 0x{string.Join("",viv.Header.Magic.Select(p => p.ToString("X")))} ({System.Text.Encoding.Latin1.GetString(viv.Header.Magic)})");
        Console.WriteLine($"File size in header: {viv.Header.VivLength} ({((long)viv.Header.VivLength).ByteUnits()})");
        Console.WriteLine($"Calculated file size: {calcFileSize} ({((long)calcFileSize).ByteUnits()})");
        Console.WriteLine($"Actual file size: {vivFile.Length} ({vivFile.Length.ByteUnits()})");
        Console.WriteLine($"Files: {viv.Header.Entries}");
        Console.WriteLine($"Data pool offset: {viv.Header.PoolOffset}");
    }

    private static async Task LsCommand(FileInfo vivFile, bool sizeOpt, bool offsetOpt, bool humanOpt, bool decOpt)
    {
        ISerializer<VivFileHeader> parser = new VivHeaderSerializer();
        using var fs = vivFile.OpenRead();
        var viv = await parser.DeserializeAsync(fs);
        foreach (var j in viv.Entries)
        {
            Console.WriteLine(string.Join("\t", ((string?[])[j.Key, sizeOpt ? (humanOpt ? ((long)j.Value.Length).ByteUnits(): j.Value.Length.ToString()) : null, offsetOpt ? (decOpt ? j.Value.Offset.ToString(): $"0x{j.Value.Offset:X8}") : null]).NotEmpty()));
        }
    }

    /// <inheritdoc/>
    public override Command GetCommand()
    {
        var cmd = new Command("viv", "Performs operations on VIV files.");
        var file = new Argument<FileInfo>("viv file", "Path to the VIV file.").ExistingOnly();
        cmd.AddArgument(file);
        cmd.AddCommand(BuildInfoCommand(file));
        cmd.AddCommand(BuildLsCommand(file));
        foreach (var j in BuildReadCommands(file)) cmd.AddCommand(j);
        cmd.AddCommand(BuildAddCommand(file));
        return cmd;
    }
}