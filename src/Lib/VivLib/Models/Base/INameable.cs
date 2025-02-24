namespace TheXDS.Vivianne.Models.Base;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes a name property.
/// </summary>
public interface INameable
{
    /// <summary>
    /// Gets or sets the name of this element.
    /// </summary>
    string Name { get; set; }
}
