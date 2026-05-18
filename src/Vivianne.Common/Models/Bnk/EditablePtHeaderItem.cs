using System.Collections.Generic;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Audio.Base;

namespace TheXDS.Vivianne.Models.Bnk;

/// <summary>
/// Represents an editable PT header item, encapsulating a key-value pair
/// for a custom property in the PT header.
/// </summary>
public class EditablePtHeaderItem : NotifyPropertyChanged
{
    private int _value;

    /// <summary>
    /// Gets the key value associated with this instance.
    /// </summary>
    public byte Key { get; init; }

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public int Value
    {
        get => _value;
        set => Change(ref _value, value);
    }

    /// <summary>
    /// Converts this instance into a key-value pair suitable for updating
    /// the PT header dictionaries.
    /// </summary>
    /// <returns>
    /// A key-value pair representing this PT header item.
    /// </returns>
    public KeyValuePair<byte, PtHeaderValue> ToKeyValuePair() => new(Key, Value);
}
