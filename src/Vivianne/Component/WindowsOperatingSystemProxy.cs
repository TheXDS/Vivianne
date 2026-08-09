using System.Windows.Forms;
using System.Diagnostics;
using TheXDS.Vivianne.Component.Application;

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

    public async Task<int> InvokeCommand(string command, string[] args, bool elevate)
    {
        var process = Process.Start(new ProcessStartInfo(command, args) { UseShellExecute = true, Verb = elevate ? "runas" : string.Empty });
        await (process?.WaitForExitAsync() ?? Task.CompletedTask);
        return process?.ExitCode ?? -1;
    }

    public void GoWindowless()
    {
        System.Windows.Application.Current.MainWindow = null;
    }
}
