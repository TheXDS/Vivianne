using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.Models.Base;

/// <summary>
/// Base class for all types that represent file editor states.
/// </summary>
/// <typeparam name="T">
/// Type for which this state represents an editor state.
/// </typeparam>
public abstract class   FileStateBase<T> : EditorViewModelStateBase, IFileState<T>
{
    /// <inheritdoc/>
    public T File { get; init; } = default!;

    /// <summary>
    /// Changes the property value on the underlying file.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of value of the underlying property.
    /// </typeparam>
    /// <param name="propSelector">Property selector.</param>
    /// <param name="value">Value to be set on the property.</param>
    /// <returns>
    /// <see langword="true"/> if the property on the underlying file has
    /// changed its value, <see langword="false"/> otherwise.
    /// </returns>
    protected bool Change<TValue>(Expression<Func<T, TValue>> propSelector, TValue value)
        where TValue : IEquatable<TValue>
    {
        var prop = ReflectionHelpers.GetProperty(propSelector);
        var oldValue = prop.GetValue(File);
        if ((oldValue is TValue v && v.Equals(value)) || ((object?[])[oldValue, value]).AreAllNull()) return false;
        prop.SetValue(File, value, null);
        Notify(prop.Name);
        if (prop.Name != nameof(UnsavedChanges)) UnsavedChanges = true;
        return true;
    }


    /// <summary>
    /// Changes the property value on the underlying file.
    /// </summary>
    /// <param name="propSelector">Property selector.</param>
    /// <param name="value">Value to be set on the property.</param>
    /// <returns>
    /// <see langword="true"/> if the property on the underlying file has
    /// changed its value, <see langword="false"/> otherwise.
    /// </returns>
    protected bool Change(Expression<Func<T, Enum>> propSelector, Enum value)
    {
        var prop = ReflectionHelpers.GetProperty(propSelector);
        Enum oldValue = (Enum)prop.GetValue(File)!;
        if (oldValue == value) return false;
        prop.SetValue(File, value, null);
        Notify(prop.Name);
        if (prop.Name != nameof(UnsavedChanges)) UnsavedChanges = true;
        return true;
    }

    /// <summary>
    /// Gets an observable wrap of the specified
    /// <see cref="Dictionary{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">Type of keys on the list.</typeparam>
    /// <typeparam name="TValue">
    /// Type of values held inthe list.
    /// </typeparam>
    /// <param name="dictionary">
    /// Dictionary for which to get an observable version.
    /// </param>
    /// <returns>
    /// A new observable wrap for the specified
    /// <see cref="Dictionary{TKey, TValue}"/>.
    /// </returns>
    protected ObservableDictionaryWrap<TKey, TValue> GetObservable<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        var d = new ObservableDictionaryWrap<TKey, TValue>(dictionary);
        d.CollectionChanged += (_, e) => UnsavedChanges = true;
        return d;
    }

    /// <summary>
    /// Gets an observable wrap of the specified <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of values held in the list.
    /// </typeparam>
    /// <param name="list">
    /// List for which to get an observable version.
    /// </param>
    /// <returns>
    /// A new observable wrap for the specified <see cref="IList{T}"/>.
    /// </returns>
    protected ObservableListWrap<TValue> GetObservable<TValue>(IList<TValue> list)
    {
        var d = new ObservableListWrap<TValue>(list);
        d.CollectionChanged += (_, e) => UnsavedChanges = true;
        return d;
    }

    /// <summary>
    /// Gets an observable wrap of the specified <see cref="ICollection{T}"/>.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of values held in the list.
    /// </typeparam>
    /// <param name="collection">
    /// Collection for which to get an observable version.
    /// </param>
    /// <returns>
    /// A new observable wrap for the specified <see cref="ICollection{T}"/>.
    /// </returns>
    protected ObservableCollectionWrap<TValue> GetObservable<TValue>(ICollection<TValue> collection)
    {
        var d = new ObservableCollectionWrap<TValue>(collection);
        d.CollectionChanged += (_, e) => UnsavedChanges = true;
        return d;
    }

}
