using TheXDS.Ganymede.Types.Base;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Base class for all ViewModels that reference the raw contents of a file.
/// </summary>
/// <param name="rawFile">Raw contents of a file.</param>
public class RawContentViewModel(byte[] rawFile) : ViewModel
{
    /// <summary>
    /// Gets a reference to the raw contents of a file.
    /// </summary>
    public byte[] RawFile { get; } = rawFile;
}