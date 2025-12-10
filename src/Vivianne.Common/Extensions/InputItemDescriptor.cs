using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Services;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Represents a descriptor for user input when calling
/// <see cref="DialogServiceExtensions.AskSequentially{T}(IDialogService, InputItemDescriptor{T}[])"/>
/// or
/// <see cref="DialogServiceExtensions.AskSequentially(IDialogService, IInputItemDescriptor[])"/>.
/// </summary>
/// <typeparam name="T">Type of value obtained by this instance.</typeparam>
/// <param name="inputCallback">
/// Callback to execute when asking the user for data.
/// </param>
/// <param name="isInvalidCallback">
/// If specified, indicates the callback to use when validating the data
/// entered by the user.
/// </param>
/// <exception cref="ArgumentNullException">
/// Thrown if <paramref name="inputCallback"/> is <see langword="null"/>.
/// </exception>
public class InputItemDescriptor<T>(Func<IDialogService, Task<DialogResult<T?>>> inputCallback, InputItemDescriptor<T>.IsInvalidCallback? isInvalidCallback = null) : IInputItemDescriptor where T : notnull
{
    /// <summary>
    /// Represents a delegate that will execute validation of the data entered
    /// by the user.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <param name="errorMessage">
    /// Output parameter. If the value is invalid, this output parameter will
    /// contain the error message produced during validation.
    /// </param>
    /// <returns>
    /// <see langword="false"/> if the value passes all validation,
    /// <see langword="true"/> otherwise.
    /// </returns>
    public delegate bool IsInvalidCallback(T? value, [NotNullWhen(true)] out string? errorMessage);

    private readonly Func<IDialogService, Task<DialogResult<T?>>> inputCallback = inputCallback ?? throw new ArgumentNullException(nameof(inputCallback));
    private readonly IsInvalidCallback? isInvalidCallback = isInvalidCallback;

    /// <inheritdoc/>
    public async Task<object?> GetInput(IDialogService svc)
    {
        var result = await inputCallback.Invoke(svc);
        if (!result.Success) return null;
        if (isInvalidCallback is not null && isInvalidCallback.Invoke(result.Result, out var errorMessage))
        {
            await svc.Error(errorMessage);
            return null;
        }
        return result.Result;
    }
}
