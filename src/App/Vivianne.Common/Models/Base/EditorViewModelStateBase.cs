using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Helpers;
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
    /// Gets or sets a value that indicates if the state contains unsaved
    /// changes.
    /// </summary>
    public bool UnsavedChanges
    {
        get => _unsavedChanges;
        set => Change(ref _unsavedChanges, value);
    }

    /// <inheritdoc/>
    protected override void OnDoChange<T>(ref T field, T value, string propertyName)
    {
        base.OnDoChange(ref field, value, propertyName);
        if (!_unconsequentialProps.Contains(propertyName)) UnsavedChanges = true;
    }
}
