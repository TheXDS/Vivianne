using System.Threading.Tasks;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Defines a set of members to be implemented by a type that provides entity
/// storage services.
/// </summary>
/// <typeparam name="T">Type of entity to serialize and store.</typeparam>
/// <remarks>
/// This interface differs from <see cref="IBackingStore"/> in that this
/// interface is one layer above in the storage stack, providing serialization
/// and savestate management services.
/// </remarks>
public interface IBackingStore<T> where T : notnull
{
    /// <summary>
    /// Gets a reference to the underlying raw data store used by this
    /// instance.
    /// </summary>
    IBackingStore Store { get; }

    /// <summary>
    /// Gets or sets the filename to use when reading and writing entities of
    /// type <typeparamref name="T"/>.
    /// </summary>
    string? FileName { get; set; }

    /// <summary>
    /// Reads an entity from the underlying store asyncronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that will return the entity that was read
    /// from the underlying store, or <see langword="null"/> if the read
    /// operation could not be completed.
    /// </returns>
    Task<T?> ReadAsync();

    /// <summary>
    /// Writes an entity to the uderlying store asyncronously.
    /// </summary>
    /// <param name="file">Entity to be written.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that will return <see langword="true"/>
    /// if the write operation was completed successfully, or
    /// <see langword="false"/> otherwise.
    /// </returns>
    Task<bool> WriteAsync(T file);

    /// <summary>
    /// Writes a new copy of the entity to the uderlying store asyncronously
    /// with a new name.
    /// </summary>
    /// <param name="file">Entity to be written.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that will return <see langword="true"/>
    /// if the write operation was completed successfully, or
    /// <see langword="false"/> otherwise.
    /// </returns>
    Task<bool> WriteNewAsync(T file);
}