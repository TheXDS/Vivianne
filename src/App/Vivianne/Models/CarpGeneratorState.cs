using TheXDS.MCART.Types.Base;

namespace TheXDS.Vivianne.Models;

public class CarpGeneratorState : NotifyPropertyChanged
{
    private double _Mass;
    private double _TopSpeed;
    private int _MinRpm;
    private int _MaxRpm;

    public double Mass
    {
        get => _Mass;
        set => Change(ref _Mass, value);
    }

    public double TopSpeed
    {
        get => _TopSpeed;
        set => Change(ref _TopSpeed, value);
    }

    public int MinRpm
    {
        get => _MinRpm;
        set => Change(ref _MinRpm, value);
    }

    public int MaxRpm
    {
        get => _MaxRpm;
        set => Change(ref _MaxRpm, value);
    }

}