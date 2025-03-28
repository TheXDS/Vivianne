using System.Windows;

namespace Vivianne;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
        InitializeComponent();
        if (OperatingSystem.IsWindowsVersionAtLeast(10, build: 22000))
        {
            WindowBackdropType = Wpf.Ui.Controls.WindowBackdropType.Acrylic;
        }
        else if (OperatingSystem.IsWindowsVersionAtLeast(10, build: 19041))
        {
            WindowBackdropType = Wpf.Ui.Controls.WindowBackdropType.Auto;
        }
    }
}