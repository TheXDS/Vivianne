using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheXDS.Vivianne.Views;

public partial class StartupView : UserControl
{
    public StartupView()
    {
        //InitializeComponent();
        AvaloniaXamlLoader.Load(this);
    }
}