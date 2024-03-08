namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes methods
/// to load and save entities from and to <see cref="Stream"/> objects.
/// </summary>
/// <typeparam name="T">Type of entity to read/write.</typeparam>
public interface ISerializer<T>
{
    /// <summary>
    /// Reads an entity of type <typeparamref name="T"/> from the stream.
    /// </summary>
    /// <param name="stream">Stream to read the entity from.</param>
    /// <returns>
    /// A new isntance of type <typeparamref name="T"/> loaded from the
    /// <see cref="Stream"/>.
    /// </returns>
    T Deserialize(Stream stream);

    /// <summary>
    /// Reads an entity of type <typeparamref name="T"/> from the raw bytes.
    /// </summary>
    /// <param name="content">
    /// Raw content from which to parse the entity.
    /// </param>
    /// <returns>
    /// A new instance of type <typeparamref name="T"/> loaded from the raw
    /// bytes.
    /// </returns>
    T Deserialize(byte[] content) => Deserialize(new MemoryStream(content));

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
    /// Reads an entity of type <typeparamref name="T"/> from the stream
    /// asyncronously.
    /// </summary>
    /// <param name="stream">Stream to read the entity from.</param>
    /// <returns>
    /// A new instance of type <typeparamref name="T"/> loaded from the
    /// <see cref="Stream"/>.
    /// </returns>
    Task<T> DeserializeAsync(Stream stream) => Task.Run(() => Deserialize(stream));

    /// <summary>
    /// Reads an entity of type <typeparamref name="T"/> from the stream
    /// asyncronously.
    /// </summary>
    /// <param name="rawContent">Byte array to read the entity from.</param>
    /// <returns>
    /// A new instance of type <typeparamref name="T"/> loaded from the byte
    /// array.
    /// </returns>
    Task<T> DeserializeAsync(byte[] rawContent) => Task.Run(() => Deserialize(rawContent));

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
