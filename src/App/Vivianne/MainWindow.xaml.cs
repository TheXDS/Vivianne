using System.Windows;
using TheXDS.Vivianne.Views;
using Wpf.Ui.Controls;

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
    }
}