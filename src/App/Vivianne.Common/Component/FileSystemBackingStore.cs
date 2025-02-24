using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Services;

namespace TheXDS.Vivianne.Component;

public class FileSystemBackingStore(IDialogService dialogSvc, IEnumerable<FileFilterItem> saveFilters, string path) : IBackingStore
{
    private readonly IDialogService _dialogSvc = dialogSvc;
    private readonly IEnumerable<FileFilterItem> _saveFilters = saveFilters;
    private readonly string path = path;

    public async Task<byte[]?> ReadAsync(string fileName)
    {
        return File.Exists(fileName) ? await File.ReadAllBytesAsync(fileName) : null;
    }

    public async Task<bool> WriteAsync(string fileName, byte[] content)
    {
        try
        {
            await File.WriteAllBytesAsync(Path.Combine(path, fileName), content);
            return true;
        }
        catch (System.Exception ex)
        {
            await _dialogSvc.Error(ex);
            return false;
        }
    }

    public Task<DialogResult<string>> GetNewFileName(string? oldFileName)
    {
        return _dialogSvc.GetFileSavePath(CommonDialogTemplates.FileSave, _saveFilters, Path.Combine(path, oldFileName));
    }

    public IEnumerable<string> EnumerateFiles()
    {
        return Directory.EnumerateFiles(path);
    }
}
