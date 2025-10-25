using System.Linq;
using System.Reflection;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.ViewModels.Fce.Nfs4;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents the state of the <see cref="Fce4ColorEditorViewModel"/>.
/// </summary>
/// <param name="fce">FCE whose colors will be edited.</param>
public class Fce4ColorTableEditorState(Fce4EditorState fce) : EditorViewModelStateBase
{
    /// <summary>
    /// Gets a reference to the FCE file whose color table is being edited.
    /// </summary>
    public Fce4EditorState Fce { get; } = fce;

    /// <summary>
    /// Gets the collection of colors that are being edited on the FCE file.
    /// </summary>
    public ObservableListWrap<MutableFceColorItem> Colors { get; } = CreateFromFce(fce.File);

    private static ObservableListWrap<MutableFceColorItem> CreateFromFce(FceFile fce)
    {
        var primary = fce.PrimaryColors;
        var interior = fce.InteriorColors;
        var secondary = fce.SecondaryColors;
        var driverHair = fce.DriverHairColors;
        var joint = primary.Zip(interior, secondary).Zip(driverHair).Select(p => new MutableFceColorItem(MutableFceColor.From(p.First.First), MutableFceColor.From(p.First.Second), MutableFceColor.From(p.First.Third), MutableFceColor.From(p.Second))).ToList();
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
        color.InteriorColor.Subscribe(OnColorChanged);
        color.SecondaryColor.Subscribe(OnColorChanged);
        color.DriverHairColor.Subscribe(OnColorChanged);
    }
}
