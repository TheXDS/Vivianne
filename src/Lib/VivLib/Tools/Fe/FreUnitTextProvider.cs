﻿using System.Globalization;
using TheXDS.Vivianne.Models.Carp;

namespace TheXDS.Vivianne.Tools.Fe;

/// <summary>
/// Implements a <see cref="FeDataTextProvider"/> for FRE fedata files.
/// </summary>
/// <param name="carp">Performance data source.</param>
public class FreUnitTextProvider(ICarPerf carp) : FeDataTextProvider(carp, CultureInfo.GetCultureInfo("fr-FR"))
{
    /// <inheritdoc/>
    public override string Weight => CarpData.Mass.ToString("0", Culture);

    /// <inheritdoc/>
    public override string TopSpeed => (CarpData.TopSpeed * 3.6).ToString("0", Culture);

    /// <inheritdoc/>
    public override string Power
    {
        get
        {
            var (hp, rpm) = Analysis.MaxPower;
            return $"{hp.ToString("0", Culture)} à {rpm.ToString(Culture)}";
        }
    }

    /// <inheritdoc/>
    public override string Torque
    {
        get
        {
            var (torque, rpm) = Analysis.MaxTorque;
            return $"{torque.ToString("0", Culture)} à {rpm.ToString(Culture)}";
        }
    }

    /// <inheritdoc/>
    public override string MaxRpm => CarpData.EngineMaxRpm.ToString(Culture);

    /// <inheritdoc/>
    public override string Gearbox
    {
        get
        {
            return CarpData.NumberOfGearsAuto == CarpData.NumberOfGearsManual
                ? (CarpData.NumberOfGearsManual - 2).ToString(Culture)
                : $"{CarpData.NumberOfGearsManual - 2} (manuelle) / {CarpData.NumberOfGearsAuto - 2} (auto)";
        }
    }

    /// <inheritdoc/>
    public override string Accel0To60 => Analysis.EstimateAcceleration(60).ToString("0.0", Culture);

    /// <inheritdoc/>
    public override string Accel0To100 => Analysis.EstimateAcceleration(100).ToString("0.0", Culture);
}
