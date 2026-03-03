using System;
using TheXDS.MCART.Resources.Strings;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Component;
using static TheXDS.MCART.Resources.Strings.Composition;

namespace TheXDS.Vivianne.Resources;

/// <summary>
/// Includes a collection of common dialog option sets to present to the user.
/// </summary>
public static class DialogOptions
{
    private static void Noop() { }

    private static void CopyTextToClipboard(string text)
    {
        PlatformServices.OperatingSystem.WriteClipboardText(text);
    }

    /// <summary>
    /// Gets a simple dialog action that does nothing.
    /// </summary>
    public static NamedObject<Action> Ok { get; } = new NamedObject<Action>("Ok", Noop);

    /// <summary>
    /// Gets a simple dialog action that does nothing.
    /// </summary>
    public static NamedObject<Action> Close { get; } = new NamedObject<Action>("Close", Noop);

    /// <summary>
    /// Gets a command that copies the specified text to the clipboard.
    /// </summary>
    /// <param name="textToCopy">
    /// Text to copy to the system's clipboard.
    /// </param>
    /// <returns>
    /// A <see cref="NamedObject{T}"/> that returns an action that copies the
    /// specified text to the clipboard.
    /// </returns>
    public static NamedObject<Action> CopyToClipboard(string textToCopy) => new("Copy to clipboard", () => CopyTextToClipboard(textToCopy));

    /// <summary>
    /// Gets an option set for a dialog that allows the user to copy the
    /// details on an exception to the clipboard.
    /// </summary>
    /// <param name="ex">
    /// <see cref="Exception"/> that can be copied to the clipboard.
    /// </param>
    /// <param name="options">Options for exception message dumping.</param>
    /// <returns>
    /// An array of <see cref="NamedObject{T}"/> with actions that can be
    /// invoked from a dialog.
    /// </returns>
    public static NamedObject<Action>[] CopyExToClipboard(Exception ex, ExDumpOptions options = ExDumpOptions.Message) => [
        Ok,
        new("Copy details to clipboard", () => CopyTextToClipboard(ExDump(ex, options)))
    ];

    /// <summary>
    /// Creates an array containing actions to confirm an operation and copy
    /// the specified text to the clipboard.
    /// </summary>
    /// <param name="text">
    /// The text to be copied to the clipboard when the corresponding action is
    /// invoked. Can be null or empty.
    /// </param>
    /// <returns>
    /// An array of named actions. The first action represents confirmation
    /// ("Ok"), and the second action copies the specified text to the
    /// clipboard.
    /// </returns>
    public static NamedObject<Action>[] OkAndCopyText(string text) => [
        Ok,
        CopyToClipboard(text)
    ];
}
