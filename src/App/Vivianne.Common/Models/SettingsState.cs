using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Properties;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the current state of the settings editor view model.
/// </summary>
public class SettingsState : EditorViewModelStateBase
{
    private string? _nfs3Path;
    private bool _autoBackup;
    private bool _vivCheckSnOnSave;
    private bool _fceCleanupOnSave;
    private string? _nfs3LaunchArgs;
    private int _recentFilesCount;
    private bool _bnk_InfoOpenByDefault;
    private bool _fce_CenterModel;
    private SortType _viv_FileSorting;
    private bool _fce_ShadowByDefault;


    /// <summary>
    /// Gets or sets the path to the NFS3 main directory.
    /// </summary>
    public string? Nfs3Path
    {
        get => _nfs3Path;
        set => Change(ref _nfs3Path, value);
    }

    /// <summary>
    /// If enabled, generates backups whenever a file is saved.
    /// </summary>
    public bool AutoBackup
    {
        get => _autoBackup;
        set => Change(ref _autoBackup, value);
    }

    /// <summary>
    /// If enabled, runs a serial number check before saving a VIV file if any FeData file exists.
    /// </summary>
    public bool VivCheckSnOnSave
    {
        get => _vivCheckSnOnSave;
        set => Change(ref _vivCheckSnOnSave, value);
    }

    /// <summary>
    /// If enabled, runs cleanup tasks on an FCE file before saving a VIV file.
    /// </summary>
    public bool FceCleanupOnSave
    {
        get => _fceCleanupOnSave;
        set => Change(ref _fceCleanupOnSave, value);
    }

    /// <summary>
    /// Gets or sets a string with the command line arguments to pass onto nfs3.exe upon invocation.
    /// </summary>
    public string? Nfs3LaunchArgs
    {
        get => _nfs3LaunchArgs;
        set => Change(ref _nfs3LaunchArgs, value);
    }

    /// <summary>
    /// Gets or sets the number of recent files to be recalled by Vivianne.
    /// </summary>
    public int RecentFilesCount
    {
        get => _recentFilesCount;
        set => Change(ref _recentFilesCount, value);
    }

    /// <summary>
    /// gets or sets a value that indicates if the info panel will be opened by default.
    /// </summary>
    public bool Bnk_InfoOpenByDefault
    { 
        get => _bnk_InfoOpenByDefault;
        set => Change(ref _bnk_InfoOpenByDefault, value);
    }

    /// <summary>
    /// gets or sets a value that indicates if the FCE model will be centered automatically when saving an FCE file.
    /// </summary>
    public bool Fce_CenterModel
    { 
        get => _fce_CenterModel;
        set => Change(ref _fce_CenterModel, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates if the FCE editor will enable the
    /// car shadow toggle by default.
    /// </summary>
    public bool Fce_ShadowByDefault
    { 
        get => _fce_ShadowByDefault;
        set => Change(ref _fce_ShadowByDefault, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the file sorting order for the VIV directory.
    /// </summary>
    public SortType Viv_FileSorting
    { 
        get => _viv_FileSorting;
        set => Change(ref _viv_FileSorting, value);
    }

    /// <summary>
    /// Implicitly converts an object of type <see cref="Settings"/> into an
    /// object of type <see cref="SettingsState"/>.
    /// </summary>
    /// <param name="settings">Object to be converted.</param>
    public static implicit operator SettingsState(Settings settings)
    {
        return new SettingsState
        {
            Nfs3Path = settings.Nfs3Path,
            AutoBackup = settings.AutoBackup,
            VivCheckSnOnSave = settings.VivCheckSnOnSave,
            FceCleanupOnSave = settings.FceCleanupOnSave,
            Nfs3LaunchArgs = settings.Nfs3LaunchArgs,
            RecentFilesCount = settings.RecentFilesCount,
            Bnk_InfoOpenByDefault = settings.Bnk_InfoOpenByDefault,
            Fce_CenterModel = settings.Fce_CenterModel,
            Viv_FileSorting = settings.Viv_FileSorting,
        };
    }

    /// <summary>
    /// Implicitly converts an object of type <see cref="SettingsState"/> into
    /// an object of type <see cref="Settings"/>.
    /// </summary>
    /// <param name="settings">Object to be converted.</param>
    public static implicit operator Settings(SettingsState settings)
    {
        return new Settings
        {
            Nfs3Path = settings.Nfs3Path,
            AutoBackup = settings.AutoBackup,
            VivCheckSnOnSave = settings.VivCheckSnOnSave,
            FceCleanupOnSave = settings.FceCleanupOnSave,
            Nfs3LaunchArgs = settings.Nfs3LaunchArgs,
            RecentFilesCount = settings.RecentFilesCount,
            Bnk_InfoOpenByDefault = settings.Bnk_InfoOpenByDefault,
            Fce_CenterModel = settings.Fce_CenterModel,
            Viv_FileSorting = settings.Viv_FileSorting,
        };
    }
}
