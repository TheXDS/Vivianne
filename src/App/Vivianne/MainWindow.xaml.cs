using System.Windows.Media;

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
        if (OperatingSystem.IsWindowsVersionAtLeast(10, build: 26100))
        {
            WindowBackdropType = Wpf.Ui.Controls.WindowBackdropType.Acrylic;
        }
        else
        {
            Background = new SolidColorBrush(Color.FromArgb(0xff, 0x40, 0x40, 0x40));
            if (OperatingSystem.IsWindowsVersionAtLeast(10, build: 19041))
            {
                WindowBackdropType = Wpf.Ui.Controls.WindowBackdropType.Auto;
            }
        }
    }
}