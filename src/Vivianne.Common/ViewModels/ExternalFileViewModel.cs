using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// ViewModel that instructs the operating system to open a file, waits for the
/// process to end and optionally modifies the file inside the VIV.
/// </summary>
/// <param name="store">
/// Backing store to use when saving the file. Will usually be a VIV-backed
/// store, as opening a file with an external application from the filesystem
/// through Vivianne would not make sense, although it's possible to do so.
/// </param>
/// <param name="name">Name of the file to be opened externally.</param>
public class ExternalFileViewModel(IBackingStore store, string name) : ViewModel()
{
    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        return DialogService?.RunOperation(OnAwaitExternalApp) ?? Task.CompletedTask;
    }

    private async Task OnAwaitExternalApp(IProgress<ProgressReport> progress)
    {
        try
        {
            progress.Report($"Extracting {name}...");
            if (await CreateTempFile() is string tempFile)
            {
                progress.Report("Waiting for external sapplication");
                var proc = Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });
                if (!(proc is null || proc.HasExited))
                {
                    await proc.WaitForExitAsync();
                    progress.Report($"Repacking {name}...");
                    await RepackFile(tempFile);
                }
                await (NavigationService?.Reset() ?? Task.CompletedTask);
            }
            else
            {
                await (NavigationService?.NavigateAndReset(new FileErrorViewModel($"Could not extract {name}")) ?? Task.CompletedTask);
            }
        }
        catch(Exception ex)
        {
            await (NavigationService?.NavigateAndReset(new FileErrorViewModel(ex)) ?? Task.CompletedTask);
        }
    }

    private async Task<string?> CreateTempFile()
    {
        if (await store.ReadAsync(name) is not { } contents) return null;        
        var dir = Directory.CreateTempSubdirectory();
        var file = Path.Combine(dir.FullName, name);
        await File.WriteAllBytesAsync(file, contents);
        return file;
    }

    private async Task RepackFile(string tempFile)
    {
        await store.WriteAsync(name, await File.ReadAllBytesAsync(tempFile));
        File.Delete(tempFile);
        if (Path.GetDirectoryName(tempFile) is { } dir) Directory.Delete(dir);
    }
}
