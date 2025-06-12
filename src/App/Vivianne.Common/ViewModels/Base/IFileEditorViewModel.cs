using System.Windows.Input;
using TheXDS.Ganymede.Types.Base;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes commands to save a file.
/// </summary>
public interface IFileEditorViewModel : IViewModel
{
    /// <summary>
    /// Gets a reference to the command used to save all changes made to the
    /// file.
    /// </summary>
    ICommand SaveCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to save all changes made to the
    /// file to a different stream.
    /// </summary>
    ICommand SaveAsCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to save the current file and close the editor.
    /// </summary>
    ICommand SaveAndCloseCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to close the active editor, discarding all changes.
    /// </summary>
    ICommand DiscardAndCloseCommand { get; }

    /// <summary>
    /// Gets a value that indicates if there's unsaved changes in the editor.
    /// </summary>
    bool UnsavedChanges { get; }
}
