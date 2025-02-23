using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents an inmutable FCE color set for NFS3 cars.
/// </summary>
/// <param name="primaryColor">Value of the primary color.</param>
/// <param name="secondaryColor">Value of the secondary color.</param>
public class Fce3Color : NotifyPropertyChanged
{
    private HsbColor _primaryColor;
    private HsbColor _secondaryColor;
    private string _name;

    public string Name
    {
        get => _name;
        set => Change(ref _name, value);
    }

    public HsbColor PrimaryColor 
    { 
        get => _primaryColor;
        set => Change(ref _primaryColor, value);
    }

    public HsbColor SecondaryColor
    { 
        get => _secondaryColor;
        set => Change(ref _secondaryColor, value);
    }
}
