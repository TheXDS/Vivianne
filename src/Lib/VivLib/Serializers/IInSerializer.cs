namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes methods
/// that serialize entities to a <see cref="Stream"/> or a byte array.
/// </summary>
/// <typeparam name="T">Contravariant. Type of entity to serialize.</typeparam>
public interface IInSerializer<in T>
{
    /// <summary>
    /// Writes the specified entity to the <see cref="Stream"/>.
    /// </summary>
    /// <param name="entity">Entity to write.</param>
    /// <param name="stream">Stream to write the entity to.</param>
    void SerializeTo(T entity, Stream stream);

    /// <summary>
    /// Serializes the specified entity into a new byte array.
    /// </summary>
    /// <param name="entity">Entity to write.</param>
    byte[] Serialize(T entity)
    {
        using MemoryStream ms = new();
        SerializeTo(entity, ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Writes the specified entity to the <see cref="Stream"/> asyncronously.
    /// </summary>
    /// <param name="entity">Entity to write.</param>
    /// <returns>
    /// A task that can be used to await the async operation.
    /// </returns>
    async Task<byte[]> SerializeAsync(T entity)
    {
        using MemoryStream ms = new();
        await SerializeToAsync(entity, ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Writes the specified entity to the <see cref="Stream"/> asyncronously.
    /// </summary>
    /// <param name="entity">Entity to write.</param>
    /// <param name="stream">Stream to write the entity to.</param>
    /// <returns>
    /// A task that can be used to await the async operation.
    /// </returns>
    Task SerializeToAsync(T entity, Stream stream) => Task.Run(() => SerializeTo(entity, stream));
}
