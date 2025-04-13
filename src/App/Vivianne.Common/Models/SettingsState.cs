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
    private double _bnk_DefaultNormalization;
    private bool _bnk_KeepTrash;
    private bool _bnk_TrimLoopsOnCleanup;
    private bool _fce_EnumerateAllImages;
    private string? _nfs4Path;
    private string? _nfs4LaunchArgs;

    /// <summary>
    /// Gets or sets the path to the NFS4 main directory.
    /// </summary>
    public string? Nfs4Path
    { 
        get => _nfs4Path;
        set => Change(ref _nfs4Path, value);
    }

    /// <summary>
    /// Gets or sets a string with the command line arguments to pass onto
    /// <c>nfs4.exe</c> upon invocation.
    /// </summary>
    public string? Nfs4LaunchArgs
    { 
        get => _nfs4LaunchArgs;
        set => Change(ref _nfs4LaunchArgs, value);
    }

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
    public bool Viv_CheckSnOnSave
    {
        get => _vivCheckSnOnSave;
        set => Change(ref _vivCheckSnOnSave, value);
    }

    /// <summary>
    /// If enabled, runs cleanup tasks on an FCE file before saving a VIV file.
    /// </summary>
    public bool Fce_CleanupOnSave
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
    /// Gets or sets a value that indicates the default volume normalization
    /// for the normalization tool in the BNK editor.
    /// </summary>
    public double Bnk_DefaultNormalization
    { 
        get => _bnk_DefaultNormalization;
        set => Change(ref _bnk_DefaultNormalization, value);
    }

    /// <summary>
    /// When enabled, forces the BNK editor to keep the trash that might exist
    /// outside of sample data in BNK files.
    /// </summary>
    public bool Bnk_KeepTrash
    {
        get => _bnk_KeepTrash;
        set => Change(ref _bnk_KeepTrash, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates if the cleanup tool should also
    /// trim any loops of audio to just the looping sections.
    /// </summary>
    public bool Bnk_TrimLoopsOnCleanup
    { 
        get => _bnk_TrimLoopsOnCleanup;
        set => Change(ref _bnk_TrimLoopsOnCleanup, value);
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
    /// Gets or sets a value that indicates whether all compatible images in
    /// the data store will be listed as textures or if only <c>.tga</c> files
    /// will be listed.
    /// </summary>
    public bool Fce_EnumerateAllImages
    { 
        get => _fce_EnumerateAllImages;
        set => Change(ref _fce_EnumerateAllImages, value);
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
            Nfs3LaunchArgs = settings.Nfs3LaunchArgs,
            Nfs4Path = settings.Nfs4Path,
            Nfs4LaunchArgs = settings.Nfs4LaunchArgs,
            AutoBackup = settings.AutoBackup,
            RecentFilesCount = settings.RecentFilesCount,
            Bnk_InfoOpenByDefault = settings.Bnk_InfoOpenByDefault,
            Bnk_DefaultNormalization = settings.Bnk_DefaultNormalization,
            Bnk_KeepTrash = settings.Bnk_KeepTrash,
            Bnk_TrimLoopsOnCleanup = settings.Bnk_TrimLoopsOnCleanup,
            Fce_CleanupOnSave = settings.Fce_CleanupOnSave,
            Fce_CenterModel = settings.Fce_CenterModel,
            Fce_EnumerateAllImages = settings.Fce_EnumerateAllImages,
            Fce_ShadowByDefault = settings.Fce_ShadowByDefault,
            Viv_CheckSnOnSave = settings.Viv_CheckSnOnSave,
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
            Nfs3LaunchArgs = settings.Nfs3LaunchArgs,
            Nfs4Path = settings.Nfs4Path,
            Nfs4LaunchArgs = settings.Nfs4LaunchArgs,
            AutoBackup = settings.AutoBackup,
            Viv_CheckSnOnSave = settings.Viv_CheckSnOnSave,
            RecentFilesCount = settings.RecentFilesCount,
            Bnk_InfoOpenByDefault = settings.Bnk_InfoOpenByDefault,
            Bnk_DefaultNormalization = settings.Bnk_DefaultNormalization,
            Bnk_KeepTrash = settings.Bnk_KeepTrash,
            Bnk_TrimLoopsOnCleanup = settings.Bnk_TrimLoopsOnCleanup,
            Fce_CleanupOnSave = settings.Fce_CleanupOnSave,
            Fce_CenterModel = settings.Fce_CenterModel,
            Fce_EnumerateAllImages = settings.Fce_EnumerateAllImages,
            Fce_ShadowByDefault = settings.Fce_ShadowByDefault,
            Viv_FileSorting = settings.Viv_FileSorting,
        };
    }
}
