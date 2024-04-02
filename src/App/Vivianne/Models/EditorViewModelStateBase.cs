﻿using System.Runtime.CompilerServices;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Base class for all types that represent the _state of a
/// <see cref="IStatefulViewModel{T}"/> with support for notifying about
/// unsaved changes.
/// </summary>
public abstract class EditorViewModelStateBase : NotifyPropertyChanged
{
    private bool _unsavedChanges;

    /// <summary>
    /// Gets or sets a value that indicates if the _state contains unsaved
    /// changes.
    /// </summary>
    public bool UnsavedChanges
    {
        get => _unsavedChanges;
        set => base.Change(ref _unsavedChanges, value);
    }

    /// <inheritdoc/>
    protected override bool Change<T>(ref T field, T value, [CallerMemberName] string propertyName = null!)
    {
        var result = base.Change(ref field, value, propertyName);
        if (result)
        {
            UnsavedChanges = true;
        }
        return result;
    }

}
