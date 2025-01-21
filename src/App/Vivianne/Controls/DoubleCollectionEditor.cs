using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TheXDS.MCART.Helpers;
using Wpf.Ui.Controls;
using static TheXDS.MCART.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements an <see cref="ItemsControl"/> that displays an array of
/// <see cref="NumberBox"/> controls that allow the user to directly edit a
/// collection of <see cref="double"/> values.
/// </summary>
public class DoubleCollectionEditor : ItemsControl
{
    /// <summary>
    /// Identifies the <see cref="DataCollection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DataCollectionProperty;

    /// <summary>
    /// Gets or sets the list to be displayed and updated through this control.
    /// </summary>
    public IList<double> DataCollection
    {
        get => (IList<double>)GetValue(DataCollectionProperty);
        set => SetValue(DataCollectionProperty, value);
    }
    static DoubleCollectionEditor()
    {
        DataCollectionProperty = NewDp<IList<double>, DoubleCollectionEditor>(nameof(DataCollection), [], OnCollectionChanged);
    }

    private static void OnCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (DoubleCollectionEditor)d;
        if (e.NewValue is ObservableCollection<double> c)
        {
            c.CollectionChanged += (sender, e) =>
            {
                var nud = control.Items.Cast<NumberBox>().First(p => (int)p.Tag == e.NewStartingIndex);
                var value = (double?)e.NewItems?[0];
                if (nud.Value != value) nud.Value = value;
            };
        }
        control.Items.Clear();
        foreach ((var index, var value) in (e.NewValue as IEnumerable<double> ?? []).WithIndex())
        {
            var nud = new NumberBox
            {
                Value = value,
                Tag = index
            };
            nud.ValueChanged += (sender, e) =>
            {
                if (control.DataCollection[(int)nud.Tag] != nud.Value)
                {
                    control.DataCollection[(int)nud.Tag] = nud.Value ?? 0.0;
                }
            };
            control.Items.Add(nud);
        }
    }
}
