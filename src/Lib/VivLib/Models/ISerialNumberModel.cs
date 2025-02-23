namespace TheXDS.Vivianne.Models;

/// <summary>
/// Defines a set of members to be implemented by a model that contains a Car's
/// serial number, such as FeData or Carp.
/// </summary>
public interface ISerialNumberModel
{
    /// <summary>
    /// Gets or sets the associated car's serial number.
    /// </summary>
    ushort SerialNumber { get; set; }
}
