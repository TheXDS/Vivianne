using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
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
        var primary = fce.Header.PrimaryColorTable
            .Take(fce.Header.PrimaryColors)
            .Select(MutableFceColor.From);
        var secondary = fce.Header.SecondaryColorTable
            .Take(fce.Header.SecondaryColors)
            .Select(MutableFceColor.From);
        var joint = primary
            .Zip(secondary)
            .Select(p => new MutableFceColorItem(p.First, p.Second)).ToList();
        var obsc = new ObservableListWrap<MutableFceColorItem>(joint);
        foreach (var item in joint)
        {
            HookItemRefresh(obsc, item);

            void OnColorChanged(object instance, PropertyInfo property, PropertyChangeNotificationType notificationType) => obsc.RefreshItem(item);
            item.PrimaryColor.Subscribe(OnColorChanged);
            item.SecondaryColor.Subscribe(OnColorChanged);
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
