using System.Windows;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Services;
using TheXDS.Vivianne.Component;

#if !DEBUG
using System.IO;
using static TheXDS.MCART.Resources.Strings.ExDumpOptions;
using static TheXDS.MCART.Resources.Strings.Composition;
#endif

namespace Vivianne;

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
        PlatformServices.SetKeyboardProxy(new WpfKeyboardProxy());
        //PlatformServices.SetFceRender(new WpfStaticFceRender());

#if !DEBUG
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        File.WriteAllText(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),$"VivianneError_{DateTime.UtcNow:yyyy-MM-dd_hh-mm-ss}.txt"), 
            ExDump((Exception)e.ExceptionObject, All));

        if (MessageBox.Show("""
            An unhandled exception has occurred in Vivianne.
            
            An exception dump has been generated onto your desktop.

            If you click 'OK' the application will be terminated inmediately. If you see a 'Cancel' button the exception might be recoverable, but be aware that Vivianne might be unstable. Save your work if possible and please, submit a bug report in Vivianne's repo at https://github.com/TheXDS/Vivianne
            """, "Whoah, what happened there?", e.IsTerminating ? MessageBoxButton.OK : MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
        {
            Environment.FailFast("Unhandled exception", (Exception)e.ExceptionObject);
        }
#endif
    }
}
