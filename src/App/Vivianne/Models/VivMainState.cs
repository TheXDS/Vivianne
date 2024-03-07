using System.IO;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Containers;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the current state of the main ViewModel used to manage VIV
/// files.
/// </summary>
public class VivMainState : VivInfo
{
    private bool _unsavedChanges;
    private VivFile _Viv;

    /// <summary>
    /// Gets or sets a reference to the actual VIV file contents.
    /// </summary>
    public VivFile Viv
    {
        get => _Viv;
        set => Change(ref _Viv, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates if the loaded VIV file contains
    /// unsaved changes.
    /// </summary>
    public bool UnsavedChanges
    {
        get => _unsavedChanges;
        set => Change(ref _unsavedChanges, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VivMainState"/> class.
    /// </summary>
    public VivMainState()
    {
        _Viv = new();
        _unsavedChanges = false;
    }

    /// <summary>
    /// Creates a new <see cref="VivMainState"/> instance, loading the viv file
    /// from the specified path.
    /// </summary>
    /// <param name="path">Path to load the VIV file from.</param>
    /// <returns>
    /// A <see cref="Task"/> that returns a new <see cref="VivMainState"/>.
    /// </returns>
    public static async Task<VivMainState> From(string path)
    {
        await using var fs = File.OpenRead(path);
        return new()
        {
            Viv = await Task.Run(() => VivFile.ReadFrom(fs)),
            UnsavedChanges = false,
            FilePath = path,
            FriendlyName = Path.GetFileName(Path.GetDirectoryName(path)) ?? Path.GetFileName(path)
        };
    }
}

public class CarpGeneratorState : NotifyPropertyChanged
{
    private double _Mass;
    private double _TopSpeed;
    private int _MinRpm;
    private int _MaxRpm;

    public double Mass
    {
        get => _Mass;
        set => Change(ref _Mass, value);
    }

    public double TopSpeed
    {
        get => _TopSpeed;
        set => Change(ref _TopSpeed, value);
    }

    public int MinRpm
    {
        get => _MinRpm;
        set => Change(ref _MinRpm, value);
    }

    public int MaxRpm
    {
        get => _MaxRpm;
        set => Change(ref _MaxRpm, value);
    }

}