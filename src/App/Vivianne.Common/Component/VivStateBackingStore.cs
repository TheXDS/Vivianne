using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.Component;

public class VivStateBackingStore(VivEditorViewModel viv) : IBackingStore
{
    private readonly VivEditorViewModel _viv = viv;

    public Task<byte[]?> ReadAsync(string fileName)
    {
        return Task.FromResult(_viv.State.File.TryGetValue(fileName, out var data) ? data : null);
    }

    public Task<bool> WriteAsync(string fileName, byte[] content)
    {
        _viv.State.File[fileName] = content;
        _viv.State.UnsavedChanges = true;
        return Task.FromResult(true);
    }

    public Task<DialogResult<string>> GetNewFileName(string? oldFileName)
    {
        return _viv.DialogService?.GetInputText(CommonDialogTemplates.FileSave, oldFileName) ?? Task.FromResult(new DialogResult<string>(oldFileName is not null, oldFileName ?? ""));
    }
}
