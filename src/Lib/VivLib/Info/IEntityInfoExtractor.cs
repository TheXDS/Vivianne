namespace TheXDS.Vivianne.Info;

/// <summary>
/// Defines a set of members to be implemented by a type that can extract
/// descriptive information from an entity.
/// </summary>
/// <typeparam name="T">Type of entity to get information from.</typeparam>
public interface IEntityInfoExtractor<in T>
{
    /// <summary>
    /// Returns an array of strings that describe the properties of the
    /// specified entity.
    /// </summary>
    /// <param name="entity">Type of entity to get the properties for.</param>
    /// <returns>
    /// A <see cref="string"/> array of descriptive properties for the
    /// specified entity.
    /// </returns>
    string[] GetInfo(T entity);
}