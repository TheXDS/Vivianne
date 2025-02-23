using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fce.Nfs3;
using TheXDS.Vivianne.Serializers.Fe.Nfs3;
using TheXDS.Vivianne.Tools;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to see and edit FeData car
/// information files for Need For Speed 3.
/// </summary>
public class FeData3EditorViewModel : ViewModel
{
    private static readonly ISerializer<FeData> serializer = new FeDataSerializer();
    private readonly Action<byte[]> saveCallback;
    private readonly VivEditorState? viv;
    private readonly string? fedataName;
    private bool _LinkEdits;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeData3EditorViewModel"/>
    /// class.
    /// </summary>
    /// <param name="data">FeData to manipulate.</param>
    /// <param name="saveCallback">
    /// Callback to invoke when saving the FeData contents.
    /// </param>
    /// <param name="viv">
    /// Reference to the VIV file when attempting to sync up changes on other
    /// files.
    /// </param>
    /// <param name="fedataName">
    /// Name of the FeData file being edited, for sync purposes.
    /// </param>
    public FeData3EditorViewModel(byte[] data, Action<byte[]> saveCallback, VivEditorState? viv = null, string? fedataName = null)
    {
        this.saveCallback = saveCallback;
        this.viv = viv;
        this.fedataName = fedataName;
        Data = serializer.Deserialize(data);
        SaveCommand = new SimpleCommand(OnSave);
        PreviewFceColorTable = LoadColorsFromFce(viv?.File);
    }

    private static Fce3Color[]? LoadColorsFromFce(VivFile? viv)
    {
        if (viv is null || !viv.TryGetValue("car.fce", out var data)) return null;
        ISerializer<FceFile> s = new FceSerializer();
        var fce = s.Deserialize(data);
        return [];// fce.PrimaryColors.Zip(fce.SecondaryColors).Select(p => new Fce3Color(p.First, p.Second)).ToArray();
    }

    /// <summary>
    /// Gets a table of the colors defined in the FCE file.
    /// </summary>
    public Fce3Color[]? PreviewFceColorTable { get; }

    /// <summary>
    /// Gets the <see cref="FeData"/> instance to view/edit.
    /// </summary>
    public FeData Data { get; }

    /// <summary>
    /// Gets a reference to the command used to save the changes made to the
    /// FeData File.
    /// </summary>
    public ICommand SaveCommand { get; }

    /// <summary>
    /// Gets or sets a value that indicates that changes made on the FeData
    /// editor should be synced up on other FeData files and the Carp.txt file.
    /// </summary>
    public bool LinkEdits
    {
        get => _LinkEdits;
        set => Change(ref _LinkEdits, value);
    }

    private void OnSave()
    {
        saveCallback?.Invoke(serializer.Serialize(Data));
        if (LinkEdits) OnSyncChanges();
    }

    private void OnSyncChanges()
    {
        if (viv is null || fedataName is null || Path.GetExtension(fedataName) is not { } ext) return;
        FeData3SyncTool.Sync(Data, ext, viv.Directory);
    }
}