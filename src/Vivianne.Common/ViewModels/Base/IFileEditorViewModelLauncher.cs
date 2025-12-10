using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Defines a ser of member to be implemented by a type that creates editor
/// ViewModels for files with a physical file backing store.
/// </summary>
public interface IFileEditorViewModelLauncher : IFileViewerViewModelLauncher
{
    /// <summary>
    /// Gets a reference to the command used to create new files using an
    /// editor ViewModel created by this instance.
    /// </summary>
    ICommand NewFileCommand { get; }

    /// <summary>
    /// Gets a value that indicates if the "New" command should be available.
    /// </summary>
    bool CanCreateNew { get; }
}
