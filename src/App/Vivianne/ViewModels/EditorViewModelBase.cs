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
public abstract class EditorViewModelBase<T> : DialogViewModel, IStatefulViewModel<T>, IAwaitableDialogViewModel where T : EditorViewModelStateBase
{
    private T _state = default!;
    private readonly TaskCompletionSource dlgAwaiter = new();

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

    /// <inheritdoc/>
    public Task DialogAwaiter => dlgAwaiter.Task;

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
        DiscardChangesCommand = new SimpleCommand(dlgAwaiter.SetResult);

        Interactions.Add(new(SaveChangesCommand, St.Save));
        Interactions.Add(new(DiscardChangesCommand, St.Discard));
    }

    private async Task InvokeSaveChanges()
    {
        await OnSaveChanges();
        dlgAwaiter.SetResult();
    }

    /// <summary>
    /// Executes an operation that will persist any pending changes on the
    /// state.
    /// </summary>
    /// <returns></returns>
    protected abstract Task OnSaveChanges();
}
