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
    /// <param name="humanSize">
    /// Optional parameter. If omitted or set to <see langword="true"/>, the
    /// size of objects will be expressed in human-readable format, otherwise
    /// the size of the entity in bytes will be displayed directly.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> array of descriptive properties for the
    /// specified entity.
    /// </returns>
    string[] GetInfo(T entity, bool humanSize = true);
}