using System.Collections.Generic;
using System.Linq;
using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows a user to preview a FSH texture file.
/// </summary>
/// <remarks>
/// QFS files can be decompressed and shown as FSH files with this ViewModel.
/// </remarks>
public class FshPreviewViewModel : ViewModel
{
    private readonly FshTexture _fsh;
    private Gimx _CurrentImage;

    /// <summary>
    /// Initializes a new instance of the <see cref="FshPreviewViewModel"/>
    /// class.
    /// </summary>
    /// <param name="fsh">FSH file to preview.</param>
    public FshPreviewViewModel(FshTexture fsh)
    {
        _fsh = fsh;
        CurrentImage = _fsh.Images.Values.FirstOrDefault();
    }

    /// <summary>
    /// Gets a dictionary with the contents of the FSH file.
    /// </summary>
    public IDictionary<string, Gimx> Images => _fsh.Images;

    /// <summary>
    /// Gets or sets a reference to the GIMX blob currently on display.
    /// </summary>
    public Gimx CurrentImage
    {
        get => _CurrentImage;
        set => Change(ref _CurrentImage, value);
    }
}
