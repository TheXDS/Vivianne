namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes methods
/// that deserialize entities from a <see cref="Stream"/> or a byte array.
/// </summary>
/// <typeparam name="T">Covariant. Type of entity to deserialize.</typeparam>
public interface IOutSerializer<out T>
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
}
