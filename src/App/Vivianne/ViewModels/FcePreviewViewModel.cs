using System.Collections.Generic;
using System.Linq;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

public class FcePreviewViewModel : ViewModel
{
    private readonly FceFile fce;
    private readonly IDictionary<string, byte[]> vivDirectory;
    private FceColor _SelectedPrimaryColor;
    private FceColor _SelectedSecondaryColor;
    private byte[]? _SelectedCarTexture;
    private RenderTreeState? _RenderTree;

    public FcePreviewViewModel(FceFile fce, IDictionary<string, byte[]> vivDirectory)
    {
        this.fce = fce;
        this.vivDirectory = vivDirectory;
        Parts = fce.Select(p => new FcePartListItem(p)).ToArray();
        PrimaryColors = fce.Header.PrimaryColorTable;
        SecondaryColors = fce.Header.SecondaryColorTable;
        CarTextures = vivDirectory.Where(p => p.Key.EndsWith(".tga")).Select(p => p.Value);
        foreach (var j in Parts)
        {
            j.ForwardChange(this);
        }
        RenderTree = new(this);
        RegisterPropertyChangeTrigger(nameof(RenderTree), "IsVisible", nameof(SelectedPrimaryColor), nameof(SelectedSecondaryColor), nameof(SelectedCarTexture));
    }

    public IEnumerable<FcePartListItem> Parts { get; }

    public FceColor[] PrimaryColors { get; }

    public FceColor[] SecondaryColors { get; }

    public IEnumerable<byte[]> CarTextures { get; }

    public FceColor SelectedPrimaryColor
    {
        get => _SelectedPrimaryColor;
        set => Change(ref _SelectedPrimaryColor, value);
    }

    public FceColor SelectedSecondaryColor
    {
        get => _SelectedSecondaryColor;
        set => Change(ref _SelectedSecondaryColor, value);
    }

    public byte[]? SelectedCarTexture
    {
        get => _SelectedCarTexture;
        set => Change(ref _SelectedCarTexture, value);
    }

    public RenderTreeState? RenderTree
    {
        get => _RenderTree;
        set => Change(ref _RenderTree, value);
    }
}

public class RenderTreeState
{
    private readonly FcePreviewViewModel source;

    public RenderTreeState(FcePreviewViewModel source)
    {
        this.source = source;
    }

    public IEnumerable<FcePart> Parts => source.Parts.Where(p => p.IsVisible).Select(p => p.Part);

    public byte[]? Texture => source.SelectedCarTexture;
}

public class FcePartListItem : NotifyPropertyChanged
{
    private bool _IsVisible;

    public FcePartListItem(FcePart part, string partName)
    {
        Part = part;
        PartName = partName;
    }

    public FcePartListItem(KeyValuePair<string, FcePart> part) : this(part.Value, part.Key)
    {
        IsVisible = part.Key.StartsWith(":H", System.StringComparison.InvariantCultureIgnoreCase);
    }

    public FcePart Part { get; }

    public string PartName { get; }

    public bool IsVisible
    {
        get => _IsVisible;
        set => Change(ref _IsVisible, value);
    }
}
