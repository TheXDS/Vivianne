using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that displays information about the current VIV
/// file.
/// </summary>
public class VivInfoViewModel : ViewModel, IStatefulViewModel<VivEditorState>
{
    private VivEditorState state = null!;

    /// <inheritdoc/>
    public VivEditorState State
    {
        get => state;
        set => Change(ref state, value);
    }

    /// <summary>
    /// Gets a reference to the command used to export the entire VIV directory.
    /// </summary>
    public ICommand ExportAllCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VivInfoViewModel"/> class.
    /// </summary>
    public VivInfoViewModel()
    {
        var cb = CommandBuilder.For(this);
        ExportAllCommand = cb.BuildBusyOperation(OnExportAll);
    }

    private async Task OnExportAll(IProgress<ProgressReport> p)
    {
        var r = await DialogService!.GetDirectoryPath(CommonDialogTemplates.DirectorySelect with
        {
            Title = "Export all",
            Text = "Select a path to extract all VIV file contents into."
        });
        if (!r.Success) return;
        
        var c = 0;
        foreach (var j in State.File.Directory)
        {
            p.Report(new(c * 100 / State.File.Directory.Count, $"Exporting {j.Key}..."));
            await File.WriteAllBytesAsync(Path.Combine(r.Result, j.Key), j.Value);
            c++;
        }
    }
}
