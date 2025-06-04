using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Resources;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Properties;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Tools.Fce;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.FceEditorView;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Base class for all types that represent FCE editor view models.
/// </summary>
/// <typeparam name="TState">
/// Type of internal ViewModel state to use.
/// </typeparam>
/// <typeparam name="TFile">
/// Type of FCE file to be edited by this ViewModel.
/// </typeparam>
/// <typeparam name="TFceColor">
/// Type used by this ViewModel to represent a single FCE color.
/// </typeparam>
/// <typeparam name="THsbColor">
/// Type used by the FCE file to store a single FCE color.
/// </typeparam>
/// <typeparam name="TFcePart">
/// Type of FCE part to be stored and represented by this ViewModel.
/// </typeparam>
/// <typeparam name="TRender">
/// Type of render state builder to use when rendering the FCE model.
/// </typeparam>
public abstract class FceEditorViewModelBase<TState, TFile, TFceColor, THsbColor, TFcePart, TRender>
    : StatefulFileEditorViewModelBase<TState, TFile>
    where TState : FceEditorStateBase<TFile, TFceColor,THsbColor, TFcePart>, IFceEditorState<TFcePart>, new()
    where TFile : FceFileBase<THsbColor, TFcePart>, new()
    where TFceColor : IFceColor<THsbColor>
    where THsbColor : IHsbColor
    where TFcePart : FcePart
    where TRender : IFceRenderStateBuilder<TState>, new()
{
    private static readonly TRender _render = new();

    private bool _refreshEnabled;

    /// <summary>
    /// Gets a command that applies center transformations to fix a "floating
    /// car".
    /// </summary>
    public ICommand FceCenterCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to rename an FCE part.
    /// </summary>
    public ICommand RenamePartCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to rename an FCE part.
    /// </summary>
    public ICommand RenameDummyCommand { get; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="FceEditorViewModelBase{TState, TFile, TFceColor, THsbColor, TFcePart, TRender}"/>
    /// class.
    /// </summary>
    protected FceEditorViewModelBase()
    {
        var cb = CommandBuilder.For(this);
        RenamePartCommand = cb.BuildSimple(OnElementRename<TFcePart>);
        RenameDummyCommand = cb.BuildSimple(async p => { if (await OnElementRename<FceDummy>(p)) OnVisibleChanged(); });
        FceCenterCommand = cb.BuildSimple(OnFceCenter);
    }

    /// <summary>
    /// Toggles the visibility of the FCE parts to the desired LOD level.
    /// </summary>
    /// <param name="preset">LOD preset to switch to.</param>
    protected abstract void OnSwitchToLod(FceLodPreset preset);
    
    /// <summary>
    /// Reloads the names of all the colors defined in the FCE.
    /// </summary>
    /// <returns></returns>
    protected async Task LoadColorNames()
    {
        if (await (BackingStore?.Store.GetAnyFeData() ?? Task.FromResult<IFeData?>(null)) is { } fedata)
        {
            SetColorNames(State.Colors, fedata);
        }
    }

    /// <summary>
    /// Manually refreshes the rendered view of the FCE model.
    /// </summary>
    protected void OnVisibleChanged() => OnVisibleChanged(null, null, default);

    /// <inheritdoc/>
    protected override async Task OnCreated()
    {
        State.RenderShadow = Settings.Current.Fce_ShadowByDefault;
        await foreach (var j in GetTextures(BackingStore?.Store)) State.CarTextures.Add(j);
        State.CarTextures.Add(new(null!, St.NoTexture));
        if (State.CarTextures.FirstOrDefault(p => p.Name.StartsWith(Path.GetFileNameWithoutExtension(BackingStore!.FileName)!)) is { Value: byte[] texture })
        {
            State.SelectedTexture = texture;
        }
        await LoadColorNames();
        State.FceLodPreset = FceLodPreset.High;
        SwitchToLod(State.FceLodPreset);
        ObserveCollection(State.Dummies);
        ObserveCollection(State.Parts);
        OnVisibleChanged();
        State.UnsavedChanges = false;
        State.Subscribe(p => p.FceLodPreset, (i, p, n) => OnSwitchToLod(State.FceLodPreset));
        State.Subscribe(OnVisibleChanged);
        await base.OnCreated();
    }

    /// <inheritdoc/>
    protected override bool BeforeSave()
    {
        // Only center cars: Dashes should not be centered
        if (Settings.Current.Fce_CenterModel && (BackingStore?.FileName?.StartsWith("car", StringComparison.InvariantCultureIgnoreCase) ?? false))
        {
            OnFceCenter();
        }
        return base.BeforeSave();
    }

    private void OnVisibleChanged(object? instance, PropertyInfo? property, PropertyChangeNotificationType notificationType)
    {
        if (!_refreshEnabled) return;
        _refreshEnabled = false;
        State.RenderTree = _render.Build(State);
        _refreshEnabled = true;
    }

    private void SwitchToLod(FceLodPreset preset)
    {
        _refreshEnabled = false;
        OnSwitchToLod(preset);
        _refreshEnabled = true;
    }

    private async Task<bool> OnElementRename<T>(object? parameter) where T : Models.Base.INameable
    {
        if (parameter is not FcePartListItem<T> { Part: Models.Base.INameable nameable } part || DialogService is null) return false;
        var result = await DialogService.GetInputText(CommonDialogTemplates.Input with { Title = St.RenamePart, Text = St.RenamePartHelp }, nameable.Name);
        if (result.Success)
        {
            nameable.Name = result.Result;
            part.Refresh();
        }
        return result.Success;
    }

    private void OnFceCenter()
    {
        FceCenter.Center(State.File);
        State.UnsavedChanges = true;
        OnVisibleChanged();
    }

    private void ObserveCollection<T>(ObservableCollection<FcePartListItem<T>> collection)
    {
        foreach (var j in collection)
        {
            j.Subscribe(() => j.IsVisible, OnVisibleChanged);
        }
        collection.CollectionChanged += (_, e) =>
        {
            if (e.OldItems is not null) foreach (var j in e.OldItems.Cast<FcePartListItem<T>>()) j.Unsubscribe(() => j.IsVisible);
            if (e.NewItems is not null) foreach (var j in e.NewItems.Cast<FcePartListItem<T>>()) j.Subscribe(() => j.IsVisible, OnVisibleChanged);
        };
    }

    private static void SetColorNames(ICollection<TFceColor> colors, IFeData feData)
    {
        string[] colorNames = feData.GetColorNames();

        foreach (var (index, element) in colors.WithIndex().Take(colorNames.Length))
        {
            element.Name = colorNames[index];
        }
    }

    private static async IAsyncEnumerable<NamedObject<byte[]>> GetTextures(IBackingStore? store)
    {
        var extensions = (Settings.Current.Fce_EnumerateAllImages ? Mappings.ExportEnconder.Keys.Concat([".fsh", ".qfs"]) : [".tga"]).ToArray();
        foreach (var file in store?.EnumerateFiles().Where(p => extensions.Contains(Path.GetExtension(p).ToLowerInvariant())) ?? [])
        {
            if (await store!.ReadAsync(file) is byte[] contents)
            {
                yield return new NamedObject<byte[]>(contents, file);
            }
        }
    }
}
