﻿using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;

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
    public static DialogTemplate OpenAs { get; } = CommonDialogTemplates.FileOpen with
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
}
