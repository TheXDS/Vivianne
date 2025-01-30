using System;
using System.Linq;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the current state of the main ViewModel used to manage VIV
/// files.
/// </summary>
public class VivEditorState : FileStateBase<VivFile>
{
    private ObservableDictionaryWrap<string, byte[]>? _directory;

    /// <summary>
    /// Calculates the estimated size of the VIV file.
    /// </summary>
    public int FileSize => VivSerializer.GetFileSize(File.Directory);

    /// <summary>
    /// Gets a reference to an observable collection with the VIV directory.
    /// </summary>
    public ObservableDictionaryWrap<string, byte[]> Directory => _directory ??= GetObservable(File.Directory);

    /// <summary>
    /// Gets a descriptive friendly name inferred from FeData.
    /// </summary>
    public string FriendlyName => File.GetFriendlyName() ?? "<unknown>";

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'car.fce'
    /// entry.
    /// </summary>
    public bool HasCarFce => HasFile("car.fce");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'car.bnk'
    /// entry.
    /// </summary>
    public bool HasBnk => HasFile("car.bnk");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'dash.qfs'
    /// entry.
    /// </summary>
    public bool HasDash => HasFile("dash.qfs");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'fedata.fsh'
    /// entry.
    /// </summary>
    public bool HasFedataFsh => HasFile("fedata.fsh");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'fedata.bri'
    /// entry.
    /// </summary>
    public bool FeDataBri => HasFile("fedata.bri");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'fedata.eng'
    /// entry.
    /// </summary>
    public bool FeDataEng => HasFile("fedata.eng");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'fedata.fre'
    /// entry.
    /// </summary>
    public bool FeDataFre => HasFile("fedata.fre");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'fedata.ger'
    /// entry.
    /// </summary>
    public bool FeDataGer => HasFile("fedata.ger");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'fedata.ita'
    /// entry.
    /// </summary>
    public bool FeDataIta => HasFile("fedata.ita");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'fedata.spa'
    /// entry.
    /// </summary>
    public bool FeDataSpa => HasFile("fedata.spa");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'fedata.swe'
    /// entry.
    /// </summary>
    public bool FeDataSwe => HasFile("fedata.swe");

    /// <summary>
    /// Gets a value that indicates if the VIV file contains the 'carpsim.txt'
    /// entry.
    /// </summary>
    public bool CarpSim => HasFile("carpsim.txt");

    private bool HasFile(string key) => Directory.Keys.Any(p => p.Equals(key, StringComparison.InvariantCultureIgnoreCase));

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        broadcastSetup.RegisterPropertyChangeBroadcast(() => UnsavedChanges, [
            () => HasCarFce,
            () => HasBnk,
            () => HasDash,
            () => HasFedataFsh,
            () => FeDataBri,
            () => FeDataEng,
            () => FeDataFre,
            () => FeDataGer,
            () => FeDataIta,
            () => FeDataSpa,
            () => FeDataSwe,
            () => CarpSim,
            ]);
    }
}
