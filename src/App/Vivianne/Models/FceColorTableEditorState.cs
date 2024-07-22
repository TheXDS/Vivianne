using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the state of the <see cref="FceColorEditorViewModel"/>.
/// </summary>
/// <param name="fce">FCE whose colors will be edited.</param>
public class FceColorTableEditorState(FceFile fce) : EditorViewModelStateBase
{
    /// <summary>
    /// Gets a reference to the FCE file whose color table is being edited.
    /// </summary>
    public FceFile Fce { get; } = fce;

    /// <summary>
    /// Gets the collection of colors that are being edited on the FCE file.
    /// </summary>
    public ICollection<FceColorItem> Colors { get; } = CreateFromFce(fce);

    private static ObservableCollection<FceColorItem> CreateFromFce(FceFile fce)
    {
        var primary = fce.Header.PrimaryColorTable
            .Take(fce.Header.PrimaryColors)
            .Select(MutableFceColor.From);
        var secondary = fce.Header.SecondaryColorTable
            .Take(fce.Header.SecondaryColors)
            .Select(MutableFceColor.From);
        var joint = primary
            .Zip(secondary)
            .Select(p => new FceColorItem(p.First, p.Second));
        return new ObservableCollection<FceColorItem>(joint);
    }
}
