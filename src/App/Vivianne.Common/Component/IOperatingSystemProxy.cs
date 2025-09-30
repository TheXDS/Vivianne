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
}
