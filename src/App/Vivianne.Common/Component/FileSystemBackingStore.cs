using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Services;
using TheXDS.MCART.Helpers;
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
public class FileSystemBackingStore(IDialogService dialogSvc, IEnumerable<FileFilterItem> saveFilters, string path) : IBackingStore, IDictionary<string, byte[]>
{
    private readonly IDialogService _dialogSvc = dialogSvc;
    private readonly IEnumerable<FileFilterItem> _saveFilters = saveFilters;
    private readonly string path = path;

    ICollection<string> IDictionary<string, byte[]>.Keys => [.. EnumerateFiles()];

    ICollection<byte[]> IDictionary<string, byte[]>.Values => [..EnumerateFiles().Select(File.ReadAllBytes)];

    int ICollection<KeyValuePair<string, byte[]>>.Count => EnumerateFiles().Count();

    bool ICollection<KeyValuePair<string, byte[]>>.IsReadOnly => false;

    byte[] IDictionary<string, byte[]>.this[string key]
    {
        get => File.ReadAllBytes(key);
        set => File.WriteAllBytes(key, value);
    }

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

    /// <inheritdoc/>
    public IDictionary<string, byte[]> AsDictionary() => this;

    void IDictionary<string, byte[]>.Add(string key, byte[] value)
    {
        File.WriteAllBytes(key, value);
    }

    bool IDictionary<string, byte[]>.ContainsKey(string key)
    {
        return File.Exists(key);
    }

    bool IDictionary<string, byte[]>.Remove(string key)
    {
        var result = File.Exists(key);
        File.Delete(key);
        return result;
    }

    bool IDictionary<string, byte[]>.TryGetValue(string key, out byte[] value)
    {
        var result = File.Exists(key);
        value = result ? File.ReadAllBytes(key) : [];
        return result;
    }

    void ICollection<KeyValuePair<string, byte[]>>.Add(KeyValuePair<string, byte[]> item)
    {
        File.WriteAllBytes(item.Key, item.Value);
    }

    void ICollection<KeyValuePair<string, byte[]>>.Clear()
    {
        throw new System.NotImplementedException();
    }

    bool ICollection<KeyValuePair<string, byte[]>>.Contains(KeyValuePair<string, byte[]> item)
    {
        return File.Exists(item.Key) && File.ReadAllBytes(item.Key).SequenceEqual(item.Value);
    }

    void ICollection<KeyValuePair<string, byte[]>>.CopyTo(KeyValuePair<string, byte[]>[] array, int arrayIndex)
    {
        foreach (var (index, element) in EnumerateFiles().WithIndex())
        {
            array[arrayIndex + index] = new KeyValuePair<string, byte[]>(element, File.ReadAllBytes(element));
        }
    }

    bool ICollection<KeyValuePair<string, byte[]>>.Remove(KeyValuePair<string, byte[]> item)
    {
        if (((ICollection<KeyValuePair<string, byte[]>>)this).Contains(item))
        {
            File.Delete(item.Key);
            return true;
        }
        return false;
    }

    IEnumerator<KeyValuePair<string, byte[]>> IEnumerable<KeyValuePair<string, byte[]>>.GetEnumerator()
    {
        return EnumerateFiles().Select(p => new KeyValuePair<string, byte[]>(p, File.ReadAllBytes(p))).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, byte[]>>)this).GetEnumerator();
    }
}