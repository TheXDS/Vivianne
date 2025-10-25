using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TheXDS.Vivianne.Views.Specialized;

/// <summary>
/// Interaction logic for ComingSoonView.xaml
/// </summary>
public partial class ComingSoonView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComingSoonView"/> class.
    /// </summary>
    public ComingSoonView()
    {
        InitializeComponent();
    }

    private void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo(((Hyperlink)sender).NavigateUri.ToString()) { UseShellExecute = true });
    }
}
