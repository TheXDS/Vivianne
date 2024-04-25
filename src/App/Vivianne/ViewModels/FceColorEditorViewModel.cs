using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Models;

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
        var newHeader = State.Fce.Header;
        var newColors = State.Colors.Select<FceColorItem,(FceColor Primary, FceColor Secondary)>(p => (p.PrimaryColor.ToColor(), p.SecondaryColor.ToColor())).ToArray();
        newHeader.PrimaryColors = newColors.Length;
        newHeader.SecondaryColors = newColors.Length;
        for (int i = 0; i < State.Colors.Count; i++)
        {
            newHeader.PrimaryColorTable[i] = newColors[i].Primary;
            newHeader.SecondaryColorTable[i] = newColors[i].Secondary;
        }
        State.Fce.Header = newHeader;
        return Task.CompletedTask;
    }

    private void OnAddNewColor()
    {
        State.Colors.Add(new FceColorItem() { PrimaryColor = new(), SecondaryColor = new() });
        State.UnsavedChanges = true;
        UpdateCommands();
    }

    private void OnRemoveColor(object? parameter)
    {
        if (parameter is not FceColorItem fc) return;
        State.Colors.Remove(fc);
        State.UnsavedChanges = true;
        UpdateCommands();
    }

    private void UpdateCommands()
    {
        AddNewColorCommand.SetCanExecute(State.Colors.Count < 16);
    }
}