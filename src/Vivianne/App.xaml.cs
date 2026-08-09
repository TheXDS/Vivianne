using Microsoft.Win32;
using System.Windows;
using TheXDS.Vivianne.Data;
using TheXDS.Vivianne.Component.Application;

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
        Initialization.InitServices(new(
            new WpfKeyboardProxy(),
            new WindowsOperatingSystemProxy()));

        Startup += App_Startup;
    }

    private async void App_Startup(object sender, StartupEventArgs e)
    {
        RegisterCommandLineStartupCallbacks();
        await Initialization.RunInitialOperations();
    }

    private void RegisterCommandLineStartupCallbacks()
    {
        CommandLineStartup.Handlers.Add(Guid.Parse("a8d0e6c8-2410-460c-ab29-7682c351a313"), RegisterFileTypes);
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
