using System.Numerics;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Represents a single FCE dummy with a name and a position.
/// </summary>
public class FceDummy : INameable
{
    /// <summary>
    /// Gets or sets the name of the FCE Dummy.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the position of the FCE Dummy.
    /// </summary>
    public Vector3 Position { get; set; }
}