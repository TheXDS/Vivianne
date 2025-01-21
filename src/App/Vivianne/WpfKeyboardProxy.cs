using System.Windows.Input;
using TheXDS.Vivianne.Component;

internal class WpfKeyboardProxy : IKeyboardProxy
{
    public bool IsShiftKeyDown => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
}