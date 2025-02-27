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
public interface IFileEditorViewModel<TState, TFile> : IFileEditorViewModel, IStatefulViewModel<TState> where TState : IFileState<TFile> where TFile : notnull
{
    /// <summary>
    /// Gets or initializes a reference to the backing store used to read and
    /// write files.
    /// </summary>
    IBackingStore<TFile>? BackingStore { get; init; }
}
