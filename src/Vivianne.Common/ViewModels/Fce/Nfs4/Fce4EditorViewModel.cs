using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fce.Nfs4;
using TheXDS.Vivianne.Tools.Fce;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs4;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model for
/// NFS4 / MCO.
/// </summary>
public class Fce4EditorViewModel : FceEditorViewModelBase<
    Fce4EditorState,
    FceFile,
    FceColor,
    HsbColor,
    Fce4Part,
    Fce4RenderStateBuilder>
{
    private static readonly ReadOnlyDictionary<FceLodPreset, string[]> LodPartMapping = new Dictionary<FceLodPreset, string[]>()
    {
        { FceLodPreset.High, [":HB", ":HLFW", ":HRFW", ":HLMW", ":HRMW", ":HLRW", ":HRRW", ":OT", ":OS", ":OLB", ":ORB", ":OLM", ":ORM", ":OC", ":OH", ":OD", ":OND"] },
        { FceLodPreset.Medium, [":MB", ":MLFW", ":MRFW", ":MLMW", ":MRMW", ":MLRW", ":MRRW"] },
        { FceLodPreset.Low, [":LB"] },
        { FceLodPreset.Tiny, [":TB"] },
    }.AsReadOnly();

    /// <summary>
    /// Gets a reference to the command used to open a dialog to edit the FCE
    /// color tables.
    /// </summary>
    public ICommand ColorEditorCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to regenerate a damaged model for this FCE file.
    /// </summary>
    public ICommand RegenerateDamagedModelCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to sync the color tables across other FCE models.
    /// </summary>
    public SimpleCommand SyncColorTablesCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Fce4EditorViewModel"/> class.
    /// </summary>
    public Fce4EditorViewModel()
    {
        var cb = CommandBuilder.For(this);
        ColorEditorCommand = cb.BuildSimple(OnColorEditor);
        RegenerateDamagedModelCommand = cb.BuildSimple(OnRegenerateDamagedModel);
        SyncColorTablesCommand = cb.BuildSimple(OnSyncColorTablesCommand);
    }

    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        State.SelectedColor = State.Colors.FirstOrDefault();
        SyncColorTablesCommand.SetCanExecute(BackingStore?.FileName?.StartsWith("car", StringComparison.InvariantCultureIgnoreCase) ?? false);
        return base.OnCreated();
    }

    /// <inheritdoc/>
    protected override void OnSwitchToLod(FceLodPreset preset)
    {
        if (!LodPartMapping.TryGetValue(preset, out var mapping)) return;
        foreach (var j in State.Parts)
        {
            j.IsVisible = mapping.Contains(j.Part.Name);
        }
    }

    private async Task OnColorEditor()
    {
        var state = new Fce4ColorTableEditorState(State);
        var vm = new Fce4ColorEditorViewModel(state) { Title = "Color editor" };
        await DialogService!.Show(vm);
        await LoadColorNames();
        State.SelectedColor = State.Colors.FirstOrDefault();
        OnVisibleChanged();
    }

    private void OnRegenerateDamagedModel()
    {
        foreach (var part in State.Parts.Select(p => p.Part).Where(p => ((string[])[":HB", ":MB", ":LB"]).Contains(p.Name)))
        {
            part.DamagedVertices = FceDamageGenerator.GenerateDamageMesh(part.Vertices);
            part.DamagedNormals = FceDamageGenerator.GenerateDamageMesh(part.Normals);
        }
        OnVisibleChanged();
    }

    private async Task OnSyncColorTablesCommand()
    {
        if (BackingStore?.Store.AsDictionary() is not { } d) return;
        ISerializer<FceFile> serializer = new FceSerializer();
        IsBusy = true;
        foreach (string j in ((string[])["car", "car1", "car2", "car3"]).Select(p => $"{p}.fce").ExceptFor(BackingStore.FileName).NotNull())
        {
            if (d.TryGetValue(j, out byte[]? fceFile))
            {
                try
                {
                    var fce = await serializer.DeserializeAsync(fceFile);
                    fce.PrimaryColors = [.. State.Colors.Select(p => p.PrimaryColor)];
                    fce.SecondaryColors = [.. State.Colors.Select(p => p.SecondaryColor)];
                    fce.InteriorColors = [.. State.Colors.Select(p => p.InteriorColor)];
                    fce.DriverHairColors = [.. State.Colors.Select(p => p.DriverHairColor)];
                    d[j] = await serializer.SerializeAsync(fce);
                }
                catch { }
            }
        }
        IsBusy = false;
    }
}
