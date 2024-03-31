using System.Threading.Tasks;
using TheXDS.Ganymede.Services;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes an
/// interactive tool on Vivianne.
/// </summary>
public interface IVivianneTool
{
    /// <summary>
    /// Gets the tool name.
    /// </summary>
    string ToolName => GetType().Name;

    /// <summary>
    /// Executes the interactive tool asyncronously.
    /// </summary>
    /// <param name="dialogService">
    /// Dialog service to use to interact with the user.
    /// </param>
    /// <param name="navigationService">
    /// Navigation service to use when requesting navigatino to a different
    /// ViewModel.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that can be used to await the execution of the
    /// tool.
    /// </returns>
    Task Run(IDialogService dialogService, INavigationService navigationService);
}
