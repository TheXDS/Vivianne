using System.Threading.Tasks;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Implements a dummy, null operating system proxy for when the proxy is not
/// registered on the target platform.
/// </summary>
public class NullOperatingSystemProxy : IOperatingSystemProxy
{
    /// <inheritdoc/>
    public bool IsElevated => false;

    /// <inheritdoc/>
    public string ReadClipboardText()
    {
        return string.Empty;
    }

    /// <inheritdoc/>
    public void WriteClipboardText(string text)
    {
    }

    /// <inheritdoc/>
    public void ShowNativeErrorBox(string text, string caption)
    {
        System.Diagnostics.Debug.WriteLine($"{caption}: {text}");
    }

    /// <inheritdoc/>
    public Task InvokeCommand(string command, string[] args, bool elevate)
    {
        return Task.CompletedTask;
    }
}
