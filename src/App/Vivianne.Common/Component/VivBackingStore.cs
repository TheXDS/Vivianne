using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Vivianne.ViewModels.Viv;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Implements a backing store that saves data to a VIV file.
/// </summary>
/// <param name="viv">Instance of the VIV editorto use when saving data.</param>
public class VivBackingStore(VivEditorViewModel viv) : IBackingStore
{
    private readonly VivEditorViewModel _viv = viv;

    /// <inheritdoc/>
    public Task<byte[]?> ReadAsync(string fileName)
    {
        return Task.FromResult(_viv.State.File.TryGetValue(fileName, out var data) ? data : null);
    }

    /// <inheritdoc/>
    public Task<bool> WriteAsync(string fileName, byte[] content)
    {
        if (_viv.State.File.ContainsKey(fileName))
        {
            _viv.State.Directory[fileName] = content;
        }
        else
        {
            _viv.State.Directory.Add(fileName, content);
        }
        _viv.State.UnsavedChanges = true;
        return Task.FromResult(true);
    }

    /// <inheritdoc/>
    public Task<DialogResult<string?>> GetNewFileName(string? oldFileName)
    {
        return _viv.DialogService?.GetInputText(CommonDialogTemplates.FileSave, oldFileName) ?? Task.FromResult(new DialogResult<string?>(oldFileName is not null, oldFileName ?? ""));
    }

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateFiles()
    {
        return _viv.State.Directory.Keys;
    }

    IDictionary<string, byte[]> IBackingStore.AsDictionary()
    {
        return _viv.State.File.Directory;
    }
}
