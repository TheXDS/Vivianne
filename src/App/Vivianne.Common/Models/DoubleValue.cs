using TheXDS.MCART.Types.Base;

namespace TheXDS.Vivianne.Models;

public class DoubleValue : NotifyPropertyChanged
{
    private double _Value;

    public double Value
    {
        get => _Value;
        set => Change(ref _Value, value);
    }
}
