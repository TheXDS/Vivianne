using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types;
using TheXDS.MCART.Component;

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

    /// <summary>
    /// Converts the tool to a button interaction that can be presented on the
    /// UI.
    /// </summary>
    /// <typeparam name="T">
    /// Type of tool to convert into a button interaction.
    /// </typeparam>
    /// <param name="dialogService">
    /// Dialog service to use to interact with the user.
    /// </param>
    /// <param name="navigationService">
    /// Navigation service to use when requesting navigatino to a different
    /// ViewModel.
    /// </param>
    /// <returns>
    /// A new button interaction that can be used to invoke the specified tool.
    /// </returns>
    public static ButtonInteraction ToInteraction<T>(IDialogService dialogService, INavigationService navigationService) where T : IVivianneTool, new()
    {
        var tool = new T();
        return new ButtonInteraction(new SimpleCommand(() => tool.Run(dialogService, navigationService)), tool.ToolName);
    }
}
