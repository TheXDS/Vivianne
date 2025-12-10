using System.Windows.Controls;

#if !DEBUG
using System.IO;
using TheXDS.Vivianne.ViewModels;
#endif

namespace TheXDS.Vivianne.Views;

/// <summary>
/// Business logic for StartupView.xaml
/// </summary>
public partial class StartupView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupView"/> class.
    /// </summary>
    public StartupView()
    {
        InitializeComponent();
#if !DEBUG && EnableStartupWarning
        Loaded += StartupView_Loaded;
    }

    private static volatile bool WasWarningShown = false;

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
        if (rs is null) return null;
        using var sr = new StreamReader(rs);
        return vm.DialogService?.Warning("Very early alpha application!", sr.ReadToEnd());
#endif
    }
}
