using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents an item on a list that allows the user to show/hide FCE parts for the render tree.
/// </summary>
/// <param name="part">Key/Value pair with the part and part name to be referenced by this instance.</param>
public class FcePartListItem<TFcePart>(TFcePart part) : NotifyPropertyChanged
{
    private bool _IsVisible;

    /// <summary>
    /// Gets a reference to the part associated with this instance.
    /// </summary>
    public TFcePart Part { get; } = part;

    /// <summary>
    /// Gets or sets a value that indicates whether or not the part referenced by this instance should be rendered.
    /// </summary>
    public bool IsVisible
    {
        get => _IsVisible;
        set => Change(ref _IsVisible, value);
    }

    /// <summary>
    /// Variant of the <see cref="IsVisible"/> property that does not trigger a property change.
    /// </summary>
    public bool IsVisibleNoRedraw
    {
        get => _IsVisible;
        set => _IsVisible = value;
    }
}
