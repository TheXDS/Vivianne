using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly new DependencyProperty ItemsSourceProperty;

    /// <summary>
    /// Gets or sets the list to be displayed and updated through this control.
    /// </summary>
    public new IList<double> ItemsSource
    {
        get => (IList<double>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    static DoubleCollectionEditor()
    {
        ItemsSourceProperty = NewDp<IList<double>, DoubleCollectionEditor>(nameof(ItemsSource), [], OnItemsSourceChanged);
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (DoubleCollectionEditor)d;

        void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
        {
            var nud = control.Items.Cast<NumberBox>().First(p => (int)p.Tag == e.NewStartingIndex);
            var value = (double?)e.NewItems?[0];
            if (nud.Value != value) nud.Value = value;
        }

        if (e.OldValue is ObservableCollection<double> oldC) oldC.CollectionChanged -= OnCollectionChanged;
        if (e.NewValue is ObservableCollection<double> c) c.CollectionChanged += OnCollectionChanged;
        
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
                if (control.ItemsSource[(int)nud.Tag] != nud.Value)
                {
                    control.ItemsSource[(int)nud.Tag] = nud.Value ?? 0.0;
                }
            };
            control.Items.Add(nud);
        }
    }
}
