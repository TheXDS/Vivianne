using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Helpers;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the current _state of the main ViewModel used to manage VIV
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
        set
        {
            if (Change(ref _Viv, value))
            {
                Directory = new(_Viv.Directory);
            };
        }
    }

    /// <summary>
    /// Gets a reference to an observable collection with the VIV directory.
    /// </summary>
    public ObservableDictionaryWrap<string, byte[]> Directory { get; private set; }

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
    public VivMainState() : this(new())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VivMainState"/> class.
    /// </summary>
    /// <param name="viv">Viv file to represent in this _state.</param>
    public VivMainState(VivFile viv)
    {
        _Viv = viv;
        UiThread.Invoke(() => Directory = new(_Viv.Directory));
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
        ISerializer<VivFile> parser = new VivSerializer();
        return new(await parser.DeserializeAsync(fs))
        {
            UnsavedChanges = false,
            FilePath = path,
            FriendlyName = Path.GetFileName(Path.GetDirectoryName(path)) ?? Path.GetFileName(path)
        };
    }
}
