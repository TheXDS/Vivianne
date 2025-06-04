namespace TheXDS.Vivianne.Models;

/// <summary>
/// Defines a set of members to be implemented by a type that provides vehicle 
/// car class information.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICarClass<T> where T : unmanaged, Enum
{
    /// <summary>
    /// Gets or sets the car class to which this car belongs to.
    /// </summary>
    T CarClass { get; set; }
}
