using TheXDS.MCART.Types.Base;

namespace TheXDS.Vivianne.Models.Geo;

/// <summary>
/// Represents an item on a list that allows the user to show/hide GEO parts
/// for the render tree.
/// </summary>
/// <param name="part">Part referenced by this instance.</param>
public class GeoPartListItem(GeoPart part) : NotifyPropertyChanged
{
    private bool _IsVisible;

    /// <summary>
    /// Gets a reference to the part associated with this instance.
    /// </summary>
    public GeoPart Part { get; } = part;

    /// <summary>
    /// Gets or sets a value that indicates whether or not the part referenced
    /// by this instance should be rendered.
    /// </summary>
    public bool IsVisible
    {
        get => _IsVisible;
        set => Change(ref _IsVisible, value);
    }
}
