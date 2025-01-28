using System.Windows;
using System.Windows.Controls;
using TheXDS.Vivianne.ViewModels.Base;
using static TheXDS.MCART.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements a simple control that serves as the view for a 
/// <see cref="IFileEditorViewModelLauncher"/>.
/// </summary>
public class FileEditorLauncherView : Control
{
    /// <summary>
    /// Identifies the <see cref="Launcher"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LauncherProperty;

    /// <summary>
    /// Initializes the <see cref="FileEditorLauncherView"/> class.
    /// </summary>
    static FileEditorLauncherView()
    {
        SetControlStyle<FileEditorLauncherView>(DefaultStyleKeyProperty);
        LauncherProperty = NewDp<IFileEditorViewModelLauncher, FileEditorLauncherView>(nameof(Launcher));
    }

    /// <summary>
    /// Gets or sets a reference to the Launcher ViewModel to bind this control
    /// to.
    /// </summary>
    public IFileEditorViewModelLauncher Launcher
    {
        get => (IFileEditorViewModelLauncher)GetValue(LauncherProperty);
        set => SetValue(LauncherProperty, value);
    }
}
