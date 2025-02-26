using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Vivianne.Component;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// ViewModel that instructs the operating system to open a file, waits for the
/// process to end and optionally modifies the file inside the VIV.
/// </summary>
/// <param name="rawFile">Raw file contents.</param>
/// <param name="store">
/// Backing store to use when saving the file. Will usually be a VIV-backed
/// store, as opening a file with an external application from the filesystem
/// through Vivianne would not make sense, although it's possible to do so.
/// </param>
public class ExternalFileViewModel(byte[] rawFile, IBackingStore store, string name) : RawContentViewModel(rawFile)
{
    protected override Task OnCreated()
    {
        return DialogService.RunOperation(OnAwaitExternalApp);
        
    }

    private async Task OnAwaitExternalApp(IProgress<ProgressReport> progress)
    {
        progress.Report($"Extracting {name}...");
        var dir = Directory.CreateTempSubdirectory();
        var file = Path.Combine(dir.FullName, name);
        await File.WriteAllBytesAsync(file, RawFile);
        progress.Report("Waiting for application");
        var proc = Process.Start(file);
        if (proc is null || proc.HasExited) return;
        await proc.WaitForExitAsync();
        NavigationService.Reset();
    }
}
