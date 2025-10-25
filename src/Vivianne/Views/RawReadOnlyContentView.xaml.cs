using System.Windows;
using System.Windows.Controls;

namespace TheXDS.Vivianne.Views;

/// <summary>
/// Interaction logic for RawReadOnlyContentView.xaml
/// </summary>
public partial class RawReadOnlyContentView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RawReadOnlyContentView"/>
    /// class.
    /// </summary>
    public RawReadOnlyContentView()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(TxtRawContents.Text);
    }
}
