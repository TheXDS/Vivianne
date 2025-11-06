using System;
using TheXDS.Ganymede.Types.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.FileErrorViewModel;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a simple ViewModel that allows the navigation stack to display
/// an error message if no suitable ViewModel could be found to handle a
/// specific file type.
/// </summary>
public class FileErrorViewModel(string errorMessage) : ViewModel
{
    /// <summary>
    /// Gets a ViewModel that can be used to display an error message when the
    /// file format is not supported by Vivianne.
    /// </summary>
    public static FileErrorViewModel UnknownFileFormat => new(St.UnknownFileFormat);

    /// <summary>
    /// Gets a ViewModel that can be used to display an error message when the
    /// specified file could not be found.
    /// </summary>
    public static FileErrorViewModel FileNotFound => new("File not found.");

    /// <summary>
    /// Initializes a new instance of the <see cref="FileErrorViewModel"/>
    /// class from an exception.
    /// </summary>
    /// <param name="ex">Exception to extract the error message from.</param>
    public FileErrorViewModel(Exception ex) : this(ex.Message)
    {
    }

    /// <summary>
    /// Gets the error message to be displayed in the ViewModel.
    /// </summary>
    public string ErrorMessage { get; } = errorMessage;
}
