using TheXDS.Vivianne.Properties;

namespace TheXDS.Vivianne.Models;

public class SettingsState : EditorViewModelStateBase
{
    private string? _nfs3Path;
    private bool _autoBackup;
    private bool _vivCheckSnOnSave;
    private bool _fceCleanupOnSave;
    private string? _nfs3LaunchArgs;

    public string? Nfs3Path
    {
        get => _nfs3Path;
        set => Change(ref _nfs3Path, value);
    }

    public bool AutoBackup
    {
        get => _autoBackup;
        set => Change(ref _autoBackup, value);
    }

    public bool VivCheckSnOnSave
    {
        get => _vivCheckSnOnSave;
        set => Change(ref _vivCheckSnOnSave, value);
    }

    public bool FceCleanupOnSave
    {
        get => _fceCleanupOnSave;
        set => Change(ref _fceCleanupOnSave, value);
    }

    public string? Nfs3LaunchArgs
    {
        get => _nfs3LaunchArgs;
        set => Change(ref _nfs3LaunchArgs, value);
    }

    public static implicit operator SettingsState(Settings settings)
    {
        return new SettingsState
        {
            Nfs3Path = settings.Nfs3Path,
            AutoBackup = settings.AutoBackup,
            VivCheckSnOnSave = settings.VivCheckSnOnSave,
            FceCleanupOnSave = settings.FceCleanupOnSave,
            Nfs3LaunchArgs = settings.Nfs3LaunchArgs,
        };
    }
    public static implicit operator Settings(SettingsState settings)
    {
        return new Settings
        {
            Nfs3Path = settings.Nfs3Path,
            AutoBackup = settings.AutoBackup,
            VivCheckSnOnSave = settings.VivCheckSnOnSave,
            FceCleanupOnSave = settings.FceCleanupOnSave,
            Nfs3LaunchArgs = settings.Nfs3LaunchArgs,
        };
    }
}
