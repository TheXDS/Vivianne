using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Types;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
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
    private static bool _initialized = false;

    private static readonly IEnumerable<Func<StartupViewModel, Task?>> _InitActions = [
        vm => Environment.GetCommandLineArgs().Length > 1 && !Environment.GetCommandLineArgs()[1].IsEmpty() ? vm.OnOpenViv(Environment.GetCommandLineArgs()[1]!) : null,
        #if !DEBUG
        vm => vm.DialogService?.Warning("Very early alpha application!", """
            This copy of Vivianne is a very early version. A lot of features will be either incomplete or unstable. Please do not use Vivianne for any mods you plan to release just yet.

            Also, the UX/UI, feature set and tools are all subject to change.

            This preview is for evaluation purposes only... You've been warned!

            Happy modding.

               -- TheXDS --
            """),
        #endif
        vm => (SearchForNfs3Process() is { } proc) ? vm.WaitForNfs3Process(proc) : null,
    ];

    private IEnumerable<VivInfo> recentVivFiles = [];
    private bool _isNfs3Running;

    /// <summary>
    /// Gets a list of recent files that can be quickly opened from the UI.
    /// </summary>
    public IEnumerable<VivInfo> RecentVivFiles
    {
        get => recentVivFiles;
        private set => Change(ref recentVivFiles, value);
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
    /// Gets a reference to the command used to open the settings page.
    /// </summary>
    public ICommand SettingsCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to launch NFS3.
    /// </summary>
    public ICommand LaunchNfs3Command { get; }

    /// <summary>
    /// Gets a reference to the command used to forcefully end the NFS3
    /// process if the game can't be exited normally.
    /// </summary>
    public ICommand KillNfs3ProcessCommand { get; }

    /// <summary>
    /// Gets a value that indicates if NFS3 is running.
    /// </summary>
    public bool IsNfs3Running
    {
        get => _isNfs3Running;
        private set => Change(ref _isNfs3Running, value);
    }

    /// <summary>
    /// Gets a reference to the NFS3 process when it's running.
    /// </summary>
    public Process? Nfs3Process { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupViewModel"/> class.
    /// </summary>
    public StartupViewModel()
    {
        Title = St.VmTitle;
        var cb = CommandBuilder.For(this);
        NewVivCommand = cb.BuildSimple(OnNewViv);
        OpenVivCommand = cb.BuildSimple(OnOpenViv);
        SettingsCommand = cb.BuildSimple(OnSettings);
        LaunchNfs3Command = cb.BuildSimple(OnLaunchNfs3);
        KillNfs3ProcessCommand = cb.BuildSimple(() => Nfs3Process?.Kill());
        foreach (var x in ReflectionHelpers.FindAllObjects<IVivianneTool>())
        {
            ExtraTools.Add(new(cb.BuildSimple(() => x.Run(DialogService!, NavigationService!)), x.ToolName));
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        broadcastSetup.RegisterPropertyChangeBroadcast(() => IsNfs3Running, () => Nfs3Process);
    }

    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        await Settings.Load();
        RecentVivFiles = Settings.Current.RecentVivFiles;
        if (_initialized) return;
        _initialized = true;
        foreach (var initAction in _InitActions)
        {
            await (initAction.Invoke(this) ?? Task.CompletedTask);
        }
    }

    private void OnNewViv()
    {
        NavigationService!.NavigateAndReset<VivMainViewModel, VivMainState>(new());
    }

    private Task OnSettings()
    {
        return DialogService!.Show<SettingsViewModel>(new Ganymede.Models.DialogTemplate() { Title = "Settings" });
    }

    private async Task OnLaunchNfs3()
    {
        try
        {
            await (WaitForNfs3Process(Process.Start(Path.Combine(Settings.Current.Nfs3Path, "nfs3.exe"), Settings.Current.Nfs3LaunchArgs)));
        }
        catch (Exception ex)
        {
            await (DialogService?.Error("Could not launch NFS3", ex.Message) ?? Task.CompletedTask);
        }
    }

    private async Task OnOpenViv(object? parameter)
    {
        string? filePath = null;
        List<VivInfo> l = RecentVivFiles.ToList();
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
        else if (parameter is string file)
        {
            filePath = file;
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
        Settings.Current.RecentVivFiles = [.. l];
        await Settings.Save();
        NavigationService!.NavigateAndReset<VivMainViewModel, VivMainState>(s);
    }

    private async Task WaitForNfs3Process(Process proc)
    {
        Nfs3Process = proc;
        IsNfs3Running = true;
        await proc.WaitForExitAsync();
        Nfs3Process.Dispose();
        Nfs3Process = null;
        IsNfs3Running = false;
    }

    private static Process? SearchForNfs3Process()
    {
        return Process.GetProcessesByName("nfs3.exe").FirstOrDefault();
    }
}
