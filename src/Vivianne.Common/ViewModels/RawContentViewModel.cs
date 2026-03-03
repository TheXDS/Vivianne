using System.Text;
using System.Windows.Input;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Component;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Base class for all ViewModels that reference the raw contents of a file.
/// </summary>
public class RawContentViewModel : ViewModel
{
    /// <summary>
    /// Gets a reference to the raw contents of a file.
    /// </summary>
    public byte[] RawFile { get; }

    /// <summary>
    /// Gets a reference to the command used to copy the raw file contents
    /// to the clipboard.
    /// </summary>
    public ICommand CopyCommand { get; }

    /// <param name="rawFile">Raw contents of a file.</param>
    public RawContentViewModel(byte[] rawFile)
    {
        RawFile = rawFile;
        CopyCommand = new SimpleCommand(OnCopy);
    }

    private void OnCopy()
    {
        PlatformServices.OperatingSystem.WriteClipboardText(Encoding.Latin1.GetString(RawFile));
    }
}