using SixLabors.ImageSharp;
using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Geo;

namespace TheXDS.Vivianne.ViewModels.Geo;

/// <summary>
/// Represents the state of the <see cref="GeoPartEditorViewModel"/>.
/// </summary>
public class GeoPartEditorState : EditorViewModelStateBase
{
    private int _unk_0x14;
    private int _unk_0x18;
    private long _unk_0x1C;
    private long _unk_0x24;
    private long _unk_0x2C;
    private EditableGeoFace _selectedFace;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoPartEditorState"/>
    /// class.
    /// </summary>
    /// <param name="part">Part to be edited.</param>
    /// <param name="textureSource">
    /// Fsh file with textures to be used in the GEO model.
    /// </param>
    public GeoPartEditorState(GeoPart part, FshFile? textureSource)
    {
        Part = part;
        _unk_0x14 = part.Unk_0x14;
        _unk_0x18 = part.Unk_0x18;
        _unk_0x1C = part.Unk_0x1C;
        _unk_0x24 = part.Unk_0x24;
        _unk_0x2C = part.Unk_0x2C;
        RegisterUnconsequentialProperty(() => SelectedFace);
        Faces = [.. part.Faces.Select((p, i) => new EditableGeoFace(this, p, textureSource, i))];
        _selectedFace = Faces.First();
        TextureSource = textureSource;
    }

    /// <summary>
    /// Gets a collection of the GEO faces in editable format.
    /// </summary>
    public ICollection<EditableGeoFace> Faces { get; }

    /// <summary>
    /// Gets a reference to the available texture source.
    /// </summary>
    public FshFile? TextureSource { get; }

    /// <summary>
    /// Gets a reference to the <see cref="GeoPart"/> that is being edited.
    /// </summary>
    public GeoPart Part { get; }

    /// <summary>
    /// Gets or sets the value of the Unk_0x14 property.
    /// </summary>
    /// <value>The value of the Unk_0x14 property.</value>
    public int Unk_0x14
    {
        get => _unk_0x14;
        set => Change(ref _unk_0x14, value);
    }

    /// <summary>
    /// Gets or sets the value of the Unk_0x18 property.
    /// </summary>
    /// <value>The value of the Unk_0x18 property.</value>
    public int Unk_0x18
    {
        get => _unk_0x18;
        set => Change(ref _unk_0x18, value);
    }

    /// <summary>
    /// Gets or sets the value of the Unk_0x1C property.
    /// </summary>
    /// <value>The value of the Unk_0x1C property.</value>
    public long Unk_0x1C
    {
        get => _unk_0x1C;
        set => Change(ref _unk_0x1C, value);
    }

    /// <summary>
    /// Gets or sets the value of the Unk_0x24 property.
    /// </summary>
    /// <value>The value of the Unk_0x24 property.</value>
    public long Unk_0x24
    {
        get => _unk_0x24;
        set => Change(ref _unk_0x24, value);
    }

    /// <summary>
    /// Gets or sets the value of the Unk_0x2C property.
    /// </summary>
    /// <value>The value of the Unk_0x2C property.</value>
    public long Unk_0x2C
    {
        get => _unk_0x2C;
        set => Change(ref _unk_0x2C, value);
    }

    /// <summary>
    /// Gets or sets the selected GEO face to preview/edit.
    /// </summary>
    public EditableGeoFace SelectedFace
    {
        get => _selectedFace;
        set => Change(ref _selectedFace, value);
    }
}
