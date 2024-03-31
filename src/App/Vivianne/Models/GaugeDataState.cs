namespace TheXDS.Vivianne.Models;

public class GaugeDataState : EditorViewModelStateBase
{
    private GaugeData _data;
    private int _previewSteerAngle;
    private int _SteeringXRotation;
    private int _SteeringYRotation;
    private int _SteeringXPosition;
    private int _SteeringYPosition;
    private int _GearXPosition;
    private int _GearYPosition;

    public GaugeData BackingStore => _data;

    public FshBlob Cabin { get; }

    public FshBlob? Steering { get; }

    public FshBlob? GearIndicator { get; }

    public GaugeDataState(FshFile fsh)
    {
        Cabin = fsh.Entries["0000"];
        if (fsh.Entries.TryGetValue("0001", out var steering))
        {
            Steering = steering;
            SteeringXPosition = steering.XPosition;
            SteeringYPosition = steering.YPosition;
            SteeringXRotation = steering.XRotation;
            SteeringYRotation = steering.YRotation;
        }
        if (fsh.Entries.TryGetValue("gea1", out var gear))
        {
            GearIndicator = gear;
            GearXPosition = gear.XPosition;
            GearYPosition = gear.YPosition;
        }
        _data = Cabin.GaugeData ?? default;

        RegisterPropertyChangeTrigger(nameof(SteeringLeft), nameof(SteeringXPosition), nameof(SteeringXRotation));
        RegisterPropertyChangeTrigger(nameof(SteeringTop), nameof(SteeringYPosition), nameof(SteeringYRotation));
        RegisterPropertyChangeTrigger(nameof(GearLeft), nameof(GearXPosition));
        RegisterPropertyChangeTrigger(nameof(GearTop), nameof(GearYPosition));
    }

    public int SteeringLeft => Steering is not null ? SteeringXPosition - SteeringXRotation : 0;

    public int SteeringTop => Steering is not null ? SteeringYPosition - SteeringYRotation : 0;

    public int GearLeft => GearIndicator is not null ? GearXPosition : 0;

    public int GearTop => GearIndicator is not null ? GearYPosition : 0;

    public int PreviewSteerAngle
    {
        get => _previewSteerAngle;
        set => Change(ref _previewSteerAngle, value);
    }

    public int DialColorX
    {
        get => _data.DialColorX;
        set => Change(ref _data.DialColorX, value);
    }

    public int DialColorY
    {
        get => _data.DialColorY;
        set => Change(ref _data.DialColorY, value);
    }

    public int DialWidthBase
    {
        get => _data.DialWidthBase;
        set => Change(ref _data.DialWidthBase, value);
    }

    public int DialWidthTip
    { 
        get => _data.DialWidthTip;
        set => Change(ref _data.DialWidthTip, value);
    }

    public int SpeedometerCenterX
    {
        get => _data.SpeedometerCenterX;
        set => Change(ref _data.SpeedometerCenterX, value);
    }

    public int SpeedometerCenterY
    {
        get => _data.SpeedometerCenterY;
        set => Change(ref _data.SpeedometerCenterY, value);
    }

    public int SpeedometerCenterOffset
    {
        get => _data.SpeedometerCenterOffset; 
        set => Change(ref _data.SpeedometerCenterOffset, value);
    }

    public int SpeedometerEdgeOffset
    {
        get => _data.SpeedometerEdgeOffset;
        set => Change(ref _data.SpeedometerEdgeOffset, value);
    }

    public int SpeedometerMin
    {
        get => _data.SpeedometerMin;
        set => Change(ref _data.SpeedometerMin, value);
    }

    public int SpeedometerMax
    {
        get => _data.SpeedometerMax;
        set => Change(ref _data.SpeedometerMax, value);
    }

    public int SpeedometerMinX
    {
        get => _data.SpeedometerMinX;
        set => Change(ref _data.SpeedometerMinX, value);
    }

    public int SpeedometerMinY
    {
        get => _data.SpeedometerMinY;
        set => Change(ref _data.SpeedometerMinY, value);
    }

    public int SpeedometerMaxX
    {
        get => _data.SpeedometerMax;
        set => Change(ref _data.SpeedometerMaxX, value);
    }

    public int SpeedometerMaxY
    {
        get => _data.SpeedometerMaxY;
        set => Change(ref _data.SpeedometerMaxY, value);
    }

    public int TachometerCenterX
    {
        get => _data.TachometerCenterX;
        set => Change(ref _data.TachometerCenterX, value);
    }

    public int TachometerCenterY
    {
        get => _data.TachometerCenterY;
        set => Change(ref _data.TachometerCenterY, value);
    }

    public int TachometerCenterOffset
    {
        get => _data.TachometerCenterOffset;
        set => Change(ref _data.TachometerCenterOffset, value);
    }

    public int TachometerEdgeOffset
    {
        get => _data.TachometerEdgeOffset;
        set => Change(ref _data.TachometerEdgeOffset, value);
    }

    public int TachometerMin
    {
        get => _data.TachometerMin;
        set => Change(ref _data.TachometerMin, value);
    }

    public int TachometerMax
    {
        get => _data.TachometerMax;
        set => Change(ref _data.TachometerMax, value);
    }

    public int TachometerMinX
    {
        get => _data.TachometerMinX;
        set => Change(ref _data.TachometerMinX, value);
    }

    public int TachometerMinY
    {
        get => _data.TachometerMinY;
        set => Change(ref _data.TachometerMinY, value);
    }

    public int TachometerMaxX
    {
        get => _data.TachometerMax;
        set => Change(ref _data.TachometerMaxX, value);
    }

    public int TachometerMaxY
    {
        get => _data.TachometerMaxY;
        set => Change(ref _data.TachometerMaxY, value);
    }

    public int SteeringXRotation
    {
        get => _SteeringXRotation;
        set => Change(ref _SteeringXRotation, value);
    }

    public int SteeringYRotation
    {
        get => _SteeringYRotation;
        set => Change(ref _SteeringYRotation, value);
    }

    public int SteeringXPosition
    {
        get => _SteeringXPosition;
        set => Change(ref _SteeringXPosition, value);
    }

    public int SteeringYPosition
    {
        get => _SteeringYPosition;
        set => Change(ref _SteeringYPosition, value);
    }

    public int GearXPosition
    {
        get => _GearXPosition;
        set => Change(ref _GearXPosition, value);
    }

    public int GearYPosition
    {
        get => _GearYPosition;
        set => Change(ref _GearYPosition, value);
    }
}