using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Vivianne.Models.Base;

/// <summary>
/// Base class for all types that represent the state of a
/// <see cref="IStatefulViewModel{T}"/> with support for notifying about
/// unsaved changes.
/// </summary>
public abstract class EditorViewModelStateBase : NotifyPropertyChanged
{
    private bool _unsavedChanges;
    private readonly List<string> _unconsequentialProps = [nameof(UnsavedChanges)];

    /// <summary>
    /// Gets or sets a value that indicates if the state contains unsaved
    /// changes.
    /// </summary>
    public bool UnsavedChanges
    {
        get => _unsavedChanges;
        set => Change(ref _unsavedChanges, value);
    }

    /// <summary>
    /// Registers a property with change notification as unconsequential; that
    /// is, it will not set the <see cref="UnsavedChanges"/> property to
    /// <see langword="true"/>.
    /// </summary>
    /// <param name="propertyName">name of the property.</param>
    protected void RegisterUnconsequentialProperty(params string[] propertyName)
    {
        _unconsequentialProps.AddRange(propertyName);
    }

    /// <summary>
    /// Registers a property with change notification as unconsequential; that
    /// is, it will not set the <see cref="UnsavedChanges"/> property to
    /// <see langword="true"/>.
    /// </summary>
    /// <typeparam name="T">Property type.</typeparam>
    /// <param name="propertySelector">Expression that selects the property.</param>
    protected void RegisterUnconsequentialProperty<T>(params Expression<Func<T>>[] propertySelector)
    {
        RegisterUnconsequentialProperty([.. propertySelector.Select(p => ReflectionHelpers.GetProperty(p).Name)]);
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
    protected ObservableDictionaryWrap<TKey, TValue> GetObservable<TKey, TValue>(IDictionary<TKey, TValue> dictionary) where TKey : notnull
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

    /// <inheritdoc/>
    protected override void OnDoChange<T>(ref T field, T value, string propertyName)
    {
        base.OnDoChange(ref field, value, propertyName);
        CheckUnsavedChanges(propertyName);
    }

    /// <summary>
    /// Checks for unconsequential properties and changes the
    /// <see cref="UnsavedChanges"/> property to <see langword="true"/> if the
    /// property was not unconsequential.
    /// </summary>
    /// <param name="propertyName">Name of the property to check for.</param>
    protected void CheckUnsavedChanges(string propertyName)
    {
        if (!_unconsequentialProps.Contains(propertyName)) UnsavedChanges = true;
    }
}
