using System.Windows.Input;

namespace TheXDS.Vivianne.Component
{
    internal class WpfKeyboardProxy : IKeyboardProxy
    {
        public bool IsShiftKeyDown => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
    }
}
