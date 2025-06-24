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
using TheXDS.Vivianne.ViewModels.Asf;
using TheXDS.Vivianne.ViewModels.Base;
using TheXDS.Vivianne.ViewModels.Fce;
using TheXDS.Vivianne.ViewModels.Fsh;
using TheXDS.Vivianne.ViewModels.Viv;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.StartupViewModel;
using Stc = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements the startup page logic.
/// </summary>
public class StartupViewModel : ViewModel
{
    private static readonly IEnumerable<Func<StartupViewModel, Task?>> _InitActions = [
        TryOpenFileFromCmdArgs,
        vm => (SearchForNfsProcess() is { } proc) ? vm.WaitForNfsProcess(proc) : null,
    ];

    private static Task? TryOpenFileFromCmdArgs(StartupViewModel vm)
    {
        if (Environment.GetCommandLineArgs().ElementAtOrDefault(1) is not string file || file.IsEmpty()) return null;
        var extension = Path.GetExtension(file);
        return vm.Launchers.OfType<IFileEditorViewModelLauncher>().FirstOrDefault(p => p.CanOpen(extension))?.OnOpen(file);
    }

    private readonly IEnumerable<IViewModelLauncher> _Launchers;
    private bool _isNfsRunning;

    /// <summary>
    /// Gets a reference to the command used to open the settings page.
    /// </summary>
    public ICommand SettingsCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to launch NFS3.
    /// </summary>
    public ICommand LaunchNfs3Command { get; }

    /// <summary>
    /// Gets a reference to the command used to launch NFS4.
    /// </summary>
    public ICommand LaunchNfs4Command { get; }

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
    /// Gets a value that indicates if Need For Speed is running.
    /// </summary>
    public bool IsNfsRunning
    {
        get => _isNfsRunning;
        private set => Change(ref _isNfsRunning, value);
    }

    /// <summary>
    /// Gets a reference to the Need For Speed process when it's running.
    /// </summary>
    public Process? NfsProcess { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupViewModel"/> class.
    /// </summary>
    public StartupViewModel()
    {
        _Launchers = [
            new VivFileEditorLauncher(() => DialogService!) { IsActive = true },
            new FshFileEditorLauncher(() => DialogService!),
            new FceFileEditorLauncher(() => DialogService!),
            new MusPlayerViewModelLauncher(),
            new ExtraToolsViewModelLauncher()
        ];
        Title = St.StartupPage;
        var cb = CommandBuilder.For(this);
        SettingsCommand = cb.BuildSimple(OnSettings);
        LaunchNfs3Command = cb.BuildSimple(OnLaunchNfs3);
        LaunchNfs4Command = cb.BuildSimple(OnLaunchNfs4);
        TerminateProcessCommand = cb.BuildSimple(proc => (proc as Process)?.Kill());
    }

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        base.OnInitialize(broadcastSetup);
        broadcastSetup.RegisterPropertyChangeBroadcast(() => IsNfsRunning, () => NfsProcess);
    }

    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        foreach (var j in Launchers)
        {
            j.DialogService ??= DialogService;
            j.NavigationService ??= NavigationService;
        }

        if (IsInitialized) return;
        await Settings.Load();
        foreach (var initAction in _InitActions)
        {
            await (initAction.Invoke(this) ?? Task.CompletedTask);
        }
    }

    private Task OnSettings()
    {
        return DialogService!.Show<SettingsViewModel>(new Ganymede.Models.DialogTemplate() { Title = St.Settings });
    }

    private Task OnLaunchNfs3()
    {
        return OnLaunchNfsProcess(Settings.Current.Nfs3Path, "nfs3.exe", Settings.Current.Nfs3LaunchArgs, "Need For Speed 3");
    }

    private Task OnLaunchNfs4()
    {
        return OnLaunchNfsProcess(Settings.Current.Nfs4Path, "nfs4.exe", Settings.Current.Nfs4LaunchArgs, "Need For Speed 4");
    }

    private async Task OnLaunchNfsProcess(string? processPath, string exeName, string? args, string processName)
    {
        var errTemplate = CommonDialogTemplates.Error with { Title = $"Could not launch {processName}" };
        try
        {
            if (processPath is not null)
            {
                var procStartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(processPath, exeName),
                    Arguments = args ?? string.Empty,
                    UseShellExecute = true,
                    WorkingDirectory = processPath,
                };
                var proc = Process.Start(procStartInfo);
                if (proc is null)
                {
                    await (DialogService?.Show(errTemplate with { Text = "Could not start process" }) ?? Task.CompletedTask);
                    return;
                }
                await (WaitForNfsProcess(proc));
            }
            else if (DialogService is { } dlgSvc)
            {
                await (await dlgSvc.Show(errTemplate with { Text = St.YouHavenTConfiguredTheGamePath },
                [
                    (St.LaunchSettings, OnSettings),
                    (Stc.Ok, () => Task.CompletedTask)
                ])).Invoke();
            }
        }
        catch (Exception ex)
        {
            await (DialogService?.Show(errTemplate with { Text = ex.Message }) ?? Task.CompletedTask);
        }
    }

    private async Task WaitForNfsProcess(Process proc)
    {
        NfsProcess = proc;
        IsNfsRunning = true;
        await proc.WaitForExitAsync();
        NfsProcess.Dispose();
        NfsProcess = null;
        IsNfsRunning = false;
    }

    private static Process? SearchForNfsProcess()
    {
        return Process.GetProcessesByName("nfs3").Concat(Process.GetProcessesByName("nfs4")).FirstOrDefault();
    }
}
