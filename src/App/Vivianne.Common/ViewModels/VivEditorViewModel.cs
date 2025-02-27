using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Ganymede.ViewModels;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Data;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.VivEditorViewModel;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// ViewModel that serves as the main view for interacting with a VIV file.
/// </summary>
public class VivEditorViewModel : HostViewModelBase, IFileEditorViewModel<VivEditorState, VivFile>
{
    private static readonly Dictionary<string, ContentVisualizerViewModelFactory> ContentVisualizers = new(ContentVisualizerConfiguration.Get());
    private static readonly Dictionary<string, (string, Func<byte[]>)[]> Templates = new(VivTemplates.Get());
    private VivEditorState state;

    /// <inheritdoc/>
    public VivEditorState State
    {
        get => state;
        set
        {
            var oldState = state;
            if (Change(ref state, value))
            {
                oldState?.Unsubscribe(() => oldState.UnsavedChanges);
                value?.Subscribe(() => value.UnsavedChanges, OnUnsavedChanges);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VivEditorViewModel"/> class.
    /// </summary>
    public VivEditorViewModel()
    {
        var cb = CommandBuilder.For(this);
        OpenFileCommand = cb.BuildSimple(OnOpenFile);
        ImportFileCommand = cb.BuildSimple(OnImportFile);
        ReplaceFileCommand = cb.BuildSimple(OnReplaceFile);
        ExportFileCommand = cb.BuildSimple(OnExportFile);
        RemoveFileCommand = cb.BuildSimple(OnRemoveFile);
        RenameFileCommand = cb.BuildSimple(OnRenameFile);
        NewFromTemplateCommand = cb.BuildSimple(OnNewFromTemplate);
        DiscardAndCloseCommand = cb.BuildSimple(OnClose);
        SaveCommand = cb.BuildSimple(OnSave);
        SaveAsCommand = cb.BuildSimple(OnSaveAs);
        SaveAndCloseCommand = cb.BuildObserving(OnSaveAndClose).ListensToCanExecute(p => p.UnsavedChanges).Build();
    }

    /// <summary>
    /// Gets a reference to the command used to visualize the selected file
    /// from the VIV.
    /// </summary>
    public ICommand OpenFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to import a new file to the VIV
    /// directory.
    /// </summary>
    public ICommand ImportFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to create a new file inside the
    /// VIV directory based on a template.
    /// </summary>
    public ICommand NewFromTemplateCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to export a selected file from the
    /// VIV directory.
    /// </summary>
    public ICommand ExportFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to replace a file on the VIV
    /// directory for a new one.
    /// </summary>
    public ICommand ReplaceFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to remove a file from the VIV
    /// directory.
    /// </summary>
    public ICommand RemoveFileCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to rename a file inside the VIV
    /// directory.
    /// </summary>
    public ICommand RenameFileCommand { get; }

    /// <inheritdoc/>
    public ICommand SaveCommand { get; }

    /// <inheritdoc/>
    public ICommand? SaveAsCommand { get; }

    /// <inheritdoc/>
    public ICommand SaveAndCloseCommand { get; }

    /// <inheritdoc/>
    public ICommand DiscardAndCloseCommand { get; }

    /// <inheritdoc/>
    public bool UnsavedChanges => State?.UnsavedChanges ?? false;

    /// <inheritdoc/>
    public IBackingStore<VivFile>? BackingStore { get; init; }

    private void OnOpenFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file, Value: { } rawData })
        {
            IViewModel? vm = null;
            foreach (var j in ContentVisualizers.Where(p => file.EndsWith(p.Key, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (j.Value is { } factory && factory.Invoke(rawData, this, file) is { } visualizer)
                {
                    vm = visualizer;
                    break;
                }
            }
            vm ??= new FileErrorViewModel();
            vm.Title = file;
            ChildNavService!.NavigateAndReset(vm);
        }
        else
        {
            ChildNavService!.NavigateAndReset<VivInfoViewModel, VivEditorState>(State);
        }
    }

    private async Task OnImportFile()
    {
        var r = await DialogService!.GetFileOpenPath(CommonDialogTemplates.FileOpen with { Title = St.ImportFile }, Resources.FileFilters.AnyVivContentFilter);
        if (r.Success)
        {
            var keyName = Path.GetFileName(r.Result).ToLower();
            if (State.Directory.ContainsKey(keyName) && !await DialogService.AskYn(St.ReplaceFile, string.Format(St.TheFileXAlreadyExist, keyName)))
            {
                return;
            }
            State.Directory[keyName] = await DialogService.RunOperation(p => File.ReadAllBytesAsync(r.Result));
        }
    }

    private async Task OnExportFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file, Value: { } rawData })
        {
            var ext = Path.GetExtension(file)[1..];
            var r = await DialogService!.GetFileSavePath([FileFilterItem.Simple(ext), FileFilterItem.AllFiles], file);
            if (r.Success)
            {
                File.WriteAllBytes(r.Result, rawData);
            }
        }
    }

    private async Task OnReplaceFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file, Value: { } rawData })
        {
            var ext = Path.GetExtension(file)[1..];
            var r = await DialogService!.GetFileOpenPath(CommonDialogTemplates.FileOpen with
            {
                Title = string.Format(St.ReplaceX, file),
                Text = string.Format(St.SelectAFileToRepaceXWith, file)
            },
                [FileFilterItem.Simple(ext), FileFilterItem.AllFiles]);
            if (r.Success)
            {
                State.Directory[Path.GetFileName(r.Result).ToLower()] = await DialogService.RunOperation(p => File.ReadAllBytesAsync(r.Result));
            }
        }
    }

    private async Task OnRemoveFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file })
        {
            if (await DialogService!.AskYn(string.Format(St.RemoveX, file), string.Format(St.AreYouSureYouWantToRemoveX, file)))
            {
                State.Directory.Remove(file);
            }
        }
    }

    private async Task OnRenameFile(object? parameter)
    {
        if (parameter is not KeyValuePair<string, byte[]> { Key: { } fileName, Value: { } file }) return;
        var result = await DialogService.GetInputText(CommonDialogTemplates.Input with { Title = "St.RenamePart", Text = "St.RenamePartHelp" }, fileName);
        if (result.Success)
        {
            State.Directory.Remove(fileName);
            State.Directory.Add(result.Result, file);
        }
    }

    private async Task OnNewFromTemplate()
    {
        var r = await DialogService!.SelectOption(new DialogTemplate
        {
            Title = St.NewFromTemplate,
            Text = St.SelectATemplate,
            Color = System.Drawing.Color.Aquamarine,
            Icon = "➕"
        }, [.. Templates.Keys]);
        if (r.Success)
        {
            var template = Templates.ToList()[r.Result];
            foreach ((var filename, var factory) in template.Value)
            {
                State.Directory[filename] = factory.Invoke();
            }
        }
    }

    private Task OnSave()
    {
        return BackingStore?.WriteAsync(State.File) ?? Task.CompletedTask;
    }

    private Task OnSaveAs()
    {
        return BackingStore?.WriteNewAsync(State.File) ?? Task.CompletedTask;
    }

    protected virtual async Task OnSaveAndClose()
    {
        await OnSave();
        await OnClose();
    }

    private Task OnClose()
    {
        return NavigationService?.NavigateBack() ?? Task.CompletedTask;
    }

    private void OnUnsavedChanges(object instance, PropertyInfo property, PropertyChangeNotificationType notificationType)
    {
        Notify(nameof(UnsavedChanges));
    }

    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        if (ChildNavService is not null)
        {
            ChildNavService.HomePage = new VivInfoViewModel() { State = State };
            ChildNavService.Reset();
            //ChildNavService.NavigateAndReset<VivInfoViewModel, VivEditorState>(State);
        }
        return base.OnCreated();
    }
}
