using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Controls;

namespace TheXDS.Vivianne.Helpers;

public static class Form
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.RegisterAttached("Label", typeof(string), typeof(Form), new PropertyMetadata(null, OnLabelChanged));

    public static string GetLabel(DependencyObject obj)
    {
        return (string)obj.GetValue(LabelProperty);
    }

    public static void SetLabel(DependencyObject obj, string value)
    {
        obj.SetValue(LabelProperty, value);
    }

    private static void OnLabelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is not Control control) return;

        void OnTextBoxLoaded(object sender, RoutedEventArgs _)
        {
            if (!ReferenceEquals(sender, control)) return;
            AddAdorner(control, (string)e.NewValue);
        }
        void OnTextBoxUnloaded(object sender, RoutedEventArgs _)
        {
            if (!ReferenceEquals(sender, control)) return;
            RemoveAdorner(control);
        }

        if (!control.IsLoaded)
        {
            control.Loaded += OnTextBoxLoaded;
            control.Unloaded += OnTextBoxUnloaded;
        }
        else
        {
            RemoveAdorner(control);
            if (e.NewValue is string text) AddAdorner(control, text);
        }
    }

    private static void AddAdorner(Control control, string text)
    {
        if (!(AdornerLayer.GetAdornerLayer(control) is { } adornerLayer)) return;
        var adorner = new FormLabelAdorner(control, text);
        control.ToolTip ??= text;
        adornerLayer.Add(adorner);
    }

    private static void RemoveAdorner(Control control)
    {
        if (!(AdornerLayer.GetAdornerLayer(control) is { } adornerLayer)) return;
        foreach (var adorner in adornerLayer.GetAdorners(control).NotNull())
        {
            if (adorner is FormLabelAdorner)
            {
                adornerLayer.Remove(adorner);
            }
        }
    }
}
