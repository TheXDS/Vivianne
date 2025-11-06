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

    private static NamedObject<Action> Ok { get; } = new NamedObject<Action>("Ok", Noop);

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
        new("Copy details to clipboard", () => PlatformServices.OperatingSystem.WriteClipboardText(ExDump(ex, options)))
    ];
}
