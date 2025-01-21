using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Controls;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Helpers;

/// <summary>
/// Includes a set of attached properties that add various kinds of adorners to
/// some controls.
/// </summary>
public static class Form
{
    /// <summary>
    /// Identifies the "<c>Label</c>" attached property.
    /// </summary>
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.RegisterAttached("Label", typeof(string), typeof(Form), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnLabelChanged));

    /// <summary>
    /// Identifies the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    public static readonly DependencyProperty FceColorPreviewProperty =
        DependencyProperty.RegisterAttached("FceColorPreview", typeof(FceColorItem), typeof(Form), new FrameworkPropertyMetadata(default(FceColorItem), FrameworkPropertyMetadataOptions.AffectsRender, OnFceColorPreviewChanged));

    /// <summary>
    /// Gets the value of the "<c>Label</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object for which to get the value of the attached property.
    /// </param>
    /// <returns>The value of the "<c>Label</c>" attached property.
    /// </returns>
    public static string GetLabel(DependencyObject obj)
    {
        return (string)obj.GetValue(LabelProperty);
    }

    /// <summary>
    /// Sets the value of the "<c>Label</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object onto which to set the value of the attached property.
    /// </param>
    /// <param name="value">Value of the attached property.</param>
    public static void SetLabel(DependencyObject obj, string value)
    {
        obj.SetValue(LabelProperty, value);
    }


    /// <summary>
    /// Gets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object for which to get the value of the attached property.
    /// </param>
    /// <returns>The value of the "<c>FceColorPreview</c>" attached property.
    /// </returns>
    public static FceColorItem GetFceColorPreview(DependencyObject obj)
    {
        return (FceColorItem)obj.GetValue(FceColorPreviewProperty);
    }

    /// <summary>
    /// Sets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object onto which to set the value of the attached property.
    /// </param>
    /// <param name="value">Value of the attached property.</param>
    public static void SetFceColorPreview(DependencyObject obj, FceColorItem value)
    {
        obj.SetValue(FceColorPreviewProperty, value);
    }

    private static void OnLabelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        AttachAdorner<FormLabelAdorner, string>(obj, e, (control, text) => new FormLabelAdorner(control, text));
    }

    private static void OnFceColorPreviewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        AttachAdorner<FceColorPreviewAdorner, FceColorItem>(obj, e, (control, color) => new FceColorPreviewAdorner(control, color));
    }

    private static void AttachAdorner<TAdorner, TValue>(DependencyObject obj, DependencyPropertyChangedEventArgs e, Func<Control, TValue, TAdorner> adornerFactory) where TAdorner : Adorner
    {
        if (obj is not Control control) return;

        void OnControlLoaded(object sender, RoutedEventArgs _)
        {
            if (!ReferenceEquals(sender, control)) return;
            AddAdorner(control, adornerFactory.Invoke(control, (TValue)e.NewValue));
        }
        void OnControlUnloaded(object sender, RoutedEventArgs _)
        {
            if (!ReferenceEquals(sender, control)) return;
            RemoveAdorner<TAdorner>(control);
        }

        if (!control.IsLoaded)
        {
            control.Loaded += OnControlLoaded;
            control.Unloaded += OnControlUnloaded;
        }
        else
        {
            RemoveAdorner<TAdorner>(control);
            if (e.NewValue is TValue text) AddAdorner(control, adornerFactory(control, text));
        }
    }

    private static void AddAdorner<TAdorner>(Control control, TAdorner adorner) where TAdorner : Adorner
    {
        if (!(AdornerLayer.GetAdornerLayer(control) is { } adornerLayer)) return;
        adornerLayer.Add(adorner);
    }

    private static void RemoveAdorner<TAdorner>(Control control) where TAdorner : Adorner
    {
        if (!(AdornerLayer.GetAdornerLayer(control) is { } adornerLayer)) return;
        foreach (var adorner in adornerLayer.GetAdorners(control).NotNull())
        {
            if (adorner is TAdorner)
            {
                adornerLayer.Remove(adorner);
            }
        }
    }
}
