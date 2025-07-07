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
            WindowBackdropType = Wpf.Ui.Controls.WindowBackdropType.Mica;
        }
        else if (OperatingSystem.IsWindowsVersionAtLeast(10, build: 18362))
        {
            WindowBackdropType = Wpf.Ui.Controls.WindowBackdropType.Acrylic;
        }
        else
        {
            Background = Brushes.Gray;
        }
    }


    private int currentTheme;

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Wpf.Ui.Appearance.ApplicationThemeManager.Apply((Wpf.Ui.Appearance.ApplicationTheme)((currentTheme = 1 - currentTheme) + 1));
    }
}