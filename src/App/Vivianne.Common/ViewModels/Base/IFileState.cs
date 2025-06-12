using System.ComponentModel;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Defines a set of members to be implemented by a type that represents the
/// state of a file editor ViewModel.
/// </summary>
/// <typeparam name="TFile">
/// Type of file for which the class is the active editor state.
/// </typeparam>
public interface IFileState<TFile> : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets a value that indicates whether there's unsaved changes.
    /// </summary>
    bool UnsavedChanges { get; set; }

    /// <summary>
    /// Gets a reference to the file being edited.
    /// </summary>
    TFile File { get; init; }
}
