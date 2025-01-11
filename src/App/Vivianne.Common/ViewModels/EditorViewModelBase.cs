using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.ViewModels;
using TheXDS.MCART.Component;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using St = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Base class for all ViewModels that implement the 
/// <see cref="IStatefulViewModel{TState}"/> interface to provide editing
/// functionality for the state, as well as a command to persist these changes.
/// </summary>
/// <typeparam name="T">Type of state to edit.</typeparam>
public abstract class EditorViewModelBase<T> : AwaitableDialogViewModel, IStatefulViewModel<T> where T : EditorViewModelStateBase
{
    private T _state = default!;

    /// <summary>
    /// Triggered when the user has saved any pending changes.
    /// </summary>
    public event EventHandler? StateSaved;

    /// <summary>
    /// Triggered when the user saves any pending changes (right before
    /// <see cref="OnSaveChanges"/> is invoked).
    /// </summary>
    public event EventHandler? StateSaving;

    /// <inheritdoc/>
    public T State
    { 
        get => _state;
        set => Change(ref _state, value);
    }

    /// <summary>
    /// Gets a reference to the command used to save all pending changes and
    /// close the dialog.
    /// </summary>
    public ICommand SaveChangesCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to discard all changes and close
    /// the dialog.
    /// </summary>
    public ICommand DiscardChangesCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorViewModelBase{T}"/>
    /// class.
    /// </summary>
    protected EditorViewModelBase(T state)
    {
        _state = state;
        SaveChangesCommand = ObservingCommandBuilder.Create(state, InvokeSaveChanges)
            .ListensToCanExecute(p => p.UnsavedChanges)
            .Build();
        DiscardChangesCommand = new SimpleCommand(OnDiscardChanges);

        Interactions.Add(new(SaveChangesCommand, St.Save));
        Interactions.Add(new(DiscardChangesCommand, St.Discard));
    }

    private async Task InvokeSaveChanges()
    {
        StateSaving?.Invoke(this, EventArgs.Empty);
        await OnSaveChanges();
        StateSaved?.Invoke(this, EventArgs.Empty);
        OnDiscardChanges();
    }

    private void OnDiscardChanges()
    {
        State.UnsavedChanges = false;
        Close();
    }

    /// <summary>
    /// Executes an operation that will persist any pending changes on the
    /// state.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that can be used to await the async operation.
    /// </returns>
    protected abstract Task OnSaveChanges();
}
