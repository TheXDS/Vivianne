using System.Windows.Input;
using TheXDS.Ganymede.Types.Base;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Defines a set of members to be implemented by a type that conforms a file
/// editor ViewModel for a specific type of file.
/// </summary>
/// <typeparam name="TState">Type of state inside the ViewModel.</typeparam>
/// <typeparam name="TFile">
/// Type of file for which the ViewModel is an editor.
/// </typeparam>
public interface IFileEditorViewModel<TState, TFile> : IStatefulViewModel<TState> where TState : IFileState<TFile>
{
    /// <summary>
    /// Gets a reference to the command used to save all changes made to the
    /// file.
    /// </summary>
    ICommand SaveCommand { get; set; }

    /// <summary>
    /// Gets a reference to the command used to save all changes made to the
    /// file to a different stream.
    /// </summary>
    ICommand? SaveAsCommand { get; set; }

    /// <summary>
    /// Gets a reference to the command used to close the active editor.
    /// </summary>
    public ICommand CloseCommand { get; }
}
