using TheXDS.Vivianne.Component;
using System.Windows.Forms;
using System.Diagnostics;

internal class WindowsOperatingSystemProxy : IOperatingSystemProxy
{
    public bool IsElevated => TheXDS.MCART.Helpers.Windows.IsAdministrator();

    public string ReadClipboardText()
    {
        return Clipboard.GetText();
    }

    public void WriteClipboardText(string text)
    {
        Clipboard.SetText(text);
    }

    public void ShowNativeErrorBox(string text, string caption)
    {
        MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public Task InvokeCommand(string command, string[] args, bool elevate)
    {
        return Process.Start(new ProcessStartInfo(command, args) { UseShellExecute = true, Verb = elevate ? "runas" : string.Empty })?.WaitForExitAsync() ?? Task.CompletedTask;
    }
}
