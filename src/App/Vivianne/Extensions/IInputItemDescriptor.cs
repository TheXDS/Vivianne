using System.Threading.Tasks;
using TheXDS.Ganymede.Services;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Defines a set of members to be implemented by a type that defines a
/// descriptor for user input when calling
/// <see cref="DialogServiceExtensions.AskSequentially(IDialogService, IInputItemDescriptor[])"/>.
/// </summary>
public interface IInputItemDescriptor
{
    /// <summary>
    /// Executes the prompt callbacks in this instance and returns the value
    /// entered by the user.
    /// </summary>
    /// <param name="svc">
    /// <see cref="IDialogService"/> instance to expose to the callback.
    /// </param>
    /// <returns>
    /// A <see cref="Task{T}"/> that can be used to await the async operation.
    /// Upon completion, the task will return the value entered by the user. If
    /// the user presses cancel on the dialog, the returned task result will be
    /// <see langword="null"/>.
    /// </returns>
    Task<object?> GetInput(IDialogService svc);
}
