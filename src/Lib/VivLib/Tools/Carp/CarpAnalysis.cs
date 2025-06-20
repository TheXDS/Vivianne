﻿using TheXDS.MCART.Math;
using TheXDS.Vivianne.Models.Carp;

namespace TheXDS.Vivianne.Tools.Carp;

/// <summary>
/// Analyzes the performance data from a <see cref="ICarPerf"/>.
/// </summary>
public class CarpAnalysis
{
    private readonly ICarPerf carp;

    /// <summary>
    /// Initializes a new instance of the <see cref="CarpAnalysis"/> class.
    /// </summary>
    /// <param name="carp"><see cref="ICarPerf"/> to analyze.</param>
    public CarpAnalysis(ICarPerf carp)
    {
        var torqueWithRpmCurve = TorqueWithRpmCurve(carp);
        MaxTorque = torqueWithRpmCurve.MaxBy(p => p.torque);
        MaxPower = torqueWithRpmCurve
            .Select(p => (p.torque * p.rpm / 5252, p.rpm))
            .MaxBy(p => p.Item1);
        this.carp = carp;
    }

    /// <summary>
    /// Gets the maximum produced torque value in lb-ft and the RPM at which it is reached.
    /// </summary>
    public (double Torque, int Rpm) MaxTorque { get; }

    /// <summary>
    /// Gets the maximum produced horsepower value and the RPM at which it is reached.
    /// </summary>
    public (double Hp, int Rpm) MaxPower { get; }
    
    /// <summary>
    /// Estimates the time to accelerate from 0 to the specified speed.
    /// </summary>
    /// <param name="targetMphSpeed">Target speed, in MPH</param>
    /// <param name="withShiftDelay">
    /// If set to <see langword="true"/>, calculations will use the median
    /// torque instead of the maximum, as well as include delays per each
    /// required gear shift. If set to <see langword="false"/>, calculations
    /// will use the maximum torque instead, and no gear shift delays will be
    /// included.
    /// </param>
    /// <returns>
    /// The estimated time in seconds required to reach the specified speed.
    /// </returns>
    public double EstimateAcceleration(double targetMphSpeed, bool withShiftDelay = false)
    {
        var shiftDelay = withShiftDelay ? carp.GearShiftDelay * carp.VelocityToRpmManual.Count(p => carp.EngineMaxRpm / p > targetMphSpeed * 0.44704) : 0.0;
        var torque = withShiftDelay ? carp.TorqueCurve.Median() : carp.TorqueCurve.Max();
        const double rollingResistance = 0.02;
        var a = torque / carp.Mass - rollingResistance;
        return Math.Sqrt(2 * 0.3048 * targetMphSpeed / a) + shiftDelay / 1000.0;
    }

    private static (double torque, int rpm)[] TorqueWithRpmCurve(ICarPerf carp)
    {
        var rpmStep = (carp.EngineMaxRpm / carp.TorqueCurve.Count).Clamp(256, carp.EngineMaxRpm);
        return [.. carp.TorqueCurve
            .Zip(Enumerable.Range(0, carp.TorqueCurve.Count).Select(p => p * rpmStep + carp.EngineMinRpm))
            .Where(p => p.Second <= carp.EngineMaxRpm)];
    }
}
