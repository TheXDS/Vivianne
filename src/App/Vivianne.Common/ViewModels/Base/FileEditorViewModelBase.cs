using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Types.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.FshEditorViewModel;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Base class for all file editor viewModels.
/// </summary>
/// <typeparam name="TState">Type of state to use.</typeparam>
/// <typeparam name="TFile">
/// Type of file for which this ViewModel is an editor for.
/// </typeparam>
public abstract class FileEditorViewModelBase<TState, TFile> : ViewModel, IViewModel, IFileEditorViewModel<TState, TFile> where TState : class, IFileState<TFile>
{
    private ICommand saveCommand = null!;
    private ICommand? saveAsCommand;
    private TState state = null!;

    /// <inheritdoc/>
    public ICommand SaveCommand
    {
        get => saveCommand;
        set => Change(ref saveCommand, value);
    }

    /// <inheritdoc/>
    public ICommand? SaveAsCommand
    {
        get => saveAsCommand;
        set => Change(ref saveAsCommand, value);
    }

    /// <inheritdoc/>
    public ICommand CloseCommand { get; }

    /// <inheritdoc/>
    public TState State
    {
        get => state;
        set => Change(ref state, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileEditorViewModelBase{TState, TFile}"/>
    /// class.
    /// </summary>
    protected FileEditorViewModelBase()
    {
        var cb = CommandBuilder.For(this);
        CloseCommand = cb.BuildSimple(OnClose);
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
