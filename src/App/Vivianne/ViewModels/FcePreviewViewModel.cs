using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model.
/// </summary>
public class FcePreviewViewModel : ViewModel
{
    private byte[]? _SelectedCarTexture;
    private RenderTreeState? _RenderTree;
    private int _selectedColorIndex;
    private readonly FceFile fce;
    private readonly Func<FceFile, Task>? saveCallback;
    private readonly IDictionary<string, byte[]>? vivDirectory;
    private bool _UnsavedChanges;

    private static IEnumerable<CarColorItem> CreateColors(IDictionary<string, byte[]>? vivDirectory, FceFileHeader header)
    {
        static Color ToColor(FceColor color)
        {
            var (R, G, B) = color.ToRgb();
            return Color.FromArgb(R, G, B);
        }
        static string[] ReadColors(byte[] fd)
        {
            var fe = ((ISerializer<FeData>)new FeDataSerializer()).Deserialize(fd);
            return [fe.Color1, fe.Color2, fe.Color3, fe.Color4, fe.Color5, fe.Color6, fe.Color7, fe.Color8, fe.Color9, fe.Color10];
        }

        var names = vivDirectory is not null && vivDirectory.TryGetValue("fedata.eng", out var fd)
            ? ReadColors(fd)
            : header.PrimaryColorTable.Select(p => p.ToString());

        return header.PrimaryColorTable
            .Zip(header.SecondaryColorTable, names.Take(header.PrimaryColors))
            .Select(p => new CarColorItem(p.Third, ToColor(p.First), ToColor(p.Second)));
    }

    private static IEnumerable<NamedObject<byte[]>> GetTextures(IDictionary<string, byte[]> vivDirectory)
    {
        return vivDirectory.Where(p => p.Key.EndsWith(".tga")).Select(p => new NamedObject<byte[]>(p.Value, p.Key));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FcePreviewViewModel"/> class.
    /// </summary>
    /// <param name="fce">Model to preview.</param>
    /// <param name="vivDirectory">Reference to the VIV file from which to enumerate the available textures.</param>
    /// <param name="saveCallback">
    /// Optional save callback to use when trying to save FCE files.
    /// </param>
    public FcePreviewViewModel(FceFile fce, IDictionary<string, byte[]> vivDirectory, Func<FceFile, Task>? saveCallback = null)
        : this(fce, saveCallback)
    {
        CarTextures = GetTextures(vivDirectory);
        CarColors = new ObservableCollection<CarColorItem>(CreateColors(vivDirectory, fce.Header));
        this.vivDirectory = vivDirectory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FcePreviewViewModel"/> class.
    /// </summary>
    /// <param name="fce">Model to preview.</param>
    /// <param name="saveCallback">
    /// Optional save callback to use when trying to save FCE files.
    /// </param>
    public FcePreviewViewModel(FceFile fce, Func<FceFile, Task>? saveCallback = null)
    {
        var cb = CommandBuilder.For(this);
        Parts = fce.Select(p => new FcePartListItem(p)).ToArray();
        CarTextures = [];
        CarColors = new ObservableCollection<CarColorItem>(CreateColors(null, fce.Header));
        foreach (var j in Parts)
        {
            j.ForwardChange(this);
        }
        RenderTree = new(this);
        RegisterPropertyChangeTrigger(nameof(RenderTree), "IsVisible", nameof(SelectedColorIndex), nameof(SelectedCarTexture));
        this.fce = fce;
        this.saveCallback = saveCallback;

        SaveChangesCommand = cb.BuildObserving(OnSaveChanges).ListensToCanExecute(vm => vm.UnsavedChanges).Build();
        ColorEditorCommand = cb.BuildSimple(OnColorEditor);
    }

    /// <summary>
    /// Gets a value that indicates if this ViewModel was created in read-only mode.
    /// </summary>
    public bool IsReadOnly => saveCallback is null;

    /// <summary>
    /// Gets a collection with the available car colors.
    /// </summary>
    public ICollection<CarColorItem> CarColors { get; }

    /// <summary>
    /// Gets a collection of the FCE parts contained in the model.
    /// </summary>
    public IEnumerable<FcePartListItem> Parts { get; }

    /// <summary>
    /// Gets a collection of the available textures from the VIV file.
    /// </summary>
    public IEnumerable<NamedObject<byte[]>> CarTextures { get; }

    /// <summary>
    /// Gets a reference to the command used to open a dialog to edit the FCE
    /// color tables.
    /// </summary>
    public ICommand ColorEditorCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to save all pending changes on the
    /// FCE file.
    /// </summary>
    public ICommand SaveChangesCommand { get; }

    /// <summary>
    /// Gets or sets the raw contents of the selected car texture.
    /// </summary>
    /// <remarks>By NFS3 standard, the raw texture will be in Targa (TGA) format.</remarks>
    public byte[]? SelectedCarTexture
    {
        get => _SelectedCarTexture;
        set => Change(ref _SelectedCarTexture, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the index of the selected Car color.
    /// </summary>
    public int SelectedColorIndex
    {
        get => _selectedColorIndex;
        set => Change(ref _selectedColorIndex, value);
    }

    /// <summary>
    /// Gets the current render tree as generated from the state of this ViewModel.
    /// </summary>
    public RenderTreeState? RenderTree
    {
        get => _RenderTree;
        set => Change(ref _RenderTree, value);
    }

    /// <summary>
    /// Gets a value that indicates if there's unsaved changes on the underlying FCE file.
    /// </summary>
    public bool UnsavedChanges
    {
        get => _UnsavedChanges;
        private set => Change(ref _UnsavedChanges, value);
    }

    private async Task OnColorEditor()
    {
        var state = new FceColorTableEditorState(fce);
        var vm = new FceColorEditorViewModel(state);
        await DialogService!.CustomDialog(vm);
        CarColors.Clear();
        CarColors.AddRange(CreateColors(vivDirectory, fce.Header));
    }

    private async Task OnSaveChanges()
    {
        await (saveCallback?.Invoke(fce) ?? Task.CompletedTask);
    }
}
