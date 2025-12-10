using System;
using System.Linq;
using System.Linq.Expressions;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.Models.Base;

/// <summary>
/// Base class for all types that represent file editor states.
/// </summary>
/// <typeparam name="T">
/// Type for which this state represents an editor state.
/// </typeparam>
public abstract class FileStateBase<T> : EditorViewModelStateBase, IFileState<T>
{
    /// <inheritdoc/>
    public T File { get; init; } = default!;

    /// <summary>
    /// Changes the property newValue on the underlying file.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of newValue of the underlying property.
    /// </typeparam>
    /// <param name="propSelector">Property selector.</param>
    /// <param name="value">Value to be set on the property.</param>
    /// <returns>
    /// <see langword="true"/> if the property on the underlying file has
    /// changed its newValue, <see langword="false"/> otherwise.
    /// </returns>
    protected bool Change<TValue>(Expression<Func<T, TValue>> propSelector, TValue value)
        where TValue : IEquatable<TValue>
    {
        return Change(propSelector, value, (oldValue, newValue) => oldValue.Equals(newValue));
    }

    /// <summary>
    /// Changes the property newValue on the underlying file.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of newValue of the underlying property.
    /// </typeparam>
    /// <param name="propSelector">Property selector.</param>
    /// <param name="value">Value to be set on the property.</param>
    /// <returns>
    /// <see langword="true"/> if the property on the underlying file has
    /// changed its newValue, <see langword="false"/> otherwise.
    /// </returns>
    protected bool Change<TValue>(Expression<Func<T, TValue[]>> propSelector, TValue[] value)
    {
        return Change(propSelector, value, (oldValue, newValue) => oldValue.SequenceEqual(newValue));
    }

    /// <summary>
    /// Changes the property newValue on the underlying file.
    /// </summary>
    /// <param name="propSelector">Property selector.</param>
    /// <param name="value">Value to be set on the property.</param>
    /// <returns>
    /// <see langword="true"/> if the property on the underlying file has
    /// changed its newValue, <see langword="false"/> otherwise.
    /// </returns>
    protected bool Change(Expression<Func<T, Enum>> propSelector, Enum value)
    {
        return Change(propSelector, value, (oldValue, newValue) => oldValue == newValue);
    }

    private bool Change<TValue>(Expression<Func<T, TValue>> propSelector, TValue newValue, Func<TValue, TValue, bool> compareCallback)
    {
        var prop = ReflectionHelpers.GetProperty(propSelector);
        var oldValue = (TValue)prop.GetValue(File)!;
        if (compareCallback(oldValue, newValue)) return false;
        prop.SetValue(File, newValue, null);
        Notify(prop.Name);
        CheckUnsavedChanges(prop.Name);
        return true;
    }
}
