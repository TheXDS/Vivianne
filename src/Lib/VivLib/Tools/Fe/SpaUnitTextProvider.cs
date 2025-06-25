using System.Globalization;
using TheXDS.Vivianne.Models.Carp;

namespace TheXDS.Vivianne.Tools.Fe;

/// <summary>
/// Implements a <see cref="FeDataTextProvider"/> for SPA fedata files.
/// </summary>
/// <param name="carp">Performance data source.</param>
public class SpaUnitTextProvider(ICarPerf carp) : FeDataTextProvider(carp, CultureInfo.GetCultureInfo("es-ES"))
{
    /// <inheritdoc/>
    public override string TopSpeed => $"{(CarpData.TopSpeed * 3.6).ToString("0", Culture)} Km/h";

    /// <inheritdoc/>
    public override string Power
    {
        get
        {
            var (hp, rpm) = Analysis.MaxPower;
            return $"{(hp * 0.9862).ToString("0", Culture)} cv a {rpm.ToString(Culture)} RPM";
        }
    }

    /// <inheritdoc/>
    public override string Torque
    {
        get
        {
            var (torque, rpm) = Analysis.MaxTorque;
            return $"{(torque * 0.138254954376).ToString("0", Culture)} mkg a {rpm.ToString(Culture)} RPM";
        }
    }

    /// <inheritdoc/>
    public override string Gearbox
    {
        get
        {
            return CarpData.NumberOfGearsAuto == CarpData.NumberOfGearsManual
                ? $"{CarpData.NumberOfGearsManual - 2} velocidades"
                : $"{CarpData.NumberOfGearsManual - 2} velocidades (manual) / {CarpData.NumberOfGearsAuto - 2} velocidades (automática)";
        }
    }

    /// <inheritdoc/>
    public override string Accel0To60 => $"{Analysis.EstimateAcceleration(60).ToString("0.0", Culture)} seg.";

    /// <inheritdoc/>
    public override string Accel0To100 => $"{Analysis.EstimateAcceleration(100).ToString("0.0", Culture)} seg.";
}
