using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the current state of the Curve editor dialog.
/// </summary>
public class CurveEditorState : EditorViewModelStateBase
{
    private double _Minimum;
    private double _Maximum;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurveEditorState"/> class.
    /// </summary>
    /// <param name="targetCollection">
    /// Collection to be edited in the dialog.
    /// </param>
    public CurveEditorState(ICollection<double> targetCollection)
    {
        TargetCollection = targetCollection;
        Collection = [.. TargetCollection.Copy()];
        Collection.CollectionChanged += (sender, e) => UnsavedChanges = true;
        _Minimum = TargetCollection.Min();
        _Maximum = TargetCollection.Max();
    }

    /// <summary>
    /// Gets an in-memory copy of the collection to be edited.
    /// </summary>
    public ObservableCollection<double> Collection { get; }

    /// <summary>
    /// Gets the collection that will be altered upon completion of the dialog.
    /// </summary>
    public ICollection<double> TargetCollection { get; }

    /// <summary>
    /// Gets or sets the minimum value to be rendered on the dialog.
    /// </summary>
    public double Minimum
    {
        get => _Minimum;
        set => Change(ref _Minimum, value);
    }

    /// <summary>
    /// Gets or sets the maximum value to be rendered on the dialog.
    /// </summary>
    public double Maximum
    {
        get => _Maximum;
        set => Change(ref _Maximum, value);
    }
}
