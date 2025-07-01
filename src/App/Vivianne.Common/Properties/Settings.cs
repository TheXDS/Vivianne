using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Configuration;
using TheXDS.Vivianne.Models;
using TheXDS.MCART.Resources.Strings;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Models.Fe;

namespace TheXDS.Vivianne.Properties;

/// <summary>
/// Defines a set of values for a struct that exposes configuration for
/// Vivianne.
/// </summary>
public class Settings
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
    public static async Task Load()
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
    public static Task Save()
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
    /// If enabled, runs a serial number check before saving a VIV file if any
    /// FeData file exists.
    /// </summary>
    public bool Viv_CheckSnOnSave { get; set; }

    /// <summary>
    /// If enabled, runs cleanup tasks on an FCE file before saving a VIV file.
    /// </summary>
    public bool Fce_CleanupOnSave { get; set; }

    /// <summary>
    /// Gets or sets the number of recent files to be recalled by Vivianne.
    /// </summary>
    public int RecentFilesCount { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the info panel will be opened by
    /// default.
    /// </summary>
    public bool Bnk_InfoOpenByDefault { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the default volume normalization
    /// for the normalization tool in the BNK editor.
    /// </summary>
    public double Bnk_DefaultNormalization { get; set; }

    /// <summary>
    /// When enabled, forces the BNK editor to keep the trash that might exist
    /// outside of sample data in BNK files.
    /// </summary>
    public bool Bnk_KeepTrash { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the cleanup tool should also
    /// trim any loops of audio to just the looping sections.
    /// </summary>
    public bool Bnk_TrimLoopsOnCleanup { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether data sync to FeData files
    /// is enabled when editing a Carp file.
    /// </summary>
    public bool Carp_SyncChanges { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the FCE model will be centered
    /// automatically when saving an FCE file.
    /// </summary>
    public bool Fce_CenterModel { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the FCE editor will enable the
    /// car shadow toggle by default.
    /// </summary>
    public bool Fce_ShadowByDefault { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether all compatible images in
    /// the data store will be listed as textures or if only <c>.tga</c> files
    /// will be listed.
    /// </summary>
    public bool Fce_EnumerateAllImages { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether data sync is enabled
    /// between FeData files and carp when editing a FeData file.
    /// </summary>
    public bool Fe_SyncChanges { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the file sorting order for the VIV
    /// directory.
    /// </summary>
    public SortType Viv_FileSorting { get; set; }

    /// <summary>
    /// Adds a recent VIV file to the recent files collection.
    /// </summary>
    /// <param name="file">File name to add.</param>
    public Task AddRecentVivFile(RecentFileInfo file)
    {
        RecentVivFiles = RecentFilesCount > 0 ? [file, .. (RecentVivFiles?.Where(p => p.FilePath != file.FilePath) ?? []).Take(RecentFilesCount - 1)] : [];
        return Save();
    }
}
