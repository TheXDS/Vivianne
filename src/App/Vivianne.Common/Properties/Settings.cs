using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Configuration;
using TheXDS.Vivianne.Models;
using TheXDS.MCART.Resources.Strings;

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
        string[] sourceArray = ["./settings.development.json", "./settings.debug.json"];
        var _configurationStore = new LocalFileSettingsStore(sourceArray.FirstOrDefault(System.IO.File.Exists) ?? "./settings.json");
#else
        var _configurationStore = new LocalFileSettingsStore("./settings.json");
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
        RecentFilesCount = 10;
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
    /// Gets or sets the path to the NFS3 main directory.
    /// </summary>
    public string? Nfs3Path { get; set; }

    /// <summary>
    /// Gets or sets a string with the command line arguments to pass onto nfs3.exe upon invocation.
    /// </summary>
    public string? Nfs3LaunchArgs { get; set; }

    /// <summary>
    /// If enabled, generates backups whenever a file is saved.
    /// </summary>
    public bool AutoBackup { get; set; }

    /// <summary>
    /// If enabled, runs a serial number check before saving a VIV file if any FeData file exists.
    /// </summary>
    public bool VivCheckSnOnSave { get; set; }

    /// <summary>
    /// If enabled, runs cleanup tasks on an FCE file before saving a VIV file.
    /// </summary>
    public bool FceCleanupOnSave { get; set; }

    /// <summary>
    /// Gets or sets the number of recent files to be recalled by Vivianne.
    /// </summary>
    public int RecentFilesCount { get; set; }
}
