using INameableBase = TheXDS.MCART.Types.Base.INameable;

namespace TheXDS.Vivianne.Models.Base;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes a name property.
/// </summary>
public interface INameable : INameableBase
{
    /// <summary>
    /// Gets or sets the name of this element.
    /// </summary>
    new string Name { get; set; }

    string INameableBase.Name => Name;
}
