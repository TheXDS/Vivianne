using SixLabors.ImageSharp.Formats.Png;
using System.Collections.Generic;
using System.Linq;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to preview an FCE model.
/// </summary>
public class FcePreviewViewModel : ViewModel
{
    private readonly FceColor[] PrimaryColors;
    private readonly FceColor[] SecondaryColors;
    private byte[]? _SelectedCarTexture;
    private RenderTreeState? _RenderTree;
    private int _selectedColorIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="FcePreviewViewModel"/> class.
    /// </summary>
    /// <param name="fce">Model to preview</param>
    /// <param name="vivDirectory">Reference to the VIV file from which to enumerate the available textures.</param>
    public FcePreviewViewModel(FceFile fce, IDictionary<string, byte[]> vivDirectory)
    {
        Parts = fce.Select(p => new FcePartListItem(p)).ToArray();
        PrimaryColors = fce.Header.PrimaryColorTable;
        SecondaryColors = fce.Header.SecondaryColorTable;
        CarTextures = vivDirectory.Where(p => p.Key.EndsWith(".tga")).Select(p => new NamedObject<byte[]>(p.Value, p.Key));
        foreach (var j in Parts)
        {
            j.ForwardChange(this);
        }
        RenderTree = new(this);
        RegisterPropertyChangeTrigger(nameof(RenderTree), "IsVisible", nameof(SelectedColorIndex), nameof(SelectedCarTexture));
    }

    /// <summary>
    /// Gets a collection of the FCE parts contained in the model.
    /// </summary>
    public IEnumerable<FcePartListItem> Parts { get; }

    /// <summary>
    /// Gets a collection of the available textures from the VIV file.
    /// </summary>
    public IEnumerable<NamedObject<byte[]>> CarTextures { get; }

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
}
