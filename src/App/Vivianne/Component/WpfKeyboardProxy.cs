using System.Windows.Input;
using TheXDS.Vivianne.Component;

internal class WpfKeyboardProxy : IKeyboardProxy
{
    public bool IsShiftKeyDown => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
    public bool IsAltKeyDown => Keyboard.IsKeyDown(Key.LeftAlt);
    public bool IsCtrlKeyDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
}
