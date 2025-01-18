using System.Windows;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Services;
using TheXDS.Vivianne.Component;

namespace Vivianne
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            UiThread.SetProxy(new DispatcherUiThreadProxy());
            KeyboardProxy.SetProxy(new WpfKeyboardProxy());
        }
    }
}
internal class WpfKeyboardProxy : IKeyboardProxy
{
    public bool IsShiftKeyDown => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
}