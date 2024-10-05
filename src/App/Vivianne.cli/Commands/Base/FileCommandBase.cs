using System.CommandLine;
using System.Reflection;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Serializers;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Commands.Base;

/// <summary>
/// Base class for all commands that receive a file as its primary argument.
/// </summary>
/// <param name="alias">Alias of the command.</param>
/// <param name="help">Description of the command.</param>
/// <param name="fileArgName">Alias of the file argument.</param>
/// <param name="fileArgHelp">Description of the file argument.</param>
/// <param name="internalMethods">
/// Collection of internal methods to search for sub-command definition methods
/// to be invoked.
/// </param>
public abstract class FileCommandBase<TFile, TSerializer>(
    string alias,
    string help,
    string fileArgName,
    string fileArgHelp,
    IEnumerable<MethodInfo> internalMethods)
    : VivianneCommand where TSerializer : ISerializer<TFile>, new()
{
    private readonly string _alias = alias;
    private readonly string _help = help;
    private readonly string _fileArgName = fileArgName;
    private readonly string _fileArgHelp = fileArgHelp;
    private readonly IEnumerable<MethodInfo> _commands = internalMethods;

    /// <inheritdoc/>
    public override Command GetCommand()
    {
        var cmd = new Command(_alias, _help);
        var file = new Argument<FileInfo>(_fileArgName, _fileArgHelp).ExistingOnly();
        cmd.AddArgument(file);
        foreach (var j in _commands.WithSignature<Func<Argument<FileInfo>, Command>>().Select(p => p.Invoke(file))) cmd.AddCommand(j);
        return cmd;
    }

    /// <summary>
    /// Executes a file transaction.
    /// </summary>
    /// <param name="file">File to be read/written.</param>
    /// <param name="action">
    /// Action to execute on the deserialized contents of the file.
    /// </param>
    /// <param name="readOnly">
    /// If set to <see langword="true"/>, the file transaction will be
    /// read-only.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that can be used to <see langword="await"/> the
    /// <see langword="async"/> operation.
    /// </returns>
    protected static async Task FileTransaction(FileInfo file, Func<TFile, Task> action, bool readOnly = false)
    {
        ISerializer<TFile> serializer = new TSerializer();
        TFile value;
        try
        {
            using (var fs = file.OpenRead())
            {
                value = await serializer.DeserializeAsync(fs);
            }
            await action.Invoke(value);
            if (!readOnly)
            {
                using var fs = file.OpenWrite();
                fs.Destroy();
                await serializer.SerializeToAsync(value, fs);
            }
        }
        catch (Exception ex)
        {
            Fail(ex.Message);
        }
    }

    /// <summary>
    /// Executes a file transaction.
    /// </summary>
    /// <param name="file">File to be read/written.</param>
    /// <param name="action">
    /// Action to execute on the deserialized contents of the file.
    /// </param>
    /// <param name="readOnly">
    /// If set to <see langword="true"/>, the file transaction will be
    /// read-only.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that can be used to <see langword="await"/> the
    /// <see langword="async"/> operation.
    /// </returns>
    protected static Task FileTransaction(FileInfo file, Action<TFile> action, bool readOnly = false)
    {
        return FileTransaction(file, f => Task.Run(() => action.Invoke(f)), readOnly);
    }
}
