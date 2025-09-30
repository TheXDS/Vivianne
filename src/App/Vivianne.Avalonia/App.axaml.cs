using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TheXDS.Vivianne.Component;

namespace Vivianne.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        //PlatformServices.SetKeyboardProxy(new WpfKeyboardProxy());
        //PlatformServices.SetOperatingSystemProxy(new WpfOperatingSystemProxy());
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}