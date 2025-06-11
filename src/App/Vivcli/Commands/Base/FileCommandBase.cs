using System.CommandLine;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Commands.Base;

/// <summary>
/// Base class for all commands that receive a file as its primary argument.
/// </summary>
/// <param name="alias">Alias of the command.</param>
/// <param name="help">Description of the command.</param>
/// <param name="fileArgName">Alias of the file argument.</param>
/// <param name="fileArgHelp">Description of the file argument.</param>
/// <param name="exposedCommands">
/// Collection of internal methods to search for sub-command definition methods
/// to be invoked.
/// </param>
public abstract class FileCommandBase(
    string alias,
    string help,
    string fileArgName,
    string fileArgHelp,
    IEnumerable<Func<Argument<FileInfo>, Command>> exposedCommands) : VivianneCommand
{
    private readonly string _alias = alias;
    private readonly string _help = help;
    private readonly string _fileArgName = fileArgName;
    private readonly string _fileArgHelp = fileArgHelp;
    private readonly IEnumerable<Func<Argument<FileInfo>, Command>> _commands = exposedCommands;

    /// <inheritdoc/>
    public override Command GetCommand()
    {
        var cmd = new Command(_alias, _help);
        var file = new Argument<FileInfo>(_fileArgName, _fileArgHelp).ExistingOnly();
        cmd.AddArgument(file);
        foreach (var j in _commands.Select(p => p.Invoke(file))) cmd.AddCommand(j);
        return cmd;
    }

    /// <summary>
    /// Executes a file transaction.
    /// </summary>
    /// <param name="file">File to be read/written.</param>
    /// <param name="action">
    /// Action to execute on the deserialized contents of the file.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that can be used to <see langword="await"/> the
    /// <see langword="async"/> operation.
    /// </returns>
    protected static Task FileTransaction<TFile, TSerializer>(FileInfo file, Action<TFile> action)
        where TSerializer : ISerializer<TFile>, new()
    {
        return FileTransaction<TFile, TSerializer>(file, f => Task.Run(() => action.Invoke(f)));
    }

    /// <summary>
    /// Executes a read/write file transaction.
    /// </summary>
    /// <param name="file">File to be read/written.</param>
    /// <param name="action">
    /// Action to execute on the deserialized contents of the file.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that can be used to <see langword="await"/> the
    /// <see langword="async"/> operation.
    /// </returns>
    protected static async Task FileTransaction<TFile, TSerializer>(FileInfo file, Func<TFile, Task> action)
        where TSerializer : ISerializer<TFile>, new()
    {
        ISerializer<TFile> serializer = new TSerializer();
        TFile value = default!;
        try
        {
            using var fs = file.OpenRead();
            value = serializer.Deserialize(fs);
        }
        catch
        {
            Fail("The file you specified was either corrupt or invalid.");
        }
        try
        {
            await action.Invoke(value);
            using var outFs = file.OpenWrite();
            outFs.Destroy();
            await serializer.SerializeToAsync(value, outFs);
        }
        catch (Exception ex)
        {
            Fail(ex.Message);
        }
    }

    /// <summary>
    /// Executes a read-only file transaction.
    /// </summary>
    /// <param name="file">File to be read.</param>
    /// <param name="action">
    /// Action to execute on the deserialized contents of the file.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that can be used to <see langword="await"/> the
    /// <see langword="async"/> operation.
    /// </returns>
    protected static Task ReadOnlyFileTransaction<TFile, TSerializer>(FileInfo file, Action<TFile> action)
        where TSerializer : IOutSerializer<TFile>, new()
    {
        return ReadOnlyFileTransaction<TFile, TSerializer>(file, f => Task.Run(() => action.Invoke(f)));
    }

    /// <summary>
    /// Executes a read-only file transaction.
    /// </summary>
    /// <param name="file">File to be read.</param>
    /// <param name="action">
    /// Action to execute on the deserialized contents of the file.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that can be used to <see langword="await"/> the
    /// <see langword="async"/> operation.
    /// </returns>
    protected static async Task ReadOnlyFileTransaction<TFile, TSerializer>(FileInfo file, Func<TFile, Task> action)
        where TSerializer : IOutSerializer<TFile>, new()
    {
        IOutSerializer<TFile> serializer = new TSerializer();
        TFile value = default!;
        try
        {
            using var fs = file.OpenRead();
            value = serializer.Deserialize(fs);
        }
        catch
        {
            Fail("The file you specified was either corrupt or invalid.");
        }
        try
        {
            await action.Invoke(value);
        }
        catch (Exception ex)
        {
            Fail(ex.Message);
        }
    }
}
