using System;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.MCART.Resources.Strings;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Component;
using static TheXDS.MCART.Resources.Strings.Composition;

namespace TheXDS.Vivianne.Resources;

/// <summary>
/// Contains a collection of common file filters and dialog templates to be
/// used by Vivianne.
/// </summary>
public static class Dialogs
{
    /// <summary>
    /// Gets a dialog template for opening files allowing the user to select
    /// how to open the file.
    /// </summary>
    public static DialogTemplate OpenAs => CommonDialogTemplates.FileOpen with
    {
        Title = "Open as",
        Text = "Select how you want Vivianne to open this file as",
    };

    /// <summary>
    /// Gets a dialog template for getting an index to insert an item in a
    /// collection.
    /// </summary>
    public static DialogTemplate GetIndex => CommonDialogTemplates.Input with
    {
        Title = "Insert element",
        Text = "Please specify the insertion index for the new element"
    };

    /// <summary>
    /// Gets a dialog templated for getting a value to be added to a collection
    /// from the user.
    /// </summary>
    public static DialogTemplate GetValue => CommonDialogTemplates.Input with
    {
        Title = "Input value",
        Text = "Please specify a value to be added."
    };

    /// <summary>
    /// Gets a dialog template for displaying an error message when a file
    /// cannot be opened due to it being corrupt or otherwise unreadable.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static DialogTemplate CorruptFileError(string file) => CommonDialogTemplates.Error with
    {
        Title = $"Could not open {file}",
        Text = "The file might be damaged or corrupt; or may use a format not currently understood by Vivianne."
    };
}

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
