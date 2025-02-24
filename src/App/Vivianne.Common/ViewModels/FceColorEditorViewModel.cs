using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fce;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// ViewModel that allows the user to edit the color table in an FCE file.
/// </summary>
public class FceColorEditorViewModel : EditorViewModelBase<FceColorTableEditorState>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FceColorEditorViewModel"/>
    /// class.
    /// </summary>
    /// <param name="state">State to associate with this ViewModel.</param>
    public FceColorEditorViewModel(FceColorTableEditorState state) : base(state)
    {
        AddNewColorCommand = new SimpleCommand(OnAddNewColor);
        RemoveColorCommand = new SimpleCommand(OnRemoveColor);
        state.Colors.CollectionChanged += (sende, e) =>
        {
            State.UnsavedChanges = true;
            UpdateCommands();
        };
        UpdateCommands();
    }

    /// <summary>
    /// Gets a reference to the command used to add a new color.
    /// </summary>
    public SimpleCommand AddNewColorCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to remove an existing color.
    /// </summary>
    public SimpleCommand RemoveColorCommand { get; }

    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        var newColors = State.Colors.Select<MutableFceColorItem, (HsbColor Primary, HsbColor Secondary)>(p => (p.PrimaryColor.ToColor(), p.SecondaryColor.ToColor())).ToArray();
        for (int i = 0; i < State.Colors.Count; i++)
        {
            State.Fce.PrimaryColors[i] = newColors[i].Primary;
            State.Fce.SecondaryColors[i] = newColors[i].Secondary;
        }
        return Task.CompletedTask;
    }

    private void OnAddNewColor()
    {
        State.AddColor(new MutableFceColorItem(new(), new()));
    }

    private void OnRemoveColor(object? parameter)
    {
        if (parameter is not MutableFceColorItem fc) return;
        State.Colors.Remove(fc);
    }

    private void UpdateCommands()
    {
        AddNewColorCommand.SetCanExecute(State.Colors.Count < 16);
    }
}