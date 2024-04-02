using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Services;

namespace TheXDS.Vivianne.Extensions;
/// <summary>
/// Extends Ganymede's <see cref="IDialogService"/> API to include a sequential
/// prompt method to ask for various values fromthe user.
/// </summary>
internal static class DialogServiceExtensions
{
    /// <summary>
    /// Asks the user a series of consecutive questions.
    /// </summary>
    /// <param name="dialogSvc">Dialog service to use.</param>
    /// <param name="callbacks">
    /// Collection of prompt callbacks to invoke.
    /// </param>
    /// <returns>
    /// A task that can be used to await the async operation. After completion,
    /// the task returns an array of answers given by the user boxed as
    /// <see cref="object"/>. If the user presses cancel on any of the dialogs,
    /// the returned result will be <see langword="null"/>.
    /// </returns>
    public static async Task<object[]?> AskSequentially(this IDialogService dialogSvc, params IInputItemDescriptor[] callbacks)
    {
        List<object> result = [];
        foreach (var callback in callbacks)
        {
            var input = await callback.GetInput(dialogSvc);
            if (input is null) return null;
            result.Add(input);
        }
        return [.. result];
    }

    /// <summary>
    /// Asks the user a series of consecutive questions.
    /// </summary>
    /// <param name="dialogSvc">Dialog service to use.</param>
    /// <param name="callbacks">
    /// Collection of prompt callbacks to invoke.
    /// </param>
    /// <returns>
    /// A task that can be used to await the async operation. After completion,
    /// the task returns an array of answers given by the user. If the user
    /// presses cancel on any of the dialogs, the returned task result will be
    /// <see langword="null"/>.
    /// </returns>
    public static async Task<T[]?> AskSequentially<T>(this IDialogService dialogSvc, params InputItemDescriptor<T>[] callbacks) where T : notnull
    {
        return (await AskSequentially(dialogSvc, (IInputItemDescriptor[])callbacks))?.Cast<T>().ToArray();
    }
}
