using Microsoft.Win32;
using System.Windows;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Data;


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
        PlatformServices.SetKeyboardProxy(new WpfKeyboardProxy());
        PlatformServices.SetOperatingSystemProxy(new WindowsOperatingSystemProxy());
        Startup += App_Startup;

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

    private async void App_Startup(object sender, StartupEventArgs e)
    {
        await Settings.LoadAsync();
        RegisterCommandLineStartupCallbacks();
        ProcessSpecialCommandLineStartupCallbacks(e.Args);
    }

    private static void ProcessSpecialCommandLineStartupCallbacks(string[] args)
    {
        if (args.Length >= 1 && args[0].StartsWith("--Callback-") && CommandLineStartup.Handlers.TryGetValue(Guid.Parse(args[0][11..]), out var handler))
        {
            Current.MainWindow = null;
            handler.Invoke(args);
            Environment.Exit(0);
        }
    }

    private void RegisterCommandLineStartupCallbacks()
    {
        CommandLineStartup.Handlers.Add(Guid.Parse("a8d0e6c8-2410-460c-ab29-7682c351a313"), RegisterFileTypes);
    }

    private readonly record struct FileTypeInfo(
        string[] FileExtensions,
        string ProgId,
        string FileDescription,
        bool IsPrimary = true,
        int IconIndex = 0)
    {
    }

    private void RegisterFileTypes(string[] obj)
    {
        CommandLineStartup.FailIfNotElevated();

        foreach (var j in FileTypes.KnownFileTypes.Where(p => p.IsPrimary))
        {
            foreach (var k in j.FileExtensions)
            {
                using RegistryKey? key = Registry.ClassesRoot.CreateSubKey(k);
                key?.SetValue("", j.ProgId);
            }
            using RegistryKey? subKey = Registry.ClassesRoot.CreateSubKey(j.ProgId);
            subKey?.SetValue("", j.FileDescription);
            using RegistryKey? iconKey = subKey?.CreateSubKey("DefaultIcon");
            iconKey?.SetValue("", $"\"{System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName}\",{j.IconIndex}");
            using RegistryKey? commandKey = subKey?.CreateSubKey(@"Shell\Open\Command");
            commandKey?.SetValue("", $"\"{System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName}\" \"%1\"");
        }
    }
}
