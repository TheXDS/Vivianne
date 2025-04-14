using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Defines a set of members to be implemented by a type that provides of file
/// storage services.
/// </summary>
/// <remarks>
/// This interface differs from <see cref="IBackingStore{T}"/> in that this
/// interface sits one level below in the storage stack, providing of physical
/// raw data storage services and platform specific dialog services to get
/// filenames according to the type of underlying physical storage media.
/// </remarks>
public interface IBackingStore
{
    /// <summary>
    /// Reads the contents of a file asyncronously.
    /// </summary>
    /// <param name="fileName">Name of the file to read.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that will return the contents of the file
    /// as a <c><see cref="byte"/>[]</c> array upon completion.
    /// </returns>
    Task<byte[]?> ReadAsync(string fileName);

    /// <summary>
    /// Writes the contents to the specified name asyncronously.
    /// </summary>
    /// <param name="fileName">Name of the file to write.</param>
    /// <param name="content">
    /// Contents to be written to the specified file.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that will return a value indicating
    /// success on the write operation.
    /// </returns>
    /// <remarks>
    /// The old file contents will be overriden with
    /// <paramref name="content"/>.
    /// </remarks>
    Task<bool> WriteAsync(string fileName, byte[] content);

    /// <summary>
    /// Creates a prompt for the user to provide a file name to use when
    /// storing data on this backing store.
    /// </summary>
    /// <param name="oldFileName">
    /// Filename to suggest as the default file name. Can be set to
    /// <see langword="null"/> for new, unnamed files.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that returns a dialog result with either
    /// the acompanying file name selected by the user or
    /// <see langword="null"/> if the user cancelled the dialog.
    /// </returns>
    Task<DialogResult<string?>> GetNewFileName(string? oldFileName);

    /// <summary>
    /// Enumerates all files on the store.
    /// </summary>
    /// <returns>An enumeration of all files in the backing store.</returns>
    IEnumerable<string> EnumerateFiles();

    /// <summary>
    /// Exposes the backing store as a dictionary.
    /// </summary>
    /// <returns>
    /// A new dictionary that allows access to the files accesible through the
    /// backing store.
    /// </returns>
    IDictionary<string, byte[]> AsDictionary();
}
