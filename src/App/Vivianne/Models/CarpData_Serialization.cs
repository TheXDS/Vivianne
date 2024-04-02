using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Models;

public partial class CarpEditorState
{
    /// <summary>
    /// Creates a new instance of the <see cref="CarpEditorState"/> class from
    /// a string containing Carp data.
    /// </summary>
    /// <param name="rawData">Raw Carp data.</param>
    /// <returns>
    /// A new instance of the <see cref="CarpEditorState"/> class.
    /// </returns>
    public static CarpEditorState From(string rawData)
    {
        static string SkipRead(TextReader r)
        {
            _ = r.ReadLine();
            return r.ReadLine()!;
        }
        static IEnumerable<string> SkipReadCollection(TextReader r) => SkipRead(r).Split(",").Where(p => !p.IsEmpty());
        static IEnumerable<int> SkipReadIntCollection(TextReader r) => SkipReadCollection(r).Select(int.Parse);
        static IEnumerable<double> SkipReadDoubleCollection(TextReader r) => SkipReadCollection(r).ToArray().Select(double.Parse);

        using var r = new StringReader(rawData);
        var result = new CarpEditorState()
        {
            _SerialNumber = int.Parse(SkipRead(r)),
            _CarClass = (Nfs3CarClass)int.Parse(SkipRead(r)),
            _Mass = double.Parse(SkipRead(r)),
            _NumberOfGearsManual = int.Parse(SkipRead(r)),
            _NumberOfGearsAuto = int.Parse(SkipRead(r)),
            _GearShiftDelay = int.Parse(SkipRead(r)),
        };
        result.ShiftBlip.AddRange(SkipReadIntCollection(r));
        result.BrakeBlip.AddRange(SkipReadIntCollection(r));
        result.VelocityToRpmManual.AddRange(SkipReadDoubleCollection(r));
        result.VelocityToRpmAuto.AddRange(SkipReadDoubleCollection(r));
        result.GearRatioManual.AddRange(SkipReadDoubleCollection(r));
        result.GearRatioAuto.AddRange(SkipReadDoubleCollection(r));
        result.GearEfficiencyManual.AddRange(SkipReadDoubleCollection(r));
        result.GearEfficiencyAuto.AddRange(SkipReadDoubleCollection(r));
        result.TorqueCurve.AddRange(SkipReadDoubleCollection(r));
        result._FinalGearManual = double.Parse(SkipRead(r));
        result._FinalGearAuto = double.Parse(SkipRead(r));
        result._EngineMinRpm = int.Parse(SkipRead(r));
        result._EngineMinRpm = int.Parse(SkipRead(r));
        result._MaxVelocity = double.Parse(SkipRead(r));
        result._TopSpeed = double.Parse(SkipRead(r));
        result._FrontDriveRatio = double.Parse(SkipRead(r));
        result._Abs = int.Parse(SkipRead(r)) != 0;
        result._MaxBrakeDecel = double.Parse(SkipRead(r));
        result._FrontBrakeBias = double.Parse(SkipRead(r));
        result.GasIncreaseCurve.AddRange(SkipReadIntCollection(r));
        result.GasDecreaseCurve.AddRange(SkipReadIntCollection(r));
        result.BrakeIncreaseCurve.AddRange(SkipReadDoubleCollection(r));
        result.BrakeDecreaseCurve.AddRange(SkipReadDoubleCollection(r));
        result._WheelBase = double.Parse(SkipRead(r));
        result._FrontGripBias = double.Parse(SkipRead(r));
        result._PowerSteering = int.Parse(SkipRead(r)) != 0;
        result._MinimumSteerAccel = double.Parse(SkipRead(r));
        result._TurnInRamp = double.Parse(SkipRead(r));
        result._TurnOutRamp = double.Parse(SkipRead(r));
        result._LateralAccGripMult = double.Parse(SkipRead(r));
        result._AeroDownMult = double.Parse(SkipRead(r));
        result._GasOffFactor = double.Parse(SkipRead(r));
        result._GTransferFactor = double.Parse(SkipRead(r));
        result._TurnCircleRadius = double.Parse(SkipRead(r));
        var tirespecs = SkipReadIntCollection(r).ToArray();
        if (tirespecs.Length == 3)
        {
            result._TireWidthFront = tirespecs[0];
            result._TireSidewallFront = tirespecs[1];
            result._TireRimFront = tirespecs[2];
        }
        tirespecs = SkipReadIntCollection(r).ToArray();
        if (tirespecs.Length == 3)
        {
            result._TireWidthRear = tirespecs[0];
            result._TireSidewallRear = tirespecs[1];
            result._TireRimRear = tirespecs[2];
        }
        result._TireWear = double.Parse(SkipRead(r));
        result._SlideMult = double.Parse(SkipRead(r));
        result._SpinVelocityCap = double.Parse(SkipRead(r));
        result._SlideVelocityCap = double.Parse(SkipRead(r));
        result._SlideAssistanceFactor = double.Parse(SkipRead(r));
        result._PushFactor = double.Parse(SkipRead(r));
        result._LowTurnFactor = double.Parse(SkipRead(r));
        result._HighTurnFactor = double.Parse(SkipRead(r));
        result._PitchRollFactor = double.Parse(SkipRead(r));
        result._RoadBumpFactor = double.Parse(SkipRead(r));
        result._SpoilerFunctionType = int.Parse(SkipRead(r));
        result._SpoilerActivationSpeed = double.Parse(SkipRead(r));
        result._GradualTurnCutoff = double.Parse(SkipRead(r));
        result._MediumTurnCutoff = double.Parse(SkipRead(r));
        result._SharpTurnCutoff = double.Parse(SkipRead(r));
        result._MediumTurnSpdMod = double.Parse(SkipRead(r));
        result._SharpTurnSpdMod = double.Parse(SkipRead(r));
        result._ExtremeTurnSpdMod = double.Parse(SkipRead(r));
        result._SubdivideLevel = double.Parse(SkipRead(r));
        result._CameraArm = double.Parse(SkipRead(r));
        result._BodyDamage = double.Parse(SkipRead(r));
        result._EngineDamage = double.Parse(SkipRead(r));
        result._SuspensionDamage = double.Parse(SkipRead(r));
        result._EngineTuning = double.Parse(SkipRead(r));
        result._BrakeBalance = double.Parse(SkipRead(r));
        result._SteeringSpeed = double.Parse(SkipRead(r));
        result._GearRatFactor = double.Parse(SkipRead(r));
        result._SuspensionStiffness = double.Parse(SkipRead(r));
        result._AeroFactor = double.Parse(SkipRead(r));
        result._TireFactor = double.Parse(SkipRead(r));
        result.AiCurve0.AddRange(SkipReadDoubleCollection(r));
        result.AiCurve1.AddRange(SkipReadDoubleCollection(r));
        result.AiCurve2.AddRange(SkipReadDoubleCollection(r));
        result.AiCurve3.AddRange(SkipReadDoubleCollection(r));
        result.AiCurve4.AddRange(SkipReadDoubleCollection(r));
        result.AiCurve5.AddRange(SkipReadDoubleCollection(r));
        result.AiCurve6.AddRange(SkipReadDoubleCollection(r));
        result.AiCurve7.AddRange(SkipReadDoubleCollection(r));
        return result;
    }

