using TheXDS.Ganymede.Types.Base;
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
        if (propertyName != nameof(UnsavedChanges)) UnsavedChanges = true;
    }
}
