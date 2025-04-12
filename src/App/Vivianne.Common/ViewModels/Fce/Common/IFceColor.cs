using System.Collections.Generic;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Defines a set of members to be implemented by a type that represents a
/// complete color combination as defined in an FCE file.
/// </summary>
/// <typeparam name="TColor">
/// Underlying color structure for each color element.
/// </typeparam>
public interface IFceColor<TColor> : INameable where TColor : IHsbColor
{
    /// <summary>
    /// Enumerates all color elements that conform this color combination.
    /// </summary>
    IEnumerable<TColor> Colors { get; }
}