    /// <inheritdoc/>
    public override string ToString() => $"""
        Serial Number (0)
        {SerialNumber}
        Car Classification (1)
        {(int)CarClass}
        mass [kg](2)
        {Mass}
        number of gears (reverse + neutral + forward gears)(3)
        {NumberOfGearsManual}
        number of gears (automatic, r, n, forward)(75)
        {NumberOfGearsAuto}
        gear shift delay (ticks)(4)
        {GearShiftDelay}
        shift blip in rpm (size {ShiftBlip.Count})(5)
        {string.Join(",", ShiftBlip)}
        brake blip in rpm (size {BrakeBlip.Count})(6)
        {string.Join(",", BrakeBlip)}
        velocity to rpm ratio (size {VelocityToRpmManual.Count})(7)
        {string.Join(",", VelocityToRpmManual)}
        velocity to rpm ratio (size {VelocityToRpmAuto.Count})(76)
        {string.Join(",", VelocityToRpmAuto)}
        gear ratios (size {GearRatioManual.Count})(8)
        {string.Join(",", GearRatioManual)}
        gear ratios automatic (size {GearRatioAuto.Count})(77)
        {string.Join(",", GearRatioAuto)}
        gear efficiency (size {GearEfficiencyManual.Count})(9)
        {string.Join(",", GearEfficiencyManual)}
        gear efficiency automatic (size {GearEfficiencyAuto.Count})(78)
        {string.Join(",", GearEfficiencyAuto)}
        torque curve (size {TorqueCurve.Count}) in {(TorqueCurve.Count != 0 ? EngineMaxRpm / TorqueCurve.Count : 0)} rpm increments(10)
        {string.Join(",", TorqueCurve)}
        final gear(11)
        {FinalGearManual}
        final gear automatic(79)
        {FinalGearAuto}
        engine minimum rpm(12)
        {EngineMinRpm}
        engine redline in rpm(13)
        {EngineMaxRpm}
        Maximum velocity of car [m/s](14)
        {MaxVelocity}
        top speed cap [m/s](15)
        {TopSpeed}
        front drive ratio(16)
        {FrontDriveRatio}
        Uses Antilock Brake System(17)
        {(Abs ? 1 : 0)}
        Maximum braking deceleration(18)
        {MaxBrakeDecel}
        front bias brake ratio(19)
        {FrontBrakeBias}
        gas increasing curve(20)
        {string.Join(",", GasIncreaseCurve)}
        gas decreasing curve(21)
        {string.Join(",", GasDecreaseCurve)}
        brake increasing curve(22)
        {string.Join(",", BrakeIncreaseCurve)}
        brake decreasing curve(23)
        {string.Join(",", BrakeDecreaseCurve)}
        wheel base(24)
        {WheelBase}
        front grip bias(25)
        {FrontGripBias}
        power steering (boolean)(26)
        {(PowerSteering ? 1 : 0)}
        minimum steering acceleration(27)
        {MinimumSteerAccel}
        turn in ramp(28)
        {TurnInRamp}
        turn out ramp(29)
        {TurnOutRamp}
        lateral acceleration grip multiplier(30)
        {LateralAccGripMult}
        aerodynamic downforce multiplier(31)
        {AeroDownMult}
        gas off factor(32)
        {GasOffFactor}
        g transfer factor(33)
        {GTransferFactor}
        turning circle radius(34)
        {TurnCircleRadius}
        tire specs front(35)
        {TireWidthFront},{TireSidewallFront},{TireRimFront}
        tire specs rear(36)
        {TireWidthRear},{TireSidewallRear},{TireRimRear}
        tire wear(37)
        {TireWear}
        Slide Multiplier(38)
        {SlideMult}
        Spin Velocity Cap(39)
        {SpinVelocityCap}
        Slide Velocity Cap(40)
        {SlideVelocityCap}
        Slide Assistance Factor(41)
        {SlideAssistanceFactor}
        Push Factor(42)
        {PushFactor}
        Low Turn Factor (the lower the figure, the better the turn)(43)
        {LowTurnFactor}
        High Turn Factor (the lower the figure, the better the turn)(44)
        {HighTurnFactor}
        pitch roll factor(45)
        {PitchRollFactor}
        road bumpiness factor(46)
        {RoadBumpFactor}
        spoiler function type(47)
        {SpoilerFunctionType}
        spoiler activation speed [m/s](48)
        {SpoilerActivationSpeed}
        gradual turn cutoff(49)
        {GradualTurnCutoff}
        medium turn cutoff(50)
        {MediumTurnCutoff}
        sharp turn cutoff(51)
        {SharpTurnCutoff}
        medium turn speed modifier(52)
        {MediumTurnSpdMod}
        sharp turn speed modifier(53)
        {SharpTurnSpdMod}
        extreme turn speed modifier(54)
        {ExtremeTurnSpdMod}
        subdivide level(55)
        {SubdivideLevel}
        camera arm(56)
        {CameraArm}
        Body Damage(57)
        {BodyDamage}
        Engine Damage(58)
        {EngineDamage}
        Suspension Damage(59)
        {SuspensionDamage}
        Engine Tuning(60)
        {EngineTuning}
        Brake Balance(61)
        {BrakeBalance}
        Steering Speed(62)
        {SteeringSpeed}
        Gear Rat Factor(63)
        {GearRatFactor}
        Suspension Stiffness(64)
        {SuspensionStiffness}
        Aero Factor(65)
        {AeroFactor}
        Tire Factor(66)
        {TireFactor}
        AI ACC0 acceleration table section(67)
        {string.Join(",", AiCurve0)}
        AI ACC1 acceleration table section(68)
        {string.Join(",", AiCurve1)}
        AI ACC2 acceleration table section(69)
        {string.Join(",", AiCurve2)}
        AI ACC3 acceleration table section(70)
        {string.Join(",", AiCurve3)}
        AI ACC4 acceleration table section(71)
        {string.Join(",", AiCurve4)}
        AI ACC5 acceleration table section(72)
        {string.Join(",", AiCurve5)}
        AI ACC6 acceleration table section(73)
        {string.Join(",", AiCurve6)}
        AI ACC7 acceleration table section(74)
        {string.Join(",", AiCurve7)}
        """;
}
