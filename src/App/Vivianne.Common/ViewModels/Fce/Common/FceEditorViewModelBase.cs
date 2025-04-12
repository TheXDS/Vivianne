using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Resources;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Math;
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
    : FileEditorViewModelBase<TState, TFile>
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
        RenameDummyCommand = cb.BuildSimple(OnElementRename<FceDummy>);
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
        State.SelectedTexture = State.CarTextures.First().Value;
        await LoadColorNames();
        State.FceLodPreset = FceLodPreset.High;
        SwitchToLod(State.FceLodPreset);
        ObserveCollection(State.Dummies);
        ObserveCollection(State.Parts);
        State.UnsavedChanges = false;
        await base.OnCreated();
    }

    /// <inheritdoc/>
    protected override bool BeforeSave()
    {
        // TODO: sync up part and dummy names between state and file.
        if (Settings.Current.Fce_CenterModel)
        {
            OnFceCenter();
        }
        return base.BeforeSave();
    }

    private void OnVisibleChanged(object? instance, PropertyInfo? property, PropertyChangeNotificationType notificationType)
    {
        if (!_refreshEnabled) return;
        State.RenderTree = _render.Build(State);
    }

    private void SwitchToLod(FceLodPreset preset)
    {
        _refreshEnabled = false;
        OnSwitchToLod(preset);
        _refreshEnabled = true;
        OnVisibleChanged();
    }

    private async Task OnElementRename<T>(object? parameter) where T : Models.Base.INameable
    {
        if (parameter is not FcePartListItem<T> { Part: Models.Base.INameable nameable } part || DialogService is null) return;
        var result = await DialogService.GetInputText(CommonDialogTemplates.Input with { Title = St.RenamePart, Text = St.RenamePartHelp }, nameable.Name);
        if (result.Success)
        {
            nameable.Name = result.Result;
            part.Refresh();
        }
    }

    private void OnFceCenter()
    {
        var vertices = State.File.Parts.SelectMany(p => p.TransformedVertices).ToArray();
        var minX = vertices.Min(p => p.X);
        var minY = vertices.Min(p => p.Y);
        var minZ = vertices.Min(p => p.Z);
        var xDiff = minX + State.File.XHalfSize;
        var yDiff = minY + State.File.YHalfSize;
        var zDiff = minZ + State.File.ZHalfSize;
        if (((IEnumerable<float>)[xDiff, yDiff, zDiff]).AreZero()) return;
        foreach (var j in State.File.Parts)
        {
            j.Origin = new Vector3
            {
                X = j.Origin.X - xDiff,
                Y = j.Origin.Y - yDiff,
                Z = j.Origin.Z - zDiff
            };
        }
        State.UnsavedChanges = true;
        OnVisibleChanged();
    }

    private void ObserveCollection(INotifyCollectionChanged collection)
    {
        collection.CollectionChanged += (_, e) =>
        {
            if (e.OldItems is not null) foreach (var j in e.OldItems.Cast<FcePartListItem<TFcePart>>()) j.Unsubscribe(() => j.IsVisible);
            if (e.NewItems is not null) foreach (var j in e.NewItems.Cast<FcePartListItem<TFcePart>>()) j.Subscribe(() => j.IsVisible, OnVisibleChanged);
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
        var extensions = (Settings.Current.Fce_EnumerateAllImages ? Mappings.ExportEnconder.Keys : [".tga"]).ToArray();
        foreach (var file in store?.EnumerateFiles().Where(p => extensions.Contains(Path.GetExtension(p).ToLowerInvariant())) ?? [])
        {
            if (await store!.ReadAsync(file) is byte[] contents)
            {
                yield return new NamedObject<byte[]>(contents, file);
            }
        }
    }
}
