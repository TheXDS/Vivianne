using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Base;
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
    where TState : INotifyPropertyChanged, IFileState<TFile>, new()
{
    private TState state = default!;

    /// <inheritdoc/>
    public ICommand SaveCommand { get; }

    /// <inheritdoc/>
    public ICommand SaveAsCommand { get; }

    /// <inheritdoc/>
    public ICommand CloseCommand { get; }

    /// <inheritdoc/>
    public TState State
    {
        get => state;
        set => Change(ref state, value);
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
        CloseCommand = cb.BuildSimple(OnClose);
        SaveAsCommand = cb.BuildSimple(OnSaveAs);
        SaveCommand = cb.BuildSimple(OnSave);
    }

    protected virtual Task OnSave()
    {
        return BackingStore?.WriteAsync(State.File) ?? Task.CompletedTask;
    }

    protected virtual Task OnSaveAs()
    {
        return BackingStore?.WriteNewAsync(State.File) ?? Task.CompletedTask;
    }

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
