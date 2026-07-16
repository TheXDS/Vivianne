using System.Threading.Tasks;

namespace TheXDS.Vivianne.Component.Application;

/// <summary>
/// Implements a dummy, null operating system proxy for when the proxy is not
/// registered on the target platform.
/// </summary>
public class NullOperatingSystemProxy : IOperatingSystemProxy
{
    bool IOperatingSystemProxy.IsElevated => false;

    string IOperatingSystemProxy.ReadClipboardText() => string.Empty;

    void IOperatingSystemProxy.WriteClipboardText(string text) { }

    void IOperatingSystemProxy.ShowNativeErrorBox(string text, string caption)
    {
        System.Diagnostics.Debug.WriteLine($"{caption}: {text}");
    }

    Task<int> IOperatingSystemProxy.InvokeCommand(string command, string[] args, bool elevate) => Task.FromResult(0);

    void IOperatingSystemProxy.GoWindowless() { }
}
