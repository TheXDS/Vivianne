using System.Windows;
using System.Windows.Controls;
using TheXDS.Vivianne.Models;
using static TheXDS.MCART.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements a control that allows the user to edit a
/// <see cref="MutableFceColor"/>.
/// </summary>
public class HsbColorEditor : Control
{
    /// <summary>
    /// Identifies the <see cref="ColorProperty"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorProperty;

    /// <summary>
    /// Initializes the <see cref="HsbColorEditor"/> class.
    /// </summary>
    static HsbColorEditor()
    {
        SetControlStyle<HsbColorEditor>(DefaultStyleKeyProperty);
        ColorProperty = NewDp<MutableFceColor, HsbColorEditor>(nameof(Color), FrameworkPropertyMetadataOptions.AffectsRender);
    }

    /// <summary>
    /// Gets or sets the reference to the <see cref="MutableFceColor"/> to edit
    /// using this control.
    /// </summary>
    public MutableFceColor? Color
    {
        get => (MutableFceColor?)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}
