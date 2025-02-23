using System.Linq;
using System.Reflection;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;
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
    public ObservableListWrap<MutableFceColorItem> Colors { get; } = CreateFromFce(fce);

    private static ObservableListWrap<MutableFceColorItem> CreateFromFce(FceFile fce)
    {
        var primary = fce.PrimaryColors;
        var secondary = fce.SecondaryColors.Count > 0 ? fce.SecondaryColors.ToArray().Wrapping(16) : primary;
        var joint = primary.Zip(secondary).Select(p => new MutableFceColorItem(MutableFceColor.From(p.First), MutableFceColor.From(p.Second))).ToList();
        var obsc = new ObservableListWrap<MutableFceColorItem>(joint);
        foreach (var item in joint)
        {
            HookItemRefresh(obsc, item);
        }
        return obsc;
    }

    /// <summary>
    /// Adds a new color to the color collection.
    /// </summary>
    /// <param name="newColor">Color to be added.</param>
    public void AddColor(MutableFceColorItem newColor)
    {
        Colors.Add(newColor);
        HookItemRefresh(Colors, newColor);
    }

    private static void HookItemRefresh(ObservableListWrap<MutableFceColorItem> colorCollection, MutableFceColorItem color)
    {
        void OnColorChanged(object instance, PropertyInfo property, PropertyChangeNotificationType notificationType) => colorCollection.RefreshItem(color);
        color.PrimaryColor.Subscribe(OnColorChanged);
        color.SecondaryColor.Subscribe(OnColorChanged);
    }
}
