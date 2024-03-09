using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Types;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Tools;
using St = TheXDS.Vivianne.Resources.Strings.Views.StartupView;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements the startup page logic.
/// </summary>
public class StartupViewModel : ViewModel
{
    private IEnumerable<VivInfo> recentFiles = [];

    /// <summary>
    /// Gets a list of recent files that can be quickly opened from the UI.
    /// </summary>
    public IEnumerable<VivInfo> RecentFiles
    { 
        get => recentFiles;
        private set => Change(ref recentFiles, value);
    }

    /// <summary>
    /// Gets a collection of custom tools that can be launched from the startup
    /// view.
    /// </summary>
    public ICollection<ButtonInteraction> ExtraTools { get; } = [];

    /// <summary>
    /// Gets a reference to the command used to create a new, blank VIV file.
    /// </summary>
    public ICommand NewVivCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to open an existing VIV file.
    /// </summary>
    public ICommand OpenVivCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupViewModel"/> class.
    /// </summary>
    public StartupViewModel()
    {
        var cb = CommandBuilder.For(this);
        NewVivCommand = cb.BuildSimple(OnNewViv);
        OpenVivCommand = cb.BuildSimple(OnOpenViv);
        ExtraTools.Add(new(cb.BuildSimple(OnQfsDecompressCommand), "QFS decompress"));
        ExtraTools.Add(new(cb.BuildSimple(OnFshCompressCommand), "FSH compress"));
        ExtraTools.Add(new(cb.BuildSimple(OnFshPreviewCommand), "FSH preview"));
        ExtraTools.Add(new(cb.BuildSimple(() => SerialNumberAnalyzer.RunTool(DialogService!)), "Serial number analyzer"));
    }

    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        await Settings.Load();
        RecentFiles = Settings.Current.RecentFiles;
    }

    private void OnNewViv()
    {
        NavigationService!.NavigateAndReset<VivMainViewModel, VivMainState>(new());
    }

    private async Task OnOpenViv(object? parameter)
    {
        string? filePath = null;
        List<VivInfo> l = RecentFiles.ToList();
        if (parameter is VivInfo viv)
        {
            l.Remove(viv);
            filePath = viv.FilePath;
        }
        else
        {
            var f = await DialogService!.GetFileOpenPath(St.Open, St.OpenMessage, FileFilters.VivFileFilter);
            if (f.Success)
            {
                filePath = f.Result;
            }
        }
        if (filePath is null) return;
        

        var s = await DialogService!.RunOperation(_ => VivMainState.From(filePath));
        l = new VivInfo[] { s }.Concat(l).Take(10).ToList();
        Settings.Current.RecentFiles = [.. l];
        await Settings.Save();
        NavigationService!.NavigateAndReset<VivMainViewModel, VivMainState>(s);
    }

    private async Task OnQfsDecompressCommand()
    {
        var fin = await DialogService!.GetFileOpenPath(St.Open, St.OpenMessage, FileFilters.QfsFileFilter);
        if (fin.Success) 
        {
            var fout = await DialogService!.GetFileSavePath(St.Open, St.OpenMessage, FileFilters.FshFileFilter);
            if (fout.Success)
            {
                await DialogService.RunOperation(async p =>
                {
                    p.Report("Converting QFS to FSH...");
                    var qfs = await File.ReadAllBytesAsync(fin.Result);
                    var fsh = await Task.Run(() => QfsCodec.Decompress(qfs));
                    await File.WriteAllBytesAsync(fout.Result, fsh);
                });
            }
        }
    }

    private async Task OnFshPreviewCommand()
    {
        var fin = await DialogService!.GetFileOpenPath(St.Open, St.OpenMessage, FileFilters.FshQfsFileFilter);
        if (fin.Success)
        {
            var data = QfsCodec.Decompress(await File.ReadAllBytesAsync(fin.Result));
            using var ms = new MemoryStream(data);
            NavigationService!.Navigate(new FshPreviewViewModel(new FshSerializer().Deserialize(ms)));
        }
    }

    private async Task OnFshCompressCommand()
    {
        var fin = await DialogService!.GetFileOpenPath(St.Open, St.OpenMessage, FileFilters.FshFileFilter);
        if (fin.Success)
        {
            var fout = await DialogService!.GetFileSavePath(St.Open, St.OpenMessage, FileFilters.QfsFileFilter);
            if (fout.Success)
            {
                await DialogService.RunOperation(async p =>
                {
                    p.Report("Converting FSH to QFS...");
                    var fsh = await File.ReadAllBytesAsync(fin.Result);
                    var qfs = await Task.Run(() => QfsCodec.Compress(fsh));
                    await File.WriteAllBytesAsync(fout.Result, fsh);
                });
            }
        }
    }
}

/// <summary>
/// ViewModel that instructs the operating system to open a file, waits for the
/// process to end and optionally modifies the file inside the VIV.
/// </summary>
/// <param name="rawFile">Raw file contents.</param>
/// <param name="saveCallback">
/// Save callback to execute in case the file was changed externally.
/// </param>
public class ExternalFileViewModel(byte[] rawFile, Action<byte[]> saveCallback) : RawContentViewModel(rawFile)
{
    protected readonly Action<byte[]> _saveCallback = saveCallback;
}

public class CarpGeneratorViewModel : ViewModel
{

}