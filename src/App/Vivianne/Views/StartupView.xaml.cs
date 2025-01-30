using System.IO;
using System.Windows.Controls;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.Views;

/// <summary>
/// Business logic for StartupView.xaml
/// </summary>
public partial class StartupView : UserControl
{
    private static volatile bool WasWarningShown = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupView"/> class.
    /// </summary>
    public StartupView()
    {
        InitializeComponent();
#if !DEBUG
        Loaded += StartupView_Loaded;
#endif
    }
#if !DEBUG
    private void StartupView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (WasWarningShown) return;
        if (DataContext is StartupViewModel vm) DisplayEarlyAlphaWarning(vm);
        WasWarningShown = true;
        Loaded -= StartupView_Loaded;
    }

    private static Task? DisplayEarlyAlphaWarning(StartupViewModel vm)
    {
        using var rs = typeof(StartupView).Assembly.GetManifestResourceStream("TheXDS.Vivianne.Resources.Embedded.EarlyAlphaNote.txt");
        using var sr = new StreamReader(rs);
        return vm.DialogService?.Warning("Very early alpha application!", sr.ReadToEnd());
    }
#endif
}
