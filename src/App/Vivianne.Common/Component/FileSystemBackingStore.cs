using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Services;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Properties;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Implements a backing store that reads and writes files on the computer's
/// filesystem.
/// </summary>
/// <param name="dialogSvc">
/// Dialog service to use when requesting new filenames to store copies or new
/// versions of an entity.
/// </param>
/// <param name="saveFilters">
/// File filters to use when invoking the dialog service.
/// </param>
/// <param name="path">
/// Full path to the file in the computer's filesystem.
/// </param>
public class FileSystemBackingStore(IDialogService dialogSvc, IEnumerable<FileFilterItem> saveFilters, string path) : IBackingStore
{
    private readonly IDialogService _dialogSvc = dialogSvc;
    private readonly IEnumerable<FileFilterItem> _saveFilters = saveFilters;
    private readonly string path = path;

    /// <inheritdoc/>
    public async Task<byte[]?> ReadAsync(string fileName)
    {
        return File.Exists(fileName) ? await File.ReadAllBytesAsync(fileName) : null;
    }

    /// <inheritdoc/>
    public async Task<bool> WriteAsync(string fileName, byte[] content)
    {
        try
        {
            if (Settings.Current.AutoBackup) FileBackup.Create(fileName);            
            await File.WriteAllBytesAsync(Path.Combine(path, fileName), content);
            return true;
        }
        catch (System.Exception ex)
        {
            await _dialogSvc.Error(ex);
            return false;
        }
    }

    /// <inheritdoc/>
    public Task<DialogResult<string?>> GetNewFileName(string? oldFileName)
    {
        return _dialogSvc.GetFileSavePath(CommonDialogTemplates.FileSave, _saveFilters, Path.Combine(path, oldFileName ?? ""));
    }

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateFiles()
    {
        return Directory.EnumerateFiles(path);
    }
}
