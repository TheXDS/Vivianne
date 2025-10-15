using System.Windows;
using System.Windows.Controls;
using TheXDS.Vivianne.ViewModels.Base;
using static TheXDS.MCART.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Control that exposes commands to save or discard changes made to a file being edited.
/// </summary>
public class FileEditorCommandBar : ContentControl
{
    /// <summary>
    /// Identifies the <see cref="Editor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EditorProperty;

    /// <summary>
    /// Identifies the <see cref="ShowClose"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowCloseProperty;

    /// <summary>
    /// Identifies the <see cref="IsReadOnly"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty;

    static FileEditorCommandBar()
    {
        SetControlStyle<FileEditorCommandBar>(DefaultStyleKeyProperty);
        EditorProperty = NewDp<IFileEditorViewModel, FileEditorCommandBar>(nameof(Editor));
        ShowCloseProperty = NewDp<bool, FileEditorCommandBar>(nameof(ShowClose), true);
        IsReadOnlyProperty = NewDp<bool, FileEditorCommandBar>(nameof(IsReadOnly), false);
    }

    /// <summary>
    /// Gets or sets a reference to the linked
    /// <see cref="IFileEditorViewModel"/> instance.
    /// </summary>
    public IFileEditorViewModel? Editor
    {
        get => (IFileEditorViewModel)GetValue(EditorProperty) ?? DataContext as IFileEditorViewModel;
        set => SetValue(EditorProperty, value);
    }

    /// <summary>
    /// Gets or seta a value that indicates if the close button should be visible.
    /// </summary>
    public bool ShowClose
    {
        get => (bool)GetValue(ShowCloseProperty);
        set => SetValue(ShowCloseProperty, value);
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }
}