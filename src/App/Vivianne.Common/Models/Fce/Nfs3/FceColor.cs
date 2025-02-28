using TheXDS.MCART.Types.Base;

namespace TheXDS.Vivianne.Models.Fce.Nfs3;

/// <summary>
/// Represents an inmutable FCE color set for NFS3 cars.
/// </summary>
public class FceColor : NotifyPropertyChanged
{
    private HsbColor _primaryColor;
    private HsbColor _secondaryColor;
    private string? _name;

    /// <summary>
    /// Gets or sets the name of the color.
    /// </summary>
    public string Name
    {
        get => _name ?? PrimaryColor.ToString();
        set => Change(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the primary color.
    /// </summary>
    public HsbColor PrimaryColor 
    { 
        get => _primaryColor;
        set => Change(ref _primaryColor, value);
    }

    /// <summary>
    /// Gets or sets the secondary color.
    /// </summary>
    public HsbColor SecondaryColor
    { 
        get => _secondaryColor;
        set => Change(ref _secondaryColor, value);
    }
}
