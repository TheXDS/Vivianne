using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheXDS.MCART.Component;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs3;

/// <summary>
/// ViewModel that allows the user to edit the color table in an FCE file.
/// </summary>
public class Fce3ColorEditorViewModel : EditorViewModelBase<Fce3ColorTableEditorState>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Fce3ColorEditorViewModel"/>
    /// class.
    /// </summary>
    /// <param name="state">State to associate with this ViewModel.</param>
    public Fce3ColorEditorViewModel(Fce3ColorTableEditorState state) : base(state)
    {
        AddNewColorCommand = new SimpleCommand(OnAddNewColor);
        RemoveColorCommand = new SimpleCommand(OnRemoveColor);
        CopyColorCommand = new SimpleCommand(OnCopyColor);
        state.Colors.CollectionChanged += (sende, e) =>
        {
            foreach (var j in e.OldItems.NotNull().Cast<MutableFceColorItem>())
            {
                j.PrimaryColor.Unsubscribe(OnColorChanged);
                j.SecondaryColor.Unsubscribe(OnColorChanged);
            }
            foreach (var j in e.NewItems.NotNull().Cast<MutableFceColorItem>())
            {
                j.PrimaryColor.Subscribe(OnColorChanged);
                j.SecondaryColor.Subscribe(OnColorChanged);
            }

            State.UnsavedChanges = true;
            UpdateCommands();
        };
        UpdateCommands();
    }

    private void OnColorChanged(object instance, PropertyInfo property, PropertyChangeNotificationType notificationType)
    {
        State.UnsavedChanges = true;
    }

    /// <summary>
    /// Gets a reference to the command used to add a new color.
    /// </summary>
    public SimpleCommand AddNewColorCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to copy an existing color.
    /// </summary>
    public SimpleCommand CopyColorCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to remove an existing color.
    /// </summary>
    public SimpleCommand RemoveColorCommand { get; }

    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        var newColors = State.Colors.Select<MutableFceColorItem, (HsbColor Primary, HsbColor Secondary)>(p => (p.PrimaryColor.ToColor3(), p.SecondaryColor.ToColor3())).ToArray();
        State.Fce.Colors.Clear();
        State.Fce.File.PrimaryColors.Clear();
        State.Fce.File.SecondaryColors.Clear();
        for(int i = 0; i < State.Colors.Count; i++)
        {
            State.Fce.Colors.Add(new FceColor());
            State.Fce.File.PrimaryColors.Add(State.Fce.Colors[i].PrimaryColor = newColors[i].Primary);
            State.Fce.File.SecondaryColors.Add(State.Fce.Colors[i].SecondaryColor = newColors[i].Secondary);
        }
        State.Fce.UnsavedChanges = true;
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

    private void OnCopyColor(object? parameter)
    {
        if (parameter is not MutableFceColorItem fc) return;
        State.AddColor(new MutableFceColorItem(fc.PrimaryColor.ShallowClone(), fc.SecondaryColor.ShallowClone()));
    }

    private void UpdateCommands()
    {
        var enable = State.Colors.Count < 16;
        Message = $"Number of colors: {State.Colors.Count}/16";
        AddNewColorCommand.SetCanExecute(enable);
        CopyColorCommand.SetCanExecute(enable);
    }
}