namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the state of <see cref="ViewModels.DashEditorViewModel"/>.
/// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="GaugeDataState"/> class.
    /// </summary>
    /// <param name="fsh"></param>
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

    /// <summary>
    /// Exposes a reference to the current gauge data.
    /// </summary>
    public GaugeData BackingStore => _data;

    /// <summary>
    /// Gets a reference to the FSH blob that contains the entire cabin image.
    /// </summary>
    public FshBlob Cabin { get; }

    /// <summary>
    /// Gets a reference to the FSH blob that contains the steering wheel.
    /// </summary>
    public FshBlob? Steering { get; }

    /// <summary>
    /// Gets a reference to an optional FSH blob that contains one state of the
    /// gear indicator if one is present.
    /// </summary>
    public FshBlob? GearIndicator { get; }

    /// <summary>
    /// Gets a value that indicates the horizontal position of the steering 
    /// from the left.
    /// </summary>
    public int SteeringLeft => Steering is not null ? SteeringXPosition - SteeringXRotation : 0;

    /// <summary>
    /// Gets a value that indicates the vertical position of the steering from
    /// the top.
    /// </summary>
    public int SteeringTop => Steering is not null ? SteeringYPosition - SteeringYRotation : 0;

    /// <summary>
    /// Gets a value that indicates the horizontal position of the gear 
    /// indicator from the left.
    /// </summary>
    public int GearLeft => GearIndicator is not null ? GearXPosition : 0;

    /// <summary>
    /// Gets a value that indicates the vertical position of the gear indicator
    /// from the top.
    /// </summary>
    public int GearTop => GearIndicator is not null ? GearYPosition : 0;

    /// <summary>
    /// Gets or sets a value that indicates the angle of the steering wheel on
    /// the Preview pane.
    /// </summary>
    public int PreviewSteerAngle
    {
        get => _previewSteerAngle;
        set => Change(ref _previewSteerAngle, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate of the pixel from which to derive the
    /// color of the dials.
    /// </summary>
    public int DialColorX
    {
        get => _data.DialColorX;
        set => Change(ref _data.DialColorX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the pixel from which to derive the
    /// color of the dials.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the X coordinate of the center of the Speedometer dial.
    /// </summary>
    public int SpeedometerCenterX
    {
        get => _data.SpeedometerCenterX;
        set => Change(ref _data.SpeedometerCenterX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the center of the Speedometer dial.
    /// </summary>
    public int SpeedometerCenterY
    {
        get => _data.SpeedometerCenterY;
        set => Change(ref _data.SpeedometerCenterY, value);
    }

    /// <summary>
    /// Gets or sets the offset from the center to use when drawing the
    /// Speedometer dial.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the indicated minimum value of the Speedometer.
    /// </summary>
    /// <remarks>It's recommended to keep this value at zero.</remarks>
    public int SpeedometerMin
    {
        get => _data.SpeedometerMin;
        set => Change(ref _data.SpeedometerMin, value);
    }

    /// <summary>
    /// Gets or sets the indicated maximum value of the Speedometer.
    /// </summary>
    public int SpeedometerMax
    {
        get => _data.SpeedometerMax;
        set => Change(ref _data.SpeedometerMax, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate of the minimum indicated value of the
    /// Speedometer dial.
    /// </summary>
    public int SpeedometerMinX
    {
        get => _data.SpeedometerMinX;
        set => Change(ref _data.SpeedometerMinX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the minimum indicated value of the
    /// Speedometer dial.
    /// </summary>
    public int SpeedometerMinY
    {
        get => _data.SpeedometerMinY;
        set => Change(ref _data.SpeedometerMinY, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate of the maximum indicated value of the
    /// Speedometer dial.
    /// </summary>
    public int SpeedometerMaxX
    {
        get => _data.SpeedometerMax;
        set => Change(ref _data.SpeedometerMaxX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the maximum indicated value of the
    /// Speedometer dial.
    /// </summary>
    public int SpeedometerMaxY
    {
        get => _data.SpeedometerMaxY;
        set => Change(ref _data.SpeedometerMaxY, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate of the center of the Tachometer dial.
    /// </summary>
    public int TachometerCenterX
    {
        get => _data.TachometerCenterX;
        set => Change(ref _data.TachometerCenterX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the center of the Tachometer dial.
    /// </summary>
    public int TachometerCenterY
    {
        get => _data.TachometerCenterY;
        set => Change(ref _data.TachometerCenterY, value);
    }

    /// <summary>
    /// Gets or sets the offset from the center to use when drawing the
    /// Tachometer dial.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the indicated minimum value of the Tachometer.
    /// </summary>
    /// <remarks>It's recommended to keep this value at zero.</remarks>
    public int TachometerMin
    {
        get => _data.TachometerMin;
        set => Change(ref _data.TachometerMin, value);
    }

    /// <summary>
    /// Gets or sets the indicated maximum value of the Tachometer.
    /// </summary>
    public int TachometerMax
    {
        get => _data.TachometerMax;
        set => Change(ref _data.TachometerMax, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate of the minimum indicated value of the
    /// Tachometer dial.
    /// </summary>
    public int TachometerMinX
    {
        get => _data.TachometerMinX;
        set => Change(ref _data.TachometerMinX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the minimum indicated value of the
    /// Tachometer dial.
    /// </summary>
    public int TachometerMinY
    {
        get => _data.TachometerMinY;
        set => Change(ref _data.TachometerMinY, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate of the maximum indicated value of the
    /// Tachometer dial.
    /// </summary>
    public int TachometerMaxX
    {
        get => _data.TachometerMax;
        set => Change(ref _data.TachometerMaxX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the maximum indicated value of the
    /// Tachometer dial.
    /// </summary>
    public int TachometerMaxY
    {
        get => _data.TachometerMaxY;
        set => Change(ref _data.TachometerMaxY, value);
    }

    /// <summary>
    /// Gets or sets the local X coordinate for the rotation axis of the
    /// steering wheel.
    /// </summary>
    public int SteeringXRotation
    {
        get => _SteeringXRotation;
        set => Change(ref _SteeringXRotation, value);
    }

    /// <summary>
    /// Gets or sets the local Y coordinate for the rotation axis of the
    /// steering wheel.
    /// </summary>
    public int SteeringYRotation
    {
        get => _SteeringYRotation;
        set => Change(ref _SteeringYRotation, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate of the center of the steering wheel.
    /// </summary>
    public int SteeringXPosition
    {
        get => _SteeringXPosition;
        set => Change(ref _SteeringXPosition, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the center of the steering wheel.
    /// </summary>
    public int SteeringYPosition
    {
        get => _SteeringYPosition;
        set => Change(ref _SteeringYPosition, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate of the position of the gear indicator.
    /// </summary>
    public int GearXPosition
    {
        get => _GearXPosition;
        set => Change(ref _GearXPosition, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the position of the gear indicator.
    /// </summary>
    public int GearYPosition
    {
        get => _GearYPosition;
        set => Change(ref _GearYPosition, value);
    }
}