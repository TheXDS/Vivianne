using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Controls;
using Fce3Color = TheXDS.Vivianne.Models.Fce.Nfs3.FceColor;
using Fce4Color = TheXDS.Vivianne.Models.Fce.Nfs4.FceColor;

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
    /// Identifies the "<c>BetaMessage</c>" attached property.
    /// </summary>
    public static readonly DependencyProperty BetaMessageProperty =
        DependencyProperty.RegisterAttached("BetaMessage", typeof(string), typeof(Form), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnBetaMessageChanged));

    /// <summary>
    /// Identifies the "<c>BetaMessage</c>" attached property.
    /// </summary>
    public static readonly DependencyProperty ImportantMessageProperty =
        DependencyProperty.RegisterAttached("ImportantMessage", typeof(string), typeof(Form), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnImportantMessageChanged));

    /// <summary>
    /// Identifies the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    public static readonly DependencyProperty FceColorPreviewProperty =
        DependencyProperty.RegisterAttached("FceColorPreview", typeof(Fce3Color), typeof(Form), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnFceColorPreviewChanged));

    /// <summary>
    /// Identifies the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    public static readonly DependencyProperty Fce4ColorPreviewProperty =
        DependencyProperty.RegisterAttached("Fce4ColorPreview", typeof(Fce4Color), typeof(Form), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnFce4ColorPreviewChanged));

    /// <summary>
    /// Identifies the "<c>MouseTrackingOverlay</c>" attached property.
    /// </summary>
    public static readonly DependencyProperty MouseTrackingOverlayProperty =
        DependencyProperty.RegisterAttached("MouseTrackingOverlay", typeof(bool), typeof(Form), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender, OnMouseTrackingOverlayChanged));

    /// <summary>
    /// Identifies the "<c>BetaIndicator</c>" attached property.
    /// </summary>
    public static readonly DependencyProperty BetaIndicatorProperty =
        DependencyProperty.RegisterAttached("BetaIndicator", typeof(bool), typeof(Form), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender, OnBetaIndicatorChanged));

    /// <summary>
    /// Identifies the "<c>DoubleClickCommand</c>" attached property.
    /// </summary>
    public static readonly DependencyProperty DoubleClickCommandProperty =
        DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(Form), new FrameworkPropertyMetadata(null, OnDoubleClickChanged));

    /// <summary>
    /// Gets the command that is invoked when a double-click event occurs on the specified dependency object.
    /// </summary>
    /// <param name="obj">The dependency object from which to retrieve the double-click command. Typically a UI element.</param>
    /// <returns>The command associated with the double-click event for the specified object, or null if no command is set.</returns>
    public static ICommand GetDoubleClickCommand(DependencyObject obj)
    {
        return (ICommand)obj.GetValue(DoubleClickCommandProperty);
    }

    /// <summary>
    /// Sets the command to be invoked when a double-click event occurs on the specified dependency object.
    /// </summary>
    /// <param name="obj">The dependency object on which to set the double-click command. Cannot be null.</param>
    /// <param name="value">The command to execute when a double-click is detected. Can be null to clear the command.</param>
    public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
    {
        obj.SetValue(DoubleClickCommandProperty, value);
    }

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
    /// Gets the value of the "<c>BetaMessage</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object for which to get the value of the attached property.
    /// </param>
    /// <returns>The value of the "<c>BetaMessage</c>" attached property.
    /// </returns>
    public static string GetBetaMessage(DependencyObject obj)
    {
        return (string)obj.GetValue(BetaMessageProperty);
    }

    /// <summary>
    /// Sets the value of the "<c>BetaMessage</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object onto which to set the value of the attached property.
    /// </param>
    /// <param name="value">Value of the attached property.</param>
    public static void SetBetaMessage(DependencyObject obj, string value)
    {
        obj.SetValue(BetaMessageProperty, value);
    }

    /// <summary>
    /// Gets the value of the "<c>BetaMessage</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object for which to get the value of the attached property.
    /// </param>
    /// <returns>The value of the "<c>BetaMessage</c>" attached property.
    /// </returns>
    public static string GetImportantMessage(DependencyObject obj)
    {
        return (string)obj.GetValue(ImportantMessageProperty);
    }

    /// <summary>
    /// Sets the value of the "<c>BetaMessage</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object onto which to set the value of the attached property.
    /// </param>
    /// <param name="value">Value of the attached property.</param>
    public static void SetImportantMessage(DependencyObject obj, string value)
    {
        obj.SetValue(ImportantMessageProperty, value);
    }

    /// <summary>
    /// Gets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object for which to get the value of the attached property.
    /// </param>
    /// <returns>The value of the "<c>FceColorPreview</c>" attached property.
    /// </returns>
    public static Fce3Color GetFceColorPreview(DependencyObject obj)
    {
        return (Fce3Color)obj.GetValue(FceColorPreviewProperty);
    }

    /// <summary>
    /// Sets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object onto which to set the value of the attached property.
    /// </param>
    /// <param name="value">Value of the attached property.</param>
    public static void SetFceColorPreview(DependencyObject obj, Fce3Color value)
    {
        obj.SetValue(FceColorPreviewProperty, value);
    }

    /// <summary>
    /// Gets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object for which to get the value of the attached property.
    /// </param>
    /// <returns>The value of the "<c>FceColorPreview</c>" attached property.
    /// </returns>
    public static Fce4Color GetFce4ColorPreview(DependencyObject obj)
    {
        return (Fce4Color)obj.GetValue(Fce4ColorPreviewProperty);
    }

    /// <summary>
    /// Sets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object onto which to set the value of the attached property.
    /// </param>
    /// <param name="value">Value of the attached property.</param>
    public static void SetFce4ColorPreview(DependencyObject obj, Fce4Color value)
    {
        obj.SetValue(Fce4ColorPreviewProperty, value);
    }

    /// <summary>
    /// Gets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object for which to get the value of the attached property.
    /// </param>
    /// <returns>The value of the "<c>FceColorPreview</c>" attached property.
    /// </returns>
    public static bool GetMouseTrackingOverlay(DependencyObject obj)
    {
        return (bool)obj.GetValue(MouseTrackingOverlayProperty);
    }

    /// <summary>
    /// Sets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object onto which to set the value of the attached property.
    /// </param>
    /// <param name="value">Value of the attached property.</param>
    public static void SetMouseTrackingOverlay(DependencyObject obj, bool value)
    {
        obj.SetValue(MouseTrackingOverlayProperty, value);
    }

    /// <summary>
    /// Gets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object for which to get the value of the attached property.
    /// </param>
    /// <returns>The value of the "<c>FceColorPreview</c>" attached property.
    /// </returns>
    public static bool GetBetaIndicator(DependencyObject obj)
    {
        return (bool)obj.GetValue(BetaIndicatorProperty);
    }

    /// <summary>
    /// Sets the value of the "<c>FceColorPreview</c>" attached property.
    /// </summary>
    /// <param name="obj">
    /// Object onto which to set the value of the attached property.
    /// </param>
    /// <param name="value">Value of the attached property.</param>
    public static void SetBetaIndicator(DependencyObject obj, bool value)
    {
        obj.SetValue(BetaIndicatorProperty, value);
    }

    private static void OnMouseTrackingOverlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue) AttachAdorner<MouseTrackingAdorner, bool>(d, e, (control, _) => new MouseTrackingAdorner(control));
        else RemoveAdorner<MouseTrackingAdorner>((FrameworkElement)d);
    }

    private static void OnLabelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        AttachAdorner<FormLabelAdorner, string>(obj, e, (control, text) => new FormLabelAdorner((Control)control, text));
    }

    private static void OnBetaIndicatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue) AttachAdorner<BetaAdorner, bool>(d, e, (control, _) => new BetaAdorner(control));
        else RemoveAdorner<BetaAdorner>((FrameworkElement)d);
    }

    private static void OnDoubleClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        void Control_MouseDoubleClick(object sender, MouseButtonEventArgs e) => GetDoubleClickCommand(d)?.Execute(null);

        if (d is not Control control)
        {
#if DEBUG
            Debug.Print($"DoubleClickCommand can only be attached to Control-derived types. This is a {d?.GetType().Name ?? "<null>"}");
#endif
            return;
        }
        if (e.OldValue is ICommand)
        {
            control.MouseDoubleClick -= Control_MouseDoubleClick;
        }
        if (e.NewValue is ICommand)
        {
            control.MouseDoubleClick += Control_MouseDoubleClick;
        }
    }

    private static void OnBetaMessageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        AttachAdorner<WatermarkMessage, string>(obj, e, (control, text) => new WatermarkMessage(control, text));
    }

    private static void OnImportantMessageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        AttachAdorner<ImportantWatermarkMessage, string>(obj, e, (control, text) => new ImportantWatermarkMessage(control, text));
    }

    private static void OnFceColorPreviewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        AttachAdorner<Fce3ColorPreviewAdorner, Fce3Color?>(obj, e, (control, color) => new Fce3ColorPreviewAdorner((Control)control, color));
    }
    private static void OnFce4ColorPreviewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        AttachAdorner<Fce4ColorPreviewAdorner, Fce4Color?>(obj, e, (control, color) => new Fce4ColorPreviewAdorner((Control)control, color));
    }

    private static void AttachAdorner<TAdorner, TValue>(DependencyObject obj, DependencyPropertyChangedEventArgs e, Func<FrameworkElement, TValue, TAdorner> adornerFactory) where TAdorner : Adorner
    {
        if (obj is not FrameworkElement control) return;

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

    private static void AddAdorner<TAdorner>(Visual control, TAdorner adorner) where TAdorner : Adorner
    {
        if (!(AdornerLayer.GetAdornerLayer(control) is { } adornerLayer)) return;
        adornerLayer.Add(adorner);
    }

    private static void RemoveAdorner<TAdorner>(UIElement control) where TAdorner : Adorner
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
