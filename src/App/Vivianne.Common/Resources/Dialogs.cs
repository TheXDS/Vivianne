using TheXDS.Ganymede.Models;
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
}
