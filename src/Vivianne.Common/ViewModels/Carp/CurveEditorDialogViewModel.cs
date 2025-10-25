using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.MCART.Component;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Carp;

/// <summary>
/// Implements a ViewModel for a dialog that allows the user to edit a
/// collection of <see cref="double"/> values.
/// </summary>
public class CurveEditorDialogViewModel : EditorViewModelBase<CurveEditorState>
{
    /// <summary>
    /// Gets a reference to the command used to add a new index to the
    /// collection.
    /// </summary>
    public ICommand AddValueCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to remove a index from the
    /// collection.
    /// </summary>
    public ICommand RemoveValueCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to copy the collection as a string
    /// to the clopboard.
    /// </summary>
    public ICommand CopyToStringCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to create a new collection of
    /// values from a string representation of the collection.
    /// </summary>
    public ICommand CreateFromStringCommand { get; }

    /// <summary>
    /// Gets a value that indicates if the ViewModel allows growing or
    /// shrinking the collection, that is, adding or removing items.
    /// </summary>
    public bool AllowCollectionGrow { get; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="CurveEditorDialogViewModel"/> class.
    /// </summary>
    /// <param name="state">Current state of the dialog.</param>
    /// <param name="allowCollectionGrow">
    /// Indicates if the ViewModel allows growing or shrinking the collection.
    /// </param>
    public CurveEditorDialogViewModel(CurveEditorState state, bool allowCollectionGrow) : base(state)
    {
        AllowCollectionGrow = allowCollectionGrow;
        AddValueCommand = new SimpleCommand(OnAddValue, allowCollectionGrow);
        RemoveValueCommand = new SimpleCommand(OnRemoveValue, allowCollectionGrow);
        CopyToStringCommand = new SimpleCommand(OnCopyToString);
        CreateFromStringCommand = new SimpleCommand(OnCreateFromString);
    }

    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        if (State.TargetCollection is IList<double> list)
        {
            foreach (var (index, element) in State.Collection.WithIndex())
            {
                list[index] = element;
            }
        }
        else
        {
            State.TargetCollection.Clear();
            State.TargetCollection.AddRange(State.Collection);
        }
        return Task.CompletedTask;
    }

    private async Task OnCreateFromString()
    {
        var newCollection = PlatformServices.OperatingSystem.ReadClipboardText()
            .Split(CultureInfo.CurrentCulture.TextInfo.ListSeparator)
            .Select(p => double.TryParse(p, out var r) ? r : 0.0).ToArray();
        if (!AllowCollectionGrow && State.Collection.Count != newCollection.Length)
        {
            await DialogService!.Error(string.Format("A collection of {0} elements was expected, but received {1}.", State.Collection.Count, newCollection.Length));
            return;
        }
        State.Collection.Clear();
        State.Collection.AddRange(newCollection);
    }

    private void OnCopyToString()
    {
        PlatformServices.OperatingSystem.WriteClipboardText(string.Join(
            CultureInfo.CurrentCulture.TextInfo.ListSeparator,
            State.Collection.Select(p => p.ToString(CultureInfo.InvariantCulture))
            ));
    }

    private async Task OnAddValue()
    {
        double value = 0.0;
        if (PlatformServices.Keyboard.IsCtrlKeyDown && await DialogService!.GetInputValue<int>(
            Dialogs.GetValue, null, null) is { Success: bool s, Result: int v })
        {
            if (!s) return;
            value = v;
        }

        if (PlatformServices.Keyboard.IsShiftKeyDown)
        {
            if (await DialogService!.GetInputValue(
                Dialogs.GetIndex, 0, State.Collection.Count, State.Collection.Count - 1) is { Success: true, Result: int index })
            {
                State.Collection.Insert(index, value);
            }
        }
        else
        {
            State.Collection.Add(value);
        }
    }

    private void OnRemoveValue(object? value)
    {
        if (value is not double d) return;
        State.Collection.Remove(d);
    }
}