using System.Windows.Controls;
using static TheXDS.MCART.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements a simple control that can be used as the background of an editor
/// view.
/// </summary>
public class EditorBackdrop : Control
{
    static EditorBackdrop()
    {
        SetControlStyle<EditorBackdrop>(DefaultStyleKeyProperty);
    }
}
