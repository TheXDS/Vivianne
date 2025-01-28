using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Defines a ser of member to be implemented by a type that creates editor
/// ViewModels for files with a physical file backing store.
/// </summary>
public interface IFileEditorViewModelLauncher : IViewModelLauncher
{
    /// <summary>
    /// Determines if this launcher can be used to open files with the
    /// specified extension when Vivianne is invoked with command line
    /// arguments.
    /// </summary>
    /// <param name="fileExtension">File extension to check.</param>
    /// <returns>
    /// <see langword="true"/> if this launcher can be used to open files with
    /// the specified extension, <see langword="false"/> otherwise.
    /// </returns>
    bool CanOpen(string fileExtension);

    /// <summary>
    /// Invokes the launcher with either a <see cref="RecentFileInfo"/> or a
    /// path to a physical file on the computer.
    /// </summary>
    /// <param name="parameter">Value to be parsed and opened.</param>
    /// <returns>
    /// A task that can be used to await the async operation.
    /// </returns>
    Task OnOpen(object parameter);

    /// <summary>
    /// Gets a list of recent files to be displayed on the startup ViewModel.
    /// </summary>
    RecentFileInfo[] RecentFiles { get; }

    /// <summary>
    /// Gets a reference to the command used to create new files using an
    /// editor ViewModel created by this instance.
    /// </summary>
    ICommand NewFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to edit an existing file using an
    /// editor ViewModel created by this instance.
    /// </summary>
    ICommand OpenFileCommand { get; }
}
