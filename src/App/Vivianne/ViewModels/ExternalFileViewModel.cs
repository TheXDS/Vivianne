using System;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// ViewModel that instructs the operating system to open a file, waits for the
/// process to end and optionally modifies the file inside the VIV.
/// </summary>
/// <param name="rawFile">Raw file contents.</param>
/// <param name="saveCallback">
/// Save callback to execute in case the file was changed externally.
/// </param>
public class ExternalFileViewModel(byte[] rawFile, Action<byte[]> saveCallback) : RawContentViewModel(rawFile)
{
    protected readonly Action<byte[]> _saveCallback = saveCallback;
}
