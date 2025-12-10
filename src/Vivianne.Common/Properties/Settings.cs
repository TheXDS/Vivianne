using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Configuration;
using TheXDS.MCART.Resources.Strings;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Viv;

namespace TheXDS.Vivianne.Properties;

/// <summary>
/// Defines a set of values for a struct that exposes configuration for
/// Vivianne.
/// </summary>
public partial class Settings
{
    private static readonly IConfigurationRepository<Settings> _repository;

    static Settings()
    {
#if DEBUG
        string localAppData = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/Vivianne/";
        string rootDir = System.IO.Path.GetDirectoryName(Environment.ProcessPath) ?? localAppData;
        string[] sourceArray = [
            System.IO.Path.Combine(rootDir, "settings.development.json"),
            System.IO.Path.Combine(rootDir, "settings.debug.json"),
            System.IO.Path.Combine(rootDir, "settings.json")
            ];
        var _configurationStore = new LocalFileSettingsStore(sourceArray.FirstOrDefault(System.IO.File.Exists) ?? System.IO.Path.Combine(localAppData, "settings.json"));
#else
        var _configurationStore = new LocalFileSettingsStore($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/Vivianne/settings.json");
#endif
        _repository = new JsonConfigurationRepository<Settings>(_configurationStore);
    }

    /// <summary>
    /// Loads the configuration for Vivianne asyncronously.
    /// </summary>
    /// <returns>
    /// A task that can be used to await the async operation.
    /// </returns>
    public static async Task LoadAsync()
    {
        try
        {
            Current = await _repository.Load() ?? new();
        }
        catch (Exception ex)
        {
            Debug.Print(Composition.ExDump(ex, ExDumpOptions.All));
            Current = new();
        }
    }

    /// <summary>
    /// Saves the configuration for Vivianne asyncronously.
    /// </summary>
    /// <returns>
    /// A task that can be used to await the async operation.
    /// </returns>
    public static Task SaveAsync()
    {
        return _repository.Save(Current);
    }

    /// <summary>
    /// Gets a reference to the currently active settings.
    /// </summary>
    public static Settings Current { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/> struct.
    /// </summary>
    public Settings()
    {
        RecentVivFiles = [];
        RecentFshFiles = [];
        RecentFceFiles = [];
        RecentAsfFiles = [];
        RecentBnkFiles = [];
        RecentFilesCount = 10;
        Viv_FileSorting = SortType.FileKind;
        Fce_ShadowByDefault = true;
        Bnk_DefaultNormalization = 1.0;
    }

    /// <summary>
    /// Gets or sets the list of recent VIV files.
    /// </summary>
    public RecentFileInfo[] RecentVivFiles { get; set; }

    /// <summary>
    /// Gets or sets the list of recent FSH/QFS files.
    /// </summary>
    public RecentFileInfo[] RecentFshFiles { get; set; }

    /// <summary>
    /// Gets or sets the list of recent FCE files.
    /// </summary>
    public RecentFileInfo[] RecentFceFiles { get; set; }

    /// <summary>
    /// Gets or sets the list of recent ASF/MUS files.
    /// </summary>
    public RecentFileInfo[] RecentAsfFiles { get; set; }

    /// <summary>
    /// Gets or sets the collection of recently accessed .bnk files.
    /// </summary>
    public RecentFileInfo[] RecentBnkFiles { get; set; }

    /// <summary>
    /// Gets or sets the path to the NFS3 main directory.
    /// </summary>
    public string? Nfs3Path { get; set; }

    /// <summary>
    /// Gets or sets a string with the command line arguments to pass onto
    /// <c>nfs3.exe</c> upon invocation.
    /// </summary>
    public string? Nfs3LaunchArgs { get; set; }

    /// <summary>
    /// Gets or sets the path to the NFS4 main directory.
    /// </summary>
    public string? Nfs4Path { get; set; }

    /// <summary>
    /// Gets or sets a string with the command line arguments to pass onto
    /// <c>nfs4.exe</c> upon invocation.
    /// </summary>
    public string? Nfs4LaunchArgs { get; set; }

    /// <summary>
    /// If enabled, generates backups whenever a file is saved.
    /// </summary>
    public bool AutoBackup { get; set; }

    /// <summary>
    /// Gets or or sets a value that indicates the preferred FeData language to
    /// use for extraction and presentation of data throughout Vivianne.
    /// </summary>
    public FeDataLang PreferredFeDataLang { get; set; }

    /// <summary>
    /// Gets or sets the number of recent files to be recalled by Vivianne.
    /// </summary>
    public int RecentFilesCount { get; set; }
}
