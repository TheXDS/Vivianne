using System.Threading.Tasks;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes keyboard
/// data for modifier keys that can be used to open editors and dialogs in
/// alternate modes or with extra options.
/// </summary>
public interface IKeyboardProxy
{
    /// <summary>
    /// Gets a value that indicates if the <c>Shift</c> key is being held down.
    /// </summary>
    bool IsShiftKeyDown { get; }
}

public interface IStorageBackend
{
    Task<byte[]> Read(string path);

    Task Write(string path, byte[] data);
}