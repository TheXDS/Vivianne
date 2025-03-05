using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.Tools.FshCompressTool;
using St2 = TheXDS.Vivianne.Resources.Strings.Tools.QfsDecompressTool;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a launcher to create and/or edit FSH and QFS files.
/// </summary>
public class FshFileEditorLauncher(Func<IDialogService> dialogSvc)
    : FileEditorViewModelLauncher<FshEditorState, FshFile, FshSerializer, FshEditorViewModel>(dialogSvc, "FSH/QFS", FileFilters.FshQfsOpenFileFilter, FileFilters.FshQfsSaveFileFilter)
{
    private async Task OnDecompress()
    {
        if (DialogService is null) return;
        var fin = await DialogService.GetFileOpenPath(St2.OpnMessage, FileFilters.QfsFileFilter);
        if (!fin.Success) return;
        var fout = await DialogService.GetFileSavePath(St2.SveMessage, FileFilters.FshFileFilter);
        if (!fout.Success) return;
        await DialogService.RunOperation(async p =>
        {
            p.Report(St2.ProcessMsg);
            var qfs = await File.ReadAllBytesAsync(fin.Result);
            var fsh = await Task.Run(() => QfsCodec.Decompress(qfs));
            await File.WriteAllBytesAsync(fout.Result, fsh);
        });
    }

    private async Task OnCompress()
    {
        if (DialogService is null) return;
        var fin = await DialogService.GetFileOpenPath(St.OpnMessage, FileFilters.FshFileFilter);
        if (!fin.Success) return;
        var fout = await DialogService.GetFileSavePath(St.SveMessage, FileFilters.QfsFileFilter);
        if (!fout.Success) return;
        await DialogService.RunOperation(async p =>
        {
            p.Report(St.ProcessMsg);
            var fsh = await File.ReadAllBytesAsync(fin.Result);
            var qfs = await Task.Run(() => QfsCodec.Compress(fsh));
            await File.WriteAllBytesAsync(fout.Result, qfs);
        });
    }

    /// <inheritdoc/>
    public override RecentFileInfo[] RecentFiles
    {
        get => Settings.Current.RecentFshFiles;
        set => Settings.Current.RecentFshFiles = value;
    }

    /// <inheritdoc/>
    public override IEnumerable<ButtonInteraction> AdditionalInteractions => [
        new(new SimpleCommand(OnCompress), St.ToolName),
        new(new SimpleCommand(OnDecompress), St2.ToolName),
        ];

    /// <inheritdoc/>
    protected override void BeforeSave(FshFile state, string fileExtension)
    {
        base.BeforeSave(state, fileExtension);
        state.IsCompressed = Path.GetExtension(fileExtension).Equals(".qfs", System.StringComparison.InvariantCultureIgnoreCase);
    }
}
