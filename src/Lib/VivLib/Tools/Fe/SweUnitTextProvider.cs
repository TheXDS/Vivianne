using System.Globalization;
using TheXDS.Vivianne.Models.Carp;

namespace TheXDS.Vivianne.Tools.Fe;

/// <summary>
/// Implements a <see cref="FeDataTextProvider"/> for SWE fedata files.
/// </summary>
/// <param name="carp">Performance data source.</param>
public class SweUnitTextProvider(ICarPerf carp) : FeDataTextProvider(carp, CultureInfo.GetCultureInfo("sv-SW"))
{
    /// <inheritdoc/>
    public override string TopSpeed => $"{(CarpData.TopSpeed * 3.6).ToString("0", Culture)} Km/h";

    /// <inheritdoc/>
    public override string Power
    {
        get
        {
            var (hp, rpm) = Analysis.MaxPower;
            return $"{(hp * 0.9862).ToString("0", Culture)} vid {rpm.ToString(Culture)} v/min";
        }
    }

    /// <inheritdoc/>
    public override string Torque
    {
        get
        {
            var (torque, rpm) = Analysis.MaxTorque;
            return $"{(torque * 1.3558179483).ToString("0", Culture)} Nm vid {rpm.ToString(Culture)} v/min";
        }
    }

    /// <inheritdoc/>
    public override string MaxRpm => $"{CarpData.EngineMaxRpm.ToString(Culture)} v/min";

    /// <inheritdoc/>
    public override string Gearbox
    {
        get
        {
            return CarpData.NumberOfGearsAuto == CarpData.NumberOfGearsManual
                ? $"{CarpData.NumberOfGearsManual - 2}"
                : $"{CarpData.NumberOfGearsManual - 2} (manuell) / {CarpData.NumberOfGearsAuto - 2} (automat)";
        }
    }

    /// <inheritdoc/>
    public override string Accel0To60 => $"{Analysis.EstimateAcceleration(60).ToString("0.0", Culture)} sek.";

    /// <inheritdoc/>
    public override string Accel0To100 => $"{Analysis.EstimateAcceleration(100).ToString("0.0", Culture)} sek.";
}
