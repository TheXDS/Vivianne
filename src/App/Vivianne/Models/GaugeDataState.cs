using System.Linq;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the state of the car dashboard editor.
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
    private int _previewSpeed;
    private int _previewRpm;

    /// <summary>
    /// Gets an object that exposes Gauge data for preview purposes.
    /// </summary>
    public GaugePreviewData PreviewGauge { get; }

    /// <summary>
    /// Gets a reference to the gauge data structure stored on the footer of
    /// the FSH id '0000'.
    /// </summary>
    public GaugeData BackingStore => _data;

    /// <summary>
    /// Gets the <see cref="FshBlob"/> associated with the actual cabin of the
    /// car.
    /// </summary>
    /// <remarks>
    /// The FSH blob associated with the car cabin must have the id
    /// '<c>0000</c>'.
    /// </remarks>
    public FshBlob Cabin { get; }

    /// <summary>
    /// Gets the <see cref="FshBlob"/> associated with the steering wheel of
    /// the car if one is present.
    /// </summary>
    /// <remarks>
    /// The FSH blob associated with the steering wheel must have the id
    /// '<c>0001</c>'.
    /// </remarks>
    public FshBlob? Steering { get; }

    /// <summary>
    /// Gets the <see cref="FshBlob"/> associated with a single image from the
    /// digital gear indicator collection if present.
    /// </summary>
    /// <remarks>
    /// For the gear indicators to work, the source FSH file should contain a
    /// set of FSH images with ids prefixed by '<c>gea</c>' followed by a
    /// single character denoting the gear. For example, '<c>geaR</c>' for
    /// reverse, '<c>geaN</c>' for neutral, '<c>gea1</c>' for first, etc.
    /// </remarks>
    public FshBlob? GearIndicator { get; }

    /// <summary>
    /// Gets the collection <see cref="FshBlob"/> associated with the digital
    /// gear indicators of the car if present.
    /// </summary>
    /// <remarks>
    /// For the gear indicators to work, the source FSH file should contain a
    /// set of FSH images with ids prefixed by '<c>gea</c>' followed by a
    /// single character denoting the gear. For example, '<c>geaR</c>' for
    /// reverse, '<c>geaN</c>' for neutral, '<c>gea1</c>' for first, etc.
    /// </remarks>
    public FshBlob[] Gears { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GaugeDataState"/> class.
    /// </summary>
    /// <param name="fsh">Source FSH file.</param>
    public GaugeDataState(FshFile fsh)
    {
        Cabin = fsh.Entries["0000"];
        PreviewGauge = new(this);
        if (fsh.Entries.TryGetValue("0001", out var steering))
        {
            Steering = steering;
            SteeringXPosition = steering.XPosition;
            SteeringYPosition = steering.YPosition;
            SteeringXRotation = steering.XRotation;
            SteeringYRotation = steering.YRotation;
        }
        Gears = fsh.Entries
            .Where(p => p.Key.StartsWith("gea", System.StringComparison.InvariantCultureIgnoreCase))
            .Select(p => p.Value)
            .ToArray();
        if (Gears.Length > 0)
        {
            GearIndicator = Gears[0];
            GearXPosition = Gears[0].XPosition;
            GearYPosition = Gears[0].YPosition;
        }
        _data = Cabin.GaugeData ?? default;

        RegisterPropertyChangeTrigger(nameof(SteeringLeft), nameof(SteeringXPosition), nameof(SteeringXRotation));
        RegisterPropertyChangeTrigger(nameof(SteeringTop), nameof(SteeringYPosition), nameof(SteeringYRotation));
        RegisterPropertyChangeTrigger(nameof(GearLeft), nameof(GearXPosition));
        RegisterPropertyChangeTrigger(nameof(GearTop), nameof(GearYPosition));
        RegisterPropertyChangeTrigger(nameof(PreviewGauge), [
            nameof(DialColorX),
            nameof(DialColorY),
            nameof(SpeedometerCenterX),
            nameof(SpeedometerCenterY),
            nameof(SpeedometerCenterOffset),
            nameof(SpeedometerEdgeOffset),
            nameof(SpeedometerMin),
            nameof(SpeedometerMax),
            nameof(SpeedometerMinX),
            nameof(SpeedometerMinY),
            nameof(SpeedometerMaxX),
            nameof(SpeedometerMaxY),
            nameof(TachometerCenterX),
            nameof(TachometerCenterY),
            nameof(TachometerCenterOffset),
            nameof(TachometerEdgeOffset),
            nameof(TachometerMin),
            nameof(TachometerMax),
            nameof(TachometerMinX),
            nameof(TachometerMinY),
            nameof(TachometerMaxX),
            nameof(TachometerMaxY),
            ]);
    }

    /// <summary>
    /// Indicates the current absolute X rendering position for the steering
    /// wheel.
    /// </summary>
    public int SteeringLeft => Steering is not null ? SteeringXPosition - SteeringXRotation : 0;

    /// <summary>
    /// Indicates the current absolute Y rendering position for the steering
    /// wheel.
    /// </summary>
    public int SteeringTop => Steering is not null ? SteeringYPosition - SteeringYRotation : 0;

    /// <summary>
    /// Indicates the current absolute X rendering position for the gear
    /// indicator.
    /// </summary>
    public int GearLeft => GearIndicator is not null ? GearXPosition : 0;

    /// <summary>
    /// Indicates the current absolute Y rendering position for the gear
    /// indicator.
    /// </summary>
    public int GearTop => GearIndicator is not null ? GearYPosition : 0;

    /// <summary>
    /// Gets or sets the steering wheel angle for rendering on the preview
    /// pane.
    /// </summary>
    public int PreviewSteerAngle
    {
        get => _previewSteerAngle;
        set => Change(ref _previewSteerAngle, value);
    }

    /// <summary>
    /// Gets or sets a value used to preview the indicated speed on the gauge
    /// cluster.
    /// </summary>
    public int PreviewSpeed
    {
        get => _previewSpeed;
        set => Change(ref _previewSpeed, value);
    }

    /// <summary>
    /// Gets or sets a value used to preview the indicated RPM on the gauge
    /// cluster.
    /// </summary>
    public int PreviewRpm
    {
        get => _previewRpm;
        set => Change(ref _previewRpm, value);
    }

    /// <summary>
    /// Gets or sets the X coordinates for the dial color.
    /// </summary>
    public int DialColorX
    {
        get => _data.DialColorX;
        set => Change(ref _data.DialColorX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinates for the dial color.
    /// </summary>
    public int DialColorY
    {
        get => _data.DialColorY;
        set => Change(ref _data.DialColorY, value);
    }

    /// <summary>
    /// Gets or sets the width of the dial base.
    /// </summary>
    public int DialWidthBase
    {
        get => _data.DialWidthBase;
        set => Change(ref _data.DialWidthBase, value);
    }

    /// <summary>
    /// GEts or sets the width of the dial tip.
    /// </summary>
    public int DialWidthTip
    { 
        get => _data.DialWidthTip;
        set => Change(ref _data.DialWidthTip, value);
    }

    /// <summary>
    /// Gets or sets the X center coordinates for the speedometer dial.
    /// </summary>
    public int SpeedometerCenterX
    {
        get => _data.Speedometer.CenterX;
        set => Change(ref _data.Speedometer.CenterX, value);
    }

    /// <summary>
    /// Gets or sets the Y center coordinates for the speedometer dial.
    /// </summary>
    public int SpeedometerCenterY
    {
        get => _data.Speedometer.CenterY;
        set => Change(ref _data.Speedometer.CenterY, value);
    }

    /// <summary>
    /// Gets or sets the offset to apply from the center to the actual start of
    /// the speedometer dial.
    /// </summary>
    public int SpeedometerCenterOffset
    {
        get => _data.Speedometer.CenterOffset; 
        set => Change(ref _data.Speedometer.CenterOffset, value);
    }

    /// <summary>
    /// Gets or sets the offset to apply from the edge to the actual end of the
    /// speedometer dial.
    /// </summary>
    public int SpeedometerEdgeOffset
    {
        get => _data.Speedometer.EdgeOffset;
        set => Change(ref _data.Speedometer.EdgeOffset, value);
    }

    /// <summary>
    /// Gets or sets the minimum indicated value for the speedometer.
    /// </summary>
    public int SpeedometerMin
    {
        get => _data.Speedometer.Min;
        set => Change(ref _data.Speedometer.Min, value);
    }

    /// <summary>
    /// Gets or sets the maximum indicated value for the speedometer.
    /// </summary>
    public int SpeedometerMax
    {
        get => _data.Speedometer.Max;
        set => Change(ref _data.Speedometer.Max, value);
    }

    /// <summary>
    /// Gets or sets the X coordinates for the minimum indicated value of the
    /// speedometer.
    /// </summary>
    public int SpeedometerMinX
    {
        get => _data.Speedometer.MinX;
        set => Change(ref _data.Speedometer.MinX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinates for the minimum indicated value of the
    /// speedometer.
    /// </summary>
    public int SpeedometerMinY
    {
        get => _data.Speedometer.MinY;
        set => Change(ref _data.Speedometer.MinY, value);
    }

    /// <summary>
    /// Gets or sets the X coordinates for the maximum indicated value of the
    /// speedometer.
    /// </summary>
    public int SpeedometerMaxX
    {
        get => _data.Speedometer.Max;
        set => Change(ref _data.Speedometer.MaxX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinates for the maximum indicated value of the
    /// speedometer.
    /// </summary>
    public int SpeedometerMaxY
    {
        get => _data.Speedometer.MaxY;
        set => Change(ref _data.Speedometer.MaxY, value);
    }

    /// <summary>
    /// Gets or sets the X center coordinates for the tachometer dial.
    /// </summary>
    public int TachometerCenterX
    {
        get => _data.Tachometer.CenterX;
        set => Change(ref _data.Tachometer.CenterX, value);
    }

    /// <summary>
    /// Gets or sets the Y center coordinates for the tachometer dial.
    /// </summary>
    public int TachometerCenterY
    {
        get => _data.Tachometer.CenterY;
        set => Change(ref _data.Tachometer.CenterY, value);
    }

    /// <summary>
    /// Gets or sets the offset to apply from the center to the actual start of
    /// the tachometer dial.
    /// </summary>
    public int TachometerCenterOffset
    {
        get => _data.Tachometer.CenterOffset;
        set => Change(ref _data.Tachometer.CenterOffset, value);
    }

    /// <summary>
    /// Gets or sets the offset to apply from the edge to the actual end of the
    /// tachometer dial.
    /// </summary>
    public int TachometerEdgeOffset
    {
        get => _data.Tachometer.EdgeOffset;
        set => Change(ref _data.Tachometer.EdgeOffset, value);
    }

    /// <summary>
    /// Gets or sets the minimum indicated value for the tachometer.
    /// </summary>
    public int TachometerMin
    {
        get => _data.Tachometer.Min;
        set => Change(ref _data.Tachometer.Min, value);
    }

    /// <summary>
    /// Gets or sets the maximum indicated value for the tachometer.
    /// </summary>
    public int TachometerMax
    {
        get => _data.Tachometer.Max;
        set => Change(ref _data.Tachometer.Max, value);
    }

    /// <summary>
    /// Gets or sets the X coordinates for the minimum indicated value of the
    /// tachometer.
    /// </summary>
    public int TachometerMinX
    {
        get => _data.Tachometer.MinX;
        set => Change(ref _data.Tachometer.MinX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinates for the minimum indicated value of the
    /// tachometer.
    /// </summary>
    public int TachometerMinY
    {
        get => _data.Tachometer.MinY;
        set => Change(ref _data.Tachometer.MinY, value);
    }

    /// <summary>
    /// Gets or sets the X coordinates for the maximum indicated value of the
    /// tachometer.
    /// </summary>
    public int TachometerMaxX
    {
        get => _data.Tachometer.Max;
        set => Change(ref _data.Tachometer.MaxX, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinates for the maximum indicated value of the
    /// tachometer.
    /// </summary>
    public int TachometerMaxY
    {
        get => _data.Tachometer.MaxY;
        set => Change(ref _data.Tachometer.MaxY, value);
    }

    /// <summary>
    /// Gets or sets the relative X coordinates for the rotation axis for the steering wheel.
    /// </summary>
    public int SteeringXRotation
    {
        get => _SteeringXRotation;
        set => Change(ref _SteeringXRotation, value);
    }

    /// <summary>
    /// Gets or sets the relative Y coordinates for the rotation axis for the steering wheel.
    /// </summary>
    public int SteeringYRotation
    {
        get => _SteeringYRotation;
        set => Change(ref _SteeringYRotation, value);
    }

    /// <summary>
    /// Gets or sets the absolute X coordinates for the position of the steering wheel.
    /// </summary>
    public int SteeringXPosition
    {
        get => _SteeringXPosition;
        set => Change(ref _SteeringXPosition, value);
    }

    /// <summary>
    /// Gets or sets the absolute Y coordinates for the position of the steering wheel.
    /// </summary>
    public int SteeringYPosition
    {
        get => _SteeringYPosition;
        set => Change(ref _SteeringYPosition, value);
    }

    /// <summary>
    /// Gets or sets the absolute X coordinates for the position of the gear indicator.
    /// </summary>
    public int GearXPosition
    {
        get => _GearXPosition;
        set => Change(ref _GearXPosition, value);
    }

    /// <summary>
    /// Gets or sets the absolute Y coordinates for the position of the gear indicator.
    /// </summary>
    public int GearYPosition
    {
        get => _GearYPosition;
        set => Change(ref _GearYPosition, value);
    }
}