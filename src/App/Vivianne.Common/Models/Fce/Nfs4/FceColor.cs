using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents an inmutable FCE color set for NFS4 cars.
/// </summary>
public class FceColor : NotifyPropertyChanged
{
    private string? _name;
    private HsbColor _primaryColor;
    private HsbColor _interiorColor;
    private HsbColor _secondaryColor;
    private HsbColor _driverHairColor;

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
    /// Gets or sets the interior color.
    /// </summary>
    public HsbColor InteriorColor
    {
        get => _interiorColor;
        set => Change(ref _interiorColor, value);
    }

    /// <summary>
    /// Gets or sets the secondary color.
    /// </summary>
    public HsbColor SecondaryColor
    {
        get => _secondaryColor;
        set => Change(ref _secondaryColor, value);
    }

    /// <summary>
    /// Gets or sets the driver hair color.
    /// </summary>
    public HsbColor DriverHairColor
    {
        get => _driverHairColor;
        set => Change(ref _driverHairColor, value);
    }
}
