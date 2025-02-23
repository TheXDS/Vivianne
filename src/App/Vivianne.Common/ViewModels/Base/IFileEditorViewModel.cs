using System.Windows.Input;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Defines a set of members to be implemented by a type that conforms a file
/// editor ViewModel for a specific type of file.
/// </summary>
/// <typeparam name="TState">Type of state inside the ViewModel.</typeparam>
/// <typeparam name="TFile">
/// Type of file for which the ViewModel is an editor.
/// </typeparam>
public interface IFileEditorViewModel<TState, TFile> : IStatefulViewModel<TState> where TState : IFileState<TFile> where TFile : notnull
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
    /// Gets a reference to the command used to close the active editor.
    /// </summary>
    ICommand CloseCommand { get; }

    /// <summary>
    /// Gets or initializes a reference to the backing store used to read and
    /// write files.
    /// </summary>
    IBackingStore<TFile>? BackingStore { get; init; }
}
