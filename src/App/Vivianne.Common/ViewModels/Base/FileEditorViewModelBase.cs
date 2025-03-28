using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Component;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.FshEditorViewModel;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Base class for all file editor viewModels.
/// </summary>
/// <typeparam name="TState">Type of state to use.</typeparam>
/// <typeparam name="TFile">
/// Type of file for which this ViewModel is an editor for.
/// </typeparam>
public abstract class FileEditorViewModelBase<TState, TFile> : ViewModel, IViewModel, IFileEditorViewModel<TState, TFile>
    where TFile : notnull, new()
    where TState : NotifyPropertyChanged, IFileState<TFile>, new()
{
    private TState state = default!;

    /// <inheritdoc/>
    public ICommand SaveCommand { get; }

    /// <inheritdoc/>
    public ICommand SaveAsCommand { get; }

    /// <inheritdoc/>
    public ICommand SaveAndCloseCommand { get; }

    /// <inheritdoc/>
    public ICommand DiscardAndCloseCommand { get; }

    /// <inheritdoc/>
    public bool UnsavedChanges => State?.UnsavedChanges ?? false;

    /// <inheritdoc/>
    public TState State
    {
        get => state;
        set
        {
            var oldState = state;
            if (Change(ref state, value))
            {
                oldState?.Unsubscribe(OnUnsavedChanges);
                value?.Subscribe(OnUnsavedChanges);
            }
        }
    }

    private void OnUnsavedChanges(object instance, PropertyInfo property, PropertyChangeNotificationType notificationType)
    {
        if (property.Name != nameof(UnsavedChanges)) return;
        Notify(nameof(UnsavedChanges));
    }

    /// <inheritdoc/>
    public IBackingStore<TFile>? BackingStore { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileEditorViewModelBase{TState, TFile}"/>
    /// class.
    /// </summary>
    protected FileEditorViewModelBase()
    {
        var cb = CommandBuilder.For(this);
        DiscardAndCloseCommand = cb.BuildSimple(OnClose);
        SaveAsCommand = cb.BuildSimple(OnSaveAs);
        SaveCommand = cb.BuildObserving(OnSave).ListensToCanExecute(p => p.UnsavedChanges).Build();
        SaveAndCloseCommand = cb.BuildObserving(OnSaveAndClose).ListensToCanExecute(p => p.UnsavedChanges).Build();
    }

    /// <summary>
    /// Saves the file from the ViewModel state into the current backing store.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that can be used to await the async operation.
    /// </returns>
    protected virtual Task OnSave()
    {
        if (!BeforeSave()) return Task.CompletedTask;
        return BackingStore?.WriteAsync(State.File) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Saves the file from the ViewModel state as a new file into the current
    /// backing store.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that can be used to await the async operation.
    /// </returns>
    protected virtual Task OnSaveAs()
    {
        if (!BeforeSave()) return Task.CompletedTask;
        return BackingStore?.WriteNewAsync(State.File) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Saves the file from the ViewModel state into the current backing store
    /// and closes this editor.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that can be used to await the async operation.
    /// </returns>
    protected virtual async Task OnSaveAndClose()
    {
        if (BeforeSave() && await (BackingStore?.WriteAsync(State.File) ?? Task.FromResult(false)))
        {
            await OnClose();
        }
    }

    /// <summary>
    /// Invoked right before executing any save operation.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the save operation should continue normally,
    /// <see langword="false"/> to cancel the save operation.
    /// </returns>
    protected virtual bool BeforeSave() => true;

    private Task OnClose()
    {
        return NavigationService?.NavigateBack() ?? Task.CompletedTask;
    }

    async Task IViewModel.OnNavigateBack(CancelFlag navigation)
    {
        if (State.UnsavedChanges)
        {
            switch (await DialogService!.AskYnc(St.Unsaved, string.Format(St.SaveConfirm, Title)))
            {
                case true: SaveCommand.Execute(State); break;
                case null: navigation.Cancel(); break;
            }
        }
    }
}
