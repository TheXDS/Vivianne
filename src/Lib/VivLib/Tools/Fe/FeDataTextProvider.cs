using System.Globalization;
using TheXDS.Vivianne.Models.Carp;
using TheXDS.Vivianne.Tools.Carp;

namespace TheXDS.Vivianne.Tools.Fe;

/// <summary>
/// Base class for all FeData text providers.
/// </summary>
/// <param name="source"><see cref="ICarPerf"/> performance data source.</param>
/// <param name="culture">
/// Optional <see cref="CultureInfo"/> to use when formatting numbers.
/// </param>
public abstract class FeDataTextProvider(ICarPerf source, CultureInfo? culture = null)
{
    /// <summary>
    /// Gets a reference to the culture to use on this provider.
    /// </summary>
    protected CultureInfo Culture { get; } = culture ?? CultureInfo.InvariantCulture;

    /// <summary>
    /// Gets a reference to the <see cref="ICarPerf"/> data to get information
    /// from.
    /// </summary>
    protected ICarPerf CarpData { get; } = source;

    /// <summary>
    /// Gets a reference to a performance analysis instance to get some
    /// performance metrics from.
    /// </summary>
    protected CarpAnalysis Analysis { get; } = new(source);

    /// <summary>
    /// Gets a string that describes the weight of the vehicle.
    /// </summary>
    public virtual string Weight => $"{CarpData.Mass.ToString("0", Culture)} Kg";

    /// <summary>
    /// Gets a string that describes the top speed of the vehicle.
    /// </summary>
    public virtual string TopSpeed => $"{CarpData.TopSpeed.ToString("0", Culture)} m/s";

    /// <summary>
    /// Gets a string that describes the maximum power of the vehicle.
    /// </summary>
    public virtual string Power
    {
        get
        {
            var (hp, rpm) = Analysis.MaxPower;
            return $"{hp.ToString("0", Culture)} HP @ {rpm.ToString(Culture)} RPM";
        }
    }

    /// <summary>
    /// Gets a string that describes the maximum torque of the vehicle.
    /// </summary>
    public virtual string Torque
    {
        get
        {
            var (torque, rpm) = Analysis.MaxTorque;
            return $"{torque.ToString("0", Culture)} lb-ft @ {rpm.ToString(Culture)} RPM";
        }
    }

    /// <summary>
    /// Gets a string that describes the maximum RPM the engine can reach.
    /// </summary>
    public virtual string MaxRpm => $"{CarpData.EngineMaxRpm.ToString(Culture)} RPM";
    
    /// <summary>
    /// Gets a string that describes the tire specs.
    /// </summary>
    public virtual string Tires => $"{CarpData.TireWidthFront}/{CarpData.TireSidewallFront}R{CarpData.TireRimFront}, {CarpData.TireWidthRear}/{CarpData.TireSidewallRear}R{CarpData.TireRimRear}";
    
    /// <summary>
    /// Gets a string that describes the number of gears in the gearbox.
    /// </summary>
    public virtual string Gearbox
    {
        get
        {
            return CarpData.NumberOfGearsAuto == CarpData.NumberOfGearsManual
                ? $"{CarpData.NumberOfGearsManual - 2} speed"
                : $"{CarpData.NumberOfGearsManual - 2} speed (manual) / {CarpData.NumberOfGearsAuto - 2} speed (auto)";
        }
    }

    /// <summary>
    /// Gets a string that describes the 0 to 60 MPH acceleration time.
    /// </summary>
    public virtual string Accel0To60 => $"{Analysis.EstimateAcceleration(60).ToString("0.0", Culture)} sec";

    /// <summary>
    /// Gets a string that describes the 0 to 100 MPH acceleration time.
    /// </summary>
    public virtual string Accel0To100 => $"{Analysis.EstimateAcceleration(100).ToString("0.0", Culture)} sec";
}
