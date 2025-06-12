using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Services;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Serializers;
using St = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Base class for all ViewModels that can be used to launch file editors from
/// the startup page.
/// </summary>
/// <typeparam name="TState">Type of state represented inside the ViewModel.</typeparam>
/// <typeparam name="TFile">Type of file on which the state is based on.</typeparam>
/// <typeparam name="TSerializer">Type of serializer that can be used to read and write file data.</typeparam>
/// <typeparam name="TEditor">Type of editor to be launched upon invocation.</typeparam>
public abstract class FileEditorViewModelLauncher<TState, TFile, TSerializer, TEditor> : FileViewModelLauncherBase<TFile, TSerializer, TEditor>, IFileEditorViewModelLauncher
    where TFile : notnull, new()
    where TState : IFileState<TFile>, new()
    where TSerializer : ISerializer<TFile>, new()
    where TEditor : IFileEditorViewModel<TState, TFile>, new()
{
    private readonly IEnumerable<FileFilterItem> _saveFilter;
    private readonly Func<IDialogService> _dialogSvc;

    /// <inheritdoc/>
    public ICommand NewFileCommand { get; }

    /// <inheritdoc/>
    public bool CanCreateNew { get; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="FileEditorViewModelLauncher{TState, TFile, TSerializer, TEditor}"/>
    /// class.
    /// </summary>
    /// <param name="dialogSvc">Dialog service to consume for dialogs.</param>
    /// <param name="pageName">Display name for the page.</param>
    /// <param name="openFilter">File filter to use when opening files.</param>
    /// <param name="saveFilter">File filter to use when saving files.</param>
    /// <param name="canCreateNew">
    /// If omitted or set to <see langword="true"/>, the "New" command will be
    /// enabled and available. If set to <see langword="false"/>, the "New"
    /// command will be disabled.
    /// </param>
    protected FileEditorViewModelLauncher(Func<IDialogService> dialogSvc, string pageName, IEnumerable<FileFilterItem> openFilter, IEnumerable<FileFilterItem> saveFilter, bool canCreateNew = true)
        : base(pageName, openFilter)
    {
        _dialogSvc = dialogSvc;
        _saveFilter = saveFilter;
        CanCreateNew = canCreateNew;
        NewFileCommand = new SimpleCommand(OnNew, canCreateNew);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="FileEditorViewModelLauncher{TState, TFile, TSerializer, TEditor}"/>
    /// class.
    /// </summary>
    /// <param name="dialogSvc">Dialog service to consume for dialogs.</param>
    /// <param name="pageName">Display name for the page.</param>
    /// <param name="filter">
    /// File filter to use when opening or saving files.
    /// </param>
    /// <param name="canCreateNew">
    /// If omitted or set to <see langword="true"/>, the "New" command will be
    /// enabled and available. If set to <see langword="false"/>, the "New"
    /// command will be disabled.
    /// </param>
    protected FileEditorViewModelLauncher(Func<IDialogService> dialogSvc, string pageName, IEnumerable<FileFilterItem> filter, bool canCreateNew = true) : this(dialogSvc, pageName, filter, filter, canCreateNew)
    {
    }

    /// <inheritdoc/>
    protected override TEditor CreateViewModel(string? friendlyName, TFile file, string filePath)
    {
        return new TEditor()
        {
            Title = friendlyName,
            State = new TState { File = file },
            BackingStore = new BackingStore<TFile, TSerializer>(new FileSystemBackingStore(_dialogSvc.Invoke(), _saveFilter, Path.GetDirectoryName(filePath) ?? Environment.CurrentDirectory)) { FileName = filePath },
        };
    }

    private void OnNew(object? parameter)
    {
        var vm = new TEditor()
        {
            Title = St.NewFile,
            State = new TState
            {
                File = new TFile()
            },
            BackingStore = new BackingStore<TFile, TSerializer>(new FileSystemBackingStore(_dialogSvc.Invoke(), _saveFilter, Environment.CurrentDirectory)),
        };
        NavigationService!.Navigate(vm);
    }
}
