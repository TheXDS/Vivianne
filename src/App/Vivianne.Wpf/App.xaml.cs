using System.Windows;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Services;

namespace Vivianne.Wpf
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
        }
    }
}
