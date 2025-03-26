using System.Threading.Tasks;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Defines a set of members to be implemented by a type that provides of file
/// storage services, be it a physical device with a filesystem or a virtual
/// named data store, such as VIV files.
/// </summary>
public interface IStorageBackend
{
    /// <summary>
    /// Reads the contents of the specified file name.
    /// </summary>
    /// <param name="path">
    /// File name or path to read. The actual implementation may or may not
    /// support hirearchical file names or directories.
    /// </param>
    /// <returns>
    /// A Task that can be used to await reading the file contents.
    /// </returns>
    Task<byte[]> Read(string path);

    /// <summary>
    /// Writes the data to the specified file name.
    /// </summary>
    /// <param name="path">
    /// File name or path to write the data to. The actual implementation may
    /// or may not support hirearchical file names or directories.
    /// </param>
    /// <param name="data">Raw data to be written.</param>
    /// <returns>
    /// A Task that can be used to await the async operation.
    /// </returns>
    Task Write(string path, byte[] data);
}