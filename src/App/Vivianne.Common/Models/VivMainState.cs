using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Helpers;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Serializers;
using System.Collections.Specialized;
using System.Linq;

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
            }
        }
    }

    /// <summary>
    /// Calculates the estimated size of the VIV file.
    /// </summary>
    public int FileSize => VivSerializer.GetFileSize(_Viv.Directory);

    /// <summary>
    /// Gets a reference to an observable collection with the VIV directory.
    /// </summary>
    public ObservableDictionaryWrap<string, byte[]> Directory { get; private set; } = null!;

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'car.fce'
    /// entry.
    /// </summary>
    public bool HasCarFce => HasFile("car.fce");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'car.bnk'
    /// entry.
    /// </summary>
    public bool HasBnk => HasFile("car.bnk");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'dash.qfs'
    /// entry.
    /// </summary>
    public bool HasDash => HasFile("dash.qfs");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'fedata.fsh'
    /// entry.
    /// </summary>
    public bool HasFedataFsh => HasFile("fedata.fsh");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'fedata.bri'
    /// entry.
    /// </summary>
    public bool FeDataBri=> HasFile("fedata.bri");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'fedata.eng'
    /// entry.
    /// </summary>
    public bool FeDataEng => HasFile("fedata.eng");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'fedata.fre'
    /// entry.
    /// </summary>
    public bool FeDataFre => HasFile("fedata.fre");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'fedata.ger'
    /// entry.
    /// </summary>
    public bool FeDataGer => HasFile("fedata.ger");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'fedata.ita'
    /// entry.
    /// </summary>
    public bool FeDataIta => HasFile("fedata.ita");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'fedata.spa'
    /// entry.
    /// </summary>
    public bool FeDataSpa => HasFile("fedata.spa");

    /// <summary>
    /// Gets a value that indicates of the VIV file contains the 'fedata.swe'
    /// entry.
    /// </summary>
    public bool FeDataSwe => HasFile("fedata.swe");

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
        Directory.CollectionChanged += Directory_CollectionChanged;
        _unsavedChanges = false;
    }

    private bool HasFile(string key) => Directory.Keys.Any(p => p.Equals(key, System.StringComparison.InvariantCultureIgnoreCase));

    private void Directory_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Reset:
                Notify(
                    nameof(HasCarFce), nameof(HasBnk), nameof(HasDash),
                    nameof(HasFedataFsh), nameof(FeDataBri), nameof(FeDataEng),
                    nameof(FeDataFre), nameof(FeDataGer), nameof(FeDataIta),
                    nameof(FeDataSpa), nameof(FeDataSwe));
                break;
        }
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
        var viv = await parser.DeserializeAsync(fs);

        return new(viv)
        {
            UnsavedChanges = false,
            FilePath = path,
            FriendlyName = viv.TryGetValue("fedata.eng", out var fd) ? ((ISerializer<FeData>)new FeDataSerializer()).Deserialize(fd).CarName : Path.GetFileName(Path.GetDirectoryName(path)) ?? Path.GetFileName(path)
        };
    }
}
