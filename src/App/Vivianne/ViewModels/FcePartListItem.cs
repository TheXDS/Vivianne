﻿using System.Collections.Generic;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Represents an item on a list that allows the user to show/hide FCE parts for the render tree.
/// </summary>
/// <param name="part">Key/Value pair with the part and part name to be referenced by this instance.</param>
public class FcePartListItem(KeyValuePair<string, FcePart> part) : NotifyPropertyChanged
{
    private bool _IsVisible;

    /// <summary>
    /// Gets a reference to the part associated with this instance.
    /// </summary>
    public FcePart Part { get; } = part.Value;

    /// <summary>
    /// Gets the name to be displayed for the part referenced by this instance.
    /// </summary>
    public string PartName { get; } = part.Key;

    /// <summary>
    /// Gets or sets a value that indicates whether or not the part referenced by this instance should be rendered.
    /// </summary>
    public bool IsVisible
    {
        get => _IsVisible;
        set => Change(ref _IsVisible, value);
    }
}
