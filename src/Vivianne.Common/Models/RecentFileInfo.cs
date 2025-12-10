using TheXDS.MCART.Types.Base;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a blob of information with basic information on a file.
/// </summary>
public class RecentFileInfo : NotifyPropertyChanged
{
    private string? _FilePath;
    private string? _FriendlyName;

    /// <summary>
    /// Gets or sets the file path to the file.
    /// </summary>
    public string? FilePath
    {
        get => _FilePath;
        set => Change(ref _FilePath, value);
    }

    /// <summary>
    /// Gets or sets a friendly name to use to identify the file.
    /// </summary>
    public string? FriendlyName
    {
        get => _FriendlyName;
        set => Change(ref _FriendlyName, value);
    }
}
