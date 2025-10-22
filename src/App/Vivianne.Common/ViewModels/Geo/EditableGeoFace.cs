using SixLabors.ImageSharp;
using System.Collections.Generic;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Geo;

namespace TheXDS.Vivianne.ViewModels.Geo;

/// <summary>
/// Represents an editable GEO face.
/// </summary>
public class EditableGeoFace : NotifyPropertyChanged
{
    private GeoMaterialFlags _Flags;
    private FshBlob? _SelectedTexture;
    private readonly FshFile? _TextureSource;
    private readonly EditorViewModelStateBase parent;

    /// <summary>
    /// Gets a reference to the GEO face being edited.
    /// </summary>
    public GeoFace Face { get; }

    /// <summary>
    /// Gets an index to help identify the selected GEO face.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableGeoFace"/> class.
    /// </summary>
    /// <param name="parent">Parent editor state.</param>
    /// <param name="face">GEO face to be edited.</param>
    /// <param name="textureSource">FSH texture source.</param>
    /// <param name="index">Index of the face being edited.</param>
    public EditableGeoFace(EditorViewModelStateBase parent, GeoFace face, FshFile? textureSource, int index)
    {
        this.parent = parent;
        Face = face;
        SelectedTexture = (_TextureSource = textureSource)?.Entries.GetValueOrDefault(face.TextureName);
        Index = index;
    }

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        broadcastSetup.RegisterPropertyChangeBroadcast(() => SelectedTexture, () => Palette);
    }

    /// <summary>
    /// Gets a reference to a palette that can be used to render the selected
    /// texture, if any.
    /// </summary>
    public Color[]? Palette => SelectedTexture?.ReadLocalPalette() ?? _TextureSource?.GetPalette();

    /// <summary>
    /// Gets a value that indicates that Alpha blending should be enabled.
    /// </summary>
    public bool Alpha => true;

    /// <summary>
    /// Gets or sets the value of the Flags property.
    /// </summary>
    /// <value>The value of the Flags property.</value>
    public GeoMaterialFlags Flags
    {
        get => _Flags;
        set
        {
            if (Change(ref _Flags, value)) parent.UnsavedChanges = true;
        }
    }

    /// <summary>
    /// Gets or sets the value of the SelectedTexture property.
    /// </summary>
    /// <value>The value of the SelectedTexture property.</value>
    public FshBlob? SelectedTexture
    {
        get => _SelectedTexture;
        set
        {
            if (Change(ref _SelectedTexture, value)) parent.UnsavedChanges = true;
        }
    }
}
