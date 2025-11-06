using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Models;
using TheXDS.Ganymede.Resources;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Ganymede.ViewModels;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Data;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.VivEditorViewModel;

namespace TheXDS.Vivianne.ViewModels.Viv;

/// <summary>
/// ViewModel that serves as the main view for interacting with a VIV file.
/// </summary>
public class VivEditorViewModel : StatefulFileEditorViewModelBase<VivEditorState, VivFile>
{
    private static readonly Dictionary<string, (string, Func<byte[]>)[]> Templates = new(VivTemplates.Get());
    private readonly HostViewModel _childNavViewModel;

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

    /// <summary>
    /// Gets a reference to the child navigation service used to navigate to
    /// different file editors/visualizers.
    /// </summary>
    public INavigationService ChildNavService => _childNavViewModel.NavigationService;

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
        _childNavViewModel = new(this);
    }

    /// <inheritdoc/>
    protected override Task OnCreated()
    {
        State.FileName = BackingStore?.FileName;
        ChildNavService.HomePage = new VivInfoViewModel()
        {
            State = State,
            FileName = State.FileName,
            DialogService = DialogService
        };
        ChildNavService.Reset();
        return base.OnCreated();
    }

    private async Task OnOpenFile(object? parameter)
    {
        if (ChildNavService.CurrentViewModel is IViewModel currVm)
        {
            CancelFlag f = new();
            await currVm.OnNavigateAway(f);
            if (f.IsCancelled) return;
        }
        if (parameter is KeyValuePair<string, byte[]> { Key: { } file })
        {
            await ChildNavService.NavigateAndReset(await FileTypes.GetViewModel(file, CreateBackingStore, DialogService));
        }
        else
        {
            await ChildNavService.NavigateAndReset<VivInfoViewModel, VivEditorState>(State);
        }
    }

    private async Task OnImportFile()
    {
        if (await DialogService!.GetFilesOpenPath(CommonDialogTemplates.FileOpen with { Title = St.ImportFile }, Resources.FileFilters.AnyVivContentFilter) is { Success: true, Result: { } result })
        {
            await ImportFiles(result);
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
                await ChildNavService.Reset();
            }
        }
    }

    private async Task OnRenameFile(object? parameter)
    {
        if (parameter is not KeyValuePair<string, byte[]> { Key: { } fileName, Value: { } file }) return;
        var result = await DialogService!.GetInputText(CommonDialogTemplates.Input with { Title = St.Rename, Text = St.RenameHelp }, fileName);
        if (result.Success)
        {
            if (State.Directory.ContainsKey(result.Result) && !await DialogService.AskYn(St.Rename, string.Format("A file with the name '{0}' already exists. Overwrite?", result.Result))) return;
            State.Directory.Remove(fileName);
            State.Directory.Remove(result.Result);
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

    private IBackingStore CreateBackingStore() => new VivBackingStore(this);

//    private async Task<IViewModel> GetViewModel(byte[] rawData, string file)
//    {
//        IViewModel vm;
//        try
//        {
//            vm = PlatformServices.ModifierKey switch
//            { 
//                ModifierKey.Alt => ContentVisualizerConfiguration.CreateExternalEditorViewModel(rawData, CreateBackingStore, file),
//                ModifierKey.Ctrl => (await DialogService!.SelectOption(
//                    Dialogs.OpenAs,
//                    ContentVisualizers.Select(p => new NamedObject<ContentVisualizerViewModelFactory>(p.Key, p.Value)).ToArray())) is { Success:true, Result: { } factory }
//                    ? factory.Invoke(rawData, CreateBackingStore, file)
//                    : null,
//                _ => FindContentVisualizer(file, rawData)
//            } ?? FileErrorViewModel.UnknownFileFormat;
//        }
//        catch (Exception ex)
//        {
//#if DEBUG
//            await DialogService!.Error(ex);
//#else
//            (await DialogService!.Show<Action>(CommonDialogTemplates.Error with
//            {
//                Title = $"Could not open {file}",
//                Text = "The file might be damaged or corrupt; or may use a format not currently understood by Vivianne."
//            },
//            [
//                new("Ok", () => { }),
//                new("Copy details to clipboard", () => PlatformServices.OperatingSystem.WriteClipboardText(TheXDS.MCART.Resources.Strings.Composition.ExDump(ex, MCART.Resources.Strings.ExDumpOptions.Message)))
//            ])).Invoke();
//#endif

//            vm = new FileErrorViewModel(ex);
//        }
//        vm.Title = file;
//        return vm;
//    }

    private Task<bool> ImportFiles(IEnumerable<string> files) => DialogService!.RunOperation(async (cancel, progress) =>
    {
        foreach ((var index, var j) in files.WithIndex())
        {
            if (cancel.IsCancellationRequested) return;
            var keyName = Path.GetFileName(j).ToLower();
            if (State.Directory.ContainsKey(keyName))
            {
                switch (await DialogService.Show<int>(CommonDialogTemplates.Question with
                { 
                    Title = St.ReplaceFile,
                    Text = string.Format(St.TheFileXAlreadyExist, keyName)}, [("Yes", 0), ("No", 1), (St.Rename, 2)]))
                {
                    case 1: continue;
                    case 2:
                    {
                        if (await DialogService.GetInputText(St.Rename, St.RenameHelp, keyName) is { Success: true, Result: string result })
                        {
                            keyName = result;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            progress.Report(new ProgressReport(index * 100.0 / files.Count(), $"Importing {j}..."));
            State.Directory[keyName] = await File.ReadAllBytesAsync(j, cancel);
        }
    });
}
