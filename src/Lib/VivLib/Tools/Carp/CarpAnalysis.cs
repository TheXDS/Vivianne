using System.Reflection.Metadata.Ecma335;
using TheXDS.MCART.Math;
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
    /// <param name="withShiftDelay">Indicates whether to include gear shift delay in the calculation.</param>
    /// <returns>
    /// The estimated time in seconds required to reach the specified speed.
    /// </returns>
    public double EstimateAcceleration(double targetMphSpeed, bool withShiftDelay = true)
    {
        double mass = carp.Mass;
        double timeElapsed = 0.0;
        double speed = 0.0;
        double targetSpeed = targetMphSpeed * 0.44704;
        int gear = 2;
        double wheelRadius = GetWheelRadius();

        var torqueCurve = TorqueWithRpmCurve(carp);
        double shiftDelay = withShiftDelay ? (carp.GearShiftDelay / 60.0) : 0.0;

        while (speed < targetSpeed && gear < carp.GearRatioManual.Count)
        {
            double rpm = (speed * 60.0) / (2 * Math.PI * wheelRadius) * carp.GearRatioManual[gear] * carp.FinalGearManual;
            rpm = Math.Clamp(rpm, carp.EngineMinRpm, carp.EngineMaxRpm);
            var (torque, _) = torqueCurve.OrderBy(p => Math.Abs(p.rpm - rpm)).First();
            double gearEff = carp.GearEfficiencyManual.Count > gear ? carp.GearEfficiencyManual[gear] : 1.0;
            double force = (torque * carp.GearRatioManual[gear] * carp.FinalGearManual * gearEff) / wheelRadius;
            double acceleration = force / mass;
            if (acceleration <= 0.01)
            {
                gear++;
                if (withShiftDelay) timeElapsed += shiftDelay;
                continue;
            }
            double dt = 0.05;
            double nextSpeed = speed + (acceleration * dt);
            if (nextSpeed > targetSpeed) dt = (targetSpeed - speed) / acceleration;
            timeElapsed += dt;
            speed += acceleration * dt;
            double nextRpm = (speed * 60.0) / (2 * Math.PI * wheelRadius) * carp.GearRatioManual[gear] * carp.FinalGearManual;
            if (nextRpm > carp.EngineMaxRpm && gear + 1 < carp.GearRatioManual.Count)
            {
                gear++;
                if (withShiftDelay) timeElapsed += shiftDelay;
            }
        }
        return timeElapsed;
    }

    private double GetWheelRadius()
    {
        // Use rear tire by default
        int width = carp.TireWidthRear > 0 ? carp.TireWidthRear : 205;
        int aspect = carp.TireSidewallRear > 0 ? carp.TireSidewallRear : 55;
        int rim = carp.TireRimRear > 0 ? carp.TireRimRear : 16;
        double sidewall = width * (aspect / 100.0);
        double diameter = (rim * 25.4) + (2 * sidewall);
        return diameter / 2000.0;
    }

    private static (double torque, int rpm)[] TorqueWithRpmCurve(ICarPerf carp)
    {
        var rpmStep = (carp.EngineMaxRpm / carp.TorqueCurve.Count).Clamp(1, carp.EngineMaxRpm);
        return [.. carp.TorqueCurve
            .Zip(Enumerable.Range(0, carp.TorqueCurve.Count).Select(p => (p * rpmStep) + carp.EngineMinRpm))
            .Where(p => p.Second <= carp.EngineMaxRpm)];
    }
}
