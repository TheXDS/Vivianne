namespace TheXDS.Vivianne.Models.Fce;

/// <summary>
/// Represents a single FCE dummy with a name and a position.
/// </summary>
public class FceDummy
{
    /// <summary>
    /// Gets or sets the name of the FCE Dummy.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the position of the FCE Dummy.
    /// </summary>
    public Vector3d Position { get; set; }
}