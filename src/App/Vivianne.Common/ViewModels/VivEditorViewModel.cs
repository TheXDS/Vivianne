using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Ganymede.ViewModels;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// ViewModel that serves as the main view for interacting with a VIV file.
/// </summary>
public class VivEditorViewModel : HostViewModelBase, IFileEditorViewModel<VivEditorState, VivFile>
{
    private static readonly Dictionary<string, ContentVisualizerViewModelFactory> ContentVisualizers = new(ContentVisualizerConfiguration.Get());
    private static readonly Dictionary<string, Func<byte[]>> Templates = new(VivTemplates.Get());
    private ICommand saveCommand = null!;
    private ICommand? saveAsCommand = null;

    /// <inheritdoc/>
    public VivEditorState State { get; set; } = null!;

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
        NewFromTemplateCommand = cb.BuildSimple(OnNewFromTemplate);
        CloseCommand = cb.BuildSimple(OnClose);
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

    /// <inheritdoc/>
    public ICommand SaveCommand
    { 
        get => saveCommand;
        set => Change(ref saveCommand, value);
    }

    /// <inheritdoc/>
    public ICommand? SaveAsCommand
    {
        get => saveAsCommand;
        set => Change(ref saveAsCommand, value);
    }

    /// <inheritdoc/>
    public ICommand CloseCommand { get; }

    private void OnOpenFile(object? parameter)
    {
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file, Value: { } rawData })
        {
            void Save(byte[] data)
            {
                UiThread.Invoke((Action)(() => State.Directory[file] = data));
                State.UnsavedChanges = true;
            }
            IViewModel? vm = null;
            foreach (var j in ContentVisualizers.Where(p => file.EndsWith(p.Key, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (j.Value is { } factory && factory.Invoke(rawData, Save, State, file) is { } visualizer)
                {
                    vm = visualizer;
                    break;
                }
            }
            vm ??= new ExternalFileViewModel(rawData, Save);
            vm.Title = file;
            ChildNavService!.Navigate(vm);
        }
        else
        {
            ChildNavService!.NavigateAndReset<VivInfoViewModel, VivEditorState>(State);
        }
    }

    private async Task OnImportFile()
    {
        var r = await DialogService!.GetFileOpenPath(CommonDialogTemplates.FileOpen with { Title = "Import file" }, FileFilters.AnyVivContentFilter);
        if (r.Success)
        {
            var keyName = Path.GetFileName(r.Result).ToLower();
            if (State.Directory.ContainsKey(keyName) && !await DialogService.AskYn("Replace file", $"The file '{keyName}' already exist. Do you want to replace it?"))
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
            var r = await DialogService!.GetFileOpenPath(CommonDialogTemplates.FileOpen with { Title = $"Replace '{file}'", Text = $"Select a file to repace '{file}' with" }, [FileFilterItem.Simple(ext), FileFilterItem.AllFiles]);
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
            if (await DialogService!.AskYn($"Remove '{file}'", $"Are you sure you want to remove '{file}'?"))
            {
                State.Directory.Remove(file);
            }
        }
    }

    private async Task OnNewFromTemplate()
    {
        var r = await DialogService!.SelectOption(new DialogTemplate 
        { 
            Title = "New from template",
            Text = "Select a template to create a new file in the VIV directory.",
            Color = System.Drawing.Color.Aquamarine, Icon = "➕"
        }, [.. Templates.Keys]);
        if (r.Success)
        {
            var template = Templates.ToList()[r.Result];
            State.Directory[template.Key] = template.Value.Invoke();
        }
    }

    private Task OnClose()
    {
        return NavigationService?.NavigateBack() ?? Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        ChildNavService!.NavigateAndReset<VivInfoViewModel, VivEditorState>(State);
        return base.OnCreated();
    }
}
