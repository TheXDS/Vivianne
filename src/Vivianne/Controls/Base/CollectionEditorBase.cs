using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using TheXDS.MCART.Helpers;
using Wpf.Ui.Controls;
using static TheXDS.MCART.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls.Base;

/// <summary>
/// Base class for all controls that implement a primitive numeric collection
/// editor.
/// </summary>
/// <typeparam name="T">
/// Type of numeric collections to edit using the control.
/// </typeparam>
public class CollectionEditorBase<T> : ItemsControl where T : unmanaged, IConvertible
{
    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly new DependencyProperty ItemsSourceProperty;

    /// <summary>
    /// Gets or sets the list to be displayed and updated through this control.
    /// </summary>
    public new IList<T> ItemsSource
    {
        get => (IList<T>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    static CollectionEditorBase()
    {
        ItemsSourceProperty = NewDp<IList<T>, CollectionEditorBase<T>>(nameof(ItemsSource), [], OnItemsSourceChanged);
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CollectionEditorBase<T>)d;
        if (e.OldValue is ObservableCollection<T> oldC) oldC.CollectionChanged -= control.OnCollectionChanged;
        if (e.NewValue is ObservableCollection<T> c) c.CollectionChanged += control.OnCollectionChanged;
        control.Items.Clear();
        foreach ((var index, var value) in (e.NewValue as IEnumerable<T> ?? []).WithIndex())
        {
            control.Items.Add(control.CreateNumberBox(index, value.ToDouble(null)));
        }
    }

    private void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset) return;
        if (e.OldStartingIndex >= 0 && e.OldStartingIndex != e.NewStartingIndex)
        {
            Items.RemoveAt(e.OldStartingIndex);
        }
        var value = e.NewItems?[0] is IConvertible c ? c.ToDouble(null) : 0.0;
        if (Items.Cast<NumberBox>().FirstOrDefault(p => (int)p.Tag == e.NewStartingIndex) is { } nud)
        {
            if (nud.Value != value) nud.Value = value;
        }
        else if (e.NewStartingIndex >= 0)
        {
            Items.Insert(e.NewStartingIndex, CreateNumberBox(e.NewStartingIndex, value));
        }
    }

    private NumberBox CreateNumberBox(int index, double value)
    {
        var nud = new NumberBox
        {
            Value = value,
            Tag = index
        };
        nud.ValueChanged += (sender, e) =>
        {
            if (ItemsSource[(int)nud.Tag].ToDouble(null) != nud.Value)
            {
                ItemsSource[(int)nud.Tag] = (T)((IConvertible)(nud.Value ?? 0.0)).ToType(typeof(T), null);
            }
        };
        return nud;
    }
}
