namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes methods
/// to load and save entities from and to <see cref="Stream"/> objects as well
/// as byte arrays.
/// </summary>
/// <typeparam name="T">Type of entity to read/write.</typeparam>
public interface ISerializer<T> : IOutSerializer<T>, IInSerializer<T>
{
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
}
