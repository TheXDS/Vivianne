using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fsh;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the current state of the FSH editor ViewModel.
/// </summary>
public class FshEditorState : FileStateBase<FshFile>
{
    private ObservableDictionaryWrap<string, FshBlob>? _entries;

    /// <summary>
    /// Gets the internal FSH directory that enumerates all included FSH blobs.
    /// </summary>
    public ObservableDictionaryWrap<string, FshBlob> Entries => _entries ??= GetObservable(File.Entries);

    /// <summary>
    /// Gets or sets a value that indicates the directory Id.
    /// </summary>
    /// <remarks>
    /// For NFS3, this value should remain as "<c>GIMX</c>"
    /// </remarks>
    public string DirectoryId
    {
        get => File.DirectoryId;
        set => Change(f => f.DirectoryId, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates if the file will be compressed when saved.
    /// </summary>
    public bool IsCompressed
    {
        get => File.IsCompressed;
        set => Change(f => f.IsCompressed, value);
    }
}
