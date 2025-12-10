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
