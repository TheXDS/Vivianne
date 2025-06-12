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
public interface IFileEditorViewModel<TState, TFile> : 
    IFileEditorViewModel,
    IBackingStoreViewModel<TFile>,
    IStatefulViewModel<TState>
    where TState : IFileState<TFile>
    where TFile : notnull;
