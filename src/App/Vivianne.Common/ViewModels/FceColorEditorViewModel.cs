using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Base;
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
            foreach (var j in e.OldItems.Cast<MutableFceColorItem>())
            {
                j.PrimaryColor.Unsubscribe(OnColorChanged);
                j.SecondaryColor.Unsubscribe(OnColorChanged);
            }
            foreach (var j in e.NewItems.Cast<MutableFceColorItem>())
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
    /// Gets a reference to the command used to remove an existing color.
    /// </summary>
    public SimpleCommand RemoveColorCommand { get; }

    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        var newColors = State.Colors.Select<MutableFceColorItem, (HsbColor Primary, HsbColor Secondary)>(p => (p.PrimaryColor.ToColor(), p.SecondaryColor.ToColor())).ToArray();
        for (int i = 0; i < State.Colors.Count; i++)
        {
            State.Fce.Colors[i].PrimaryColor = State.Fce.File.PrimaryColors[i] = newColors[i].Primary;
            State.Fce.Colors[i].SecondaryColor = State.Fce.File.SecondaryColors[i] = newColors[i].Secondary;
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

    private void UpdateCommands()
    {
        AddNewColorCommand.SetCanExecute(State.Colors.Count < 16);
    }
}