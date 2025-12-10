using System.Threading.Tasks;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes operating
/// system related information.
/// </summary>
public interface IOperatingSystemProxy
{
    /// <summary>
    /// Gets a value that indicates whether the current process is running with
    /// elevated privileges.
    /// </summary>
    bool IsElevated { get; }

    /// <summary>
    /// Gets the current text content of the clipboard.
    /// </summary>
    /// <returns>The current text content of the clipboard.</returns>
    string ReadClipboardText();

    /// <summary>
    /// Sets the current text content of the clipboard.
    /// </summary>
    /// <param name="text">Text to set the current clipboard to.</param>
    void WriteClipboardText(string text);

    /// <summary>
    /// Shows a native error message box that can be used before App/Ganymede
    /// initialization.
    /// </summary>
    /// <param name="text">Text on the native message box.</param>
    /// <param name="caption">Title of the native message box.</param>
    void ShowNativeErrorBox(string text, string caption);

    /// <summary>
    /// Invokes a command through the operating system and awaits for the
    /// process completion.
    /// </summary>
    /// <param name="command">Command to execute.</param>
    /// <param name="args">Arguments to pass to the command.</param>
    /// <param name="elevate">
    /// Indicates whether the command should be elevated.
    /// </param>
    /// <returns>
    /// A Task that can be used to await for the completion of the process.
    /// </returns>
    Task InvokeCommand(string command, string[] args, bool elevate = false);

    /// <summary>
    /// Invokes an internal Vivianne callback through the operating system,
    /// requesting elevated permissions as needed.
    /// </summary>
    /// <param name="callback">GUID of the callback to execute.</param>
    /// <param name="args">Arguments to pass to the command.</param>
    /// <returns>
    /// A Task that can be used to await for the completion of the process.
    /// </returns>
    Task InvokeSelfCallback(string callback, params string[] args)
    {
        return System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName is string thisFile
            ? InvokeCommand(thisFile, [$"--Callback-{callback}", ..args], true)
            : Task.CompletedTask;
    }
}
