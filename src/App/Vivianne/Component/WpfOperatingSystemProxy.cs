using TheXDS.Vivianne.Component;
using System.Windows.Forms;

internal class WpfOperatingSystemProxy : IOperatingSystemProxy
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
}
