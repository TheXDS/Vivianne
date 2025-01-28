using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.Views.StartupView;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements the startup page logic.
/// </summary>
public class StartupViewModel : ViewModel
{
    private static bool _initialized = false;
    private static readonly IEnumerable<Func<StartupViewModel, Task?>> _InitActions = [
        TryOpenFileFromCmdArgs,
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
    private static Task? TryOpenFileFromCmdArgs(StartupViewModel vm)
    {
        if (Environment.GetCommandLineArgs().ElementAtOrDefault(1) is not string file || file.IsEmpty()) return null;
        var extension = Path.GetExtension(file);
        return vm.Launchers.OfType<IFileEditorViewModelLauncher>().FirstOrDefault(p => p.CanOpen(extension))?.OnOpen(file);
    }

    private readonly IEnumerable<IViewModelLauncher> _Launchers = [
        new VivFileEditorLauncher(),
        new FshFileEditorLauncher(),
        new ExtraToolsViewModelLauncher()
        ];
    private bool _isNfs3Running;

    /// <summary>
    /// Gets a reference to the command used to open the settings page.
    /// </summary>
    public ICommand SettingsCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to launch NFS3.
    /// </summary>
    public ICommand LaunchNfs3Command { get; }

    /// <summary>
    /// Gets a reference to the command used to forcefully end the game
    /// process if it can't be exited normally.
    /// </summary>
    public ICommand TerminateProcessCommand { get; }

    /// <summary>
    /// Enumerates all available file editor launchers.
    /// </summary>
    public IEnumerable<IViewModelLauncher> Launchers => _Launchers;

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
        SettingsCommand = cb.BuildSimple(OnSettings);
        LaunchNfs3Command = cb.BuildSimple(OnLaunchNfs3);
        TerminateProcessCommand = cb.BuildSimple(proc => (proc as Process)?.Kill());
    }

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        base.OnInitialize(broadcastSetup);
        broadcastSetup.RegisterPropertyChangeBroadcast(() => IsNfs3Running, () => Nfs3Process);
    }

    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        foreach (var j in Launchers)
        {
            j.DialogService ??= DialogService;
            j.NavigationService ??= NavigationService;
        }

        if (_initialized) return;
        _initialized = true;
        await Settings.Load();
        foreach (var initAction in _InitActions)
        {
            await (initAction.Invoke(this) ?? Task.CompletedTask);
        }
        await base.OnCreated();
    }

    private Task OnSettings()
    {
        return DialogService!.Show<SettingsViewModel>(new Ganymede.Models.DialogTemplate() { Title = "Settings" });
    }

    private async Task OnLaunchNfs3()
    {
        try
        {
            if (Settings.Current.Nfs3Path is not null)
            {
                await (WaitForNfs3Process(Process.Start(Path.Combine(Settings.Current.Nfs3Path, "nfs3.exe"), Settings.Current.Nfs3LaunchArgs ?? string.Empty)));
            }
            else
            {
                await (DialogService?.SelectAction(CommonDialogTemplates.Error with
                {
                    Title = "Could not launch NFS3",
                    Text = "You haven't configured the game path."
                }, [new(OnSettings, "Launch settings"), new(() => Task.CompletedTask, "Ok")]) ?? Task.CompletedTask);
            }
        }
        catch (Exception ex)
        {
            await (DialogService?.Error("Could not launch NFS3", ex.Message) ?? Task.CompletedTask);
        }
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
        return Process.GetProcessesByName("nfs3").FirstOrDefault();
    }
}
