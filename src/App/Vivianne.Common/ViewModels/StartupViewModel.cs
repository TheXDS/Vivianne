using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Types;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Tools;
using St = TheXDS.Vivianne.Resources.Strings.Views.StartupView;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements the startup page logic.
/// </summary>
public class StartupViewModel : ViewModel
{
    private IEnumerable<VivInfo> recentFiles = [];

    /// <summary>
    /// Gets a list of recent files that can be quickly opened from the UI.
    /// </summary>
    public IEnumerable<VivInfo> RecentFiles
    {
        get => recentFiles;
        private set => Change(ref recentFiles, value);
    }

    /// <summary>
    /// Gets a collection of custom tools that can be launched from the startup
    /// view.
    /// </summary>
    public ICollection<ButtonInteraction> ExtraTools { get; } = [];

    /// <summary>
    /// Gets a reference to the command used to create a new, blank VIV file.
    /// </summary>
    public ICommand NewVivCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to open an existing VIV file.
    /// </summary>
    public ICommand OpenVivCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupViewModel"/> class.
    /// </summary>
    public StartupViewModel()
    {
        Title = St.VmTitle;
        var cb = CommandBuilder.For(this);
        NewVivCommand = cb.BuildSimple(OnNewViv);
        OpenVivCommand = cb.BuildSimple(OnOpenViv);
        foreach (var x in ReflectionHelpers.FindAllObjects<IVivianneTool>())
        {
            ExtraTools.Add(new(cb.BuildSimple(() => x.Run(DialogService!, NavigationService!)), x.ToolName));
        }
    }

    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        await Settings.Load();
        RecentFiles = Settings.Current.RecentFiles;
    }

    private void OnNewViv()
    {
        NavigationService!.NavigateAndReset<VivMainViewModel, VivMainState>(new());
    }

    private async Task OnOpenViv(object? parameter)
    {
        string? filePath = null;
        List<VivInfo> l = RecentFiles.ToList();
        if (parameter is VivInfo viv)
        {
            if (!System.IO.File.Exists(viv.FilePath))
            {
                await (DialogService?.Error("File not found.", "The file you selected does not exist or it's inaccessible.") ?? Task.CompletedTask);
                return;
            }
            l.Remove(viv);
            filePath = viv.FilePath;
        }
        else
        {
            var f = await DialogService!.GetFileOpenPath(St.OpenMessage, FileFilters.VivFileFilter);
            if (f.Success)
            {
                filePath = f.Result;
            }
        }
        if (filePath is null) return;
        VivMainState s = await DialogService!.RunOperation(_ => VivMainState.From(filePath));
        l = new VivInfo[] { s }.Concat(l).Take(10).ToList();
        Settings.Current.RecentFiles = [.. l];
        await Settings.Save();
        NavigationService!.NavigateAndReset<VivMainViewModel, VivMainState>(s);
    }
}
