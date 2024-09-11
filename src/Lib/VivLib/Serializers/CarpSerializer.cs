using System.Text;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Implements a serializer that can read and write <see cref="Carp"/>
/// entities.
/// </summary>
public class CarpSerializer : ISerializer<Carp>
{
    /// <inheritdoc/>
    public Carp Deserialize(Stream stream)
    {
        Dictionary<int, string> dic = [];
        int TryInt(string value) => int.TryParse(value, out var s) ? s : 0;
        double TryDouble(string value) => double.TryParse(value, out var s) ? s : 0.0;
        int TryIntKey(int key) => TryInt(dic[key]);
        double TryDoubleKey(int key) => TryDouble(dic[key]);
        IEnumerable<T> TryArray<T>(int key, Func<string, T> parse) => dic[key].Split(",").Where(p => !p.IsEmpty()).Select(parse);
        IEnumerable<int> TryIntArray(int key) => TryArray(key, TryInt);
        IEnumerable<double> TryDoubleArray(int key) => TryArray(key, TryDouble);
        int GetKey(string line) => int.TryParse(line.Split('(')[^1].ChopEnd(")"), out var k) ? k : -1;

        using (var r = new StreamReader(stream))
        {
            while (!r.EndOfStream)
            {
                var key = GetKey(r.ReadLine() ?? string.Empty);
                var value = r.ReadLine() ?? string.Empty;
                dic[key] = value;
            }
        }
        var carp = new Carp
        {
            SerialNumber = (ushort)TryIntKey(0),
            CarClass = (Nfs3CarClass)TryIntKey(1),
            Mass = TryDoubleKey(2),
            NumberOfGearsManual = TryIntKey(3),
            NumberOfGearsAuto = TryIntKey(75),
            GearShiftDelay = TryIntKey(4),
            FinalGearManual = TryDoubleKey(11),
            FinalGearAuto = TryDoubleKey(79),
            EngineMinRpm = TryIntKey(12),
            EngineMaxRpm = TryIntKey(13),
            MaxVelocity = TryDoubleKey(14),
            TopSpeed = TryDoubleKey(15),
            FrontDriveRatio = TryDoubleKey(16),
            Abs = TryIntKey(17) != 0,
            MaxBrakeDecel = TryDoubleKey(18),
            FrontBrakeBias = TryDoubleKey(19),
            WheelBase = TryDoubleKey(24),
            FrontGripBias = TryDoubleKey(25),
            PowerSteering = TryIntKey(26) != 0,
            MinimumSteerAccel = TryDoubleKey(27),
            TurnInRamp = TryDoubleKey(28),
            TurnOutRamp = TryDoubleKey(29),
            LateralAccGripMult = TryDoubleKey(30),
            AeroDownMult = TryDoubleKey(31),
            GasOffFactor = TryDoubleKey(32),
            GTransferFactor = TryDoubleKey(33),
            TurnCircleRadius = TryDoubleKey(34),
            TireWear = TryDoubleKey(37),
            SlideMult = TryDoubleKey(38),
            SpinVelocityCap = TryDoubleKey(39),
            SlideVelocityCap = TryDoubleKey(40),
            SlideAssistanceFactor = TryDoubleKey(41),
            PushFactor = TryDoubleKey(42),
            LowTurnFactor = TryDoubleKey(43),
            HighTurnFactor = TryDoubleKey(44),
            PitchRollFactor = TryDoubleKey(45),
            RoadBumpFactor = TryDoubleKey(46),
            SpoilerFunctionType = TryIntKey(47),
            SpoilerActivationSpeed = TryDoubleKey(48),
            GradualTurnCutoff = TryDoubleKey(49),
            MediumTurnCutoff = TryDoubleKey(50),
            SharpTurnCutoff = TryDoubleKey(51),
            MediumTurnSpdMod = TryDoubleKey(52),
            SharpTurnSpdMod = TryDoubleKey(53),
            ExtremeTurnSpdMod = TryDoubleKey(54),
            SubdivideLevel = TryDoubleKey(55),
            CameraArm = TryDoubleKey(56),
            BodyDamage = TryDoubleKey(57),
            EngineDamage = TryDoubleKey(58),
            SuspensionDamage = TryDoubleKey(59),
            EngineTuning = TryDoubleKey(60),
            BrakeBalance = TryDoubleKey(61),
            SteeringSpeed = TryDoubleKey(62),
            GearRatFactor = TryDoubleKey(63),
            SuspensionStiffness = TryDoubleKey(64),
            AeroFactor = TryDoubleKey(65),
            TireFactor = TryDoubleKey(66),
        };

        carp.ShiftBlip.AddRange(TryIntArray(5));
        carp.BrakeBlip.AddRange(TryIntArray(6));
        carp.VelocityToRpmManual.AddRange(TryDoubleArray(7));
        carp.VelocityToRpmAuto.AddRange(TryDoubleArray(76));
        carp.GearRatioManual.AddRange(TryDoubleArray(8));
        carp.GearRatioAuto.AddRange(TryDoubleArray(77));
        carp.GearEfficiencyManual.AddRange(TryDoubleArray(9));
        carp.GearEfficiencyAuto.AddRange(TryDoubleArray(78));
        carp.TorqueCurve.AddRange(TryDoubleArray(10));
        carp.GasIncreaseCurve.AddRange(TryIntArray(20));
        carp.GasDecreaseCurve.AddRange(TryIntArray(21));
        carp.BrakeIncreaseCurve.AddRange(TryDoubleArray(22));
        carp.BrakeDecreaseCurve.AddRange(TryDoubleArray(23));
        carp.AiCurve0.AddRange(TryDoubleArray(67));
        carp.AiCurve1.AddRange(TryDoubleArray(68));
        carp.AiCurve2.AddRange(TryDoubleArray(69));
        carp.AiCurve3.AddRange(TryDoubleArray(70));
        carp.AiCurve4.AddRange(TryDoubleArray(71));
        carp.AiCurve5.AddRange(TryDoubleArray(72));
        carp.AiCurve6.AddRange(TryDoubleArray(73));
        carp.AiCurve7.AddRange(TryDoubleArray(74));
        var tirespecs = TryIntArray(35).ToArray();
        if (tirespecs.Length == 3)
        {
            carp.TireWidthFront = tirespecs[0];
            carp.TireSidewallFront = tirespecs[1];
            carp.TireRimFront = tirespecs[2];
        }
        tirespecs = TryIntArray(36).ToArray();
        if (tirespecs.Length == 3)
        {
            carp.TireWidthRear = tirespecs[0];
            carp.TireSidewallRear = tirespecs[1];
            carp.TireRimRear = tirespecs[2];
        }
        return carp;
    }

    /// <inheritdoc/>
    public void SerializeTo(Carp entity, Stream stream)
    {
        stream.WriteBytes(Encoding.Latin1.GetBytes($"""
        Serial Number(0)
        {entity.SerialNumber}
        Car Classification(1)
        {(int)entity.CarClass}
        mass [kg](2)
        {entity.Mass}
        number of gears (reverse + neutral + forward gears)(3)
        {entity.NumberOfGearsManual}
        number of gears (automatic, r, n, forward)(75)
        {entity.NumberOfGearsAuto}
        gear shift delay (ticks)(4)
        {entity.GearShiftDelay}
        shift blip in rpm (size {entity.ShiftBlip.Count})(5)
        {string.Join(",", entity.ShiftBlip)}
        brake blip in rpm (size {entity.BrakeBlip.Count})(6)
        {string.Join(",", entity.BrakeBlip)}
        velocity to rpm ratio (size {entity.VelocityToRpmManual.Count})(7)
        {string.Join(",", entity.VelocityToRpmManual)}
        velocity to rpm ratio (size {entity.VelocityToRpmAuto.Count})(76)
        {string.Join(",", entity.VelocityToRpmAuto)}
        gear ratios (size {entity.GearRatioManual.Count})(8)
        {string.Join(",", entity.GearRatioManual)}
        gear ratios automatic (size {entity.GearRatioAuto.Count})(77)
        {string.Join(",", entity.GearRatioAuto)}
        gear efficiency (size {entity.GearEfficiencyManual.Count})(9)
        {string.Join(",", entity.GearEfficiencyManual)}
        gear efficiency automatic (size {entity.GearEfficiencyAuto.Count})(78)
        {string.Join(",", entity.GearEfficiencyAuto)}
        torque curve (size {entity.TorqueCurve.Count}) in {(entity.TorqueCurve.Count != 0 ? (entity.EngineMaxRpm / entity.TorqueCurve.Count).Clamp(256, entity.EngineMaxRpm) : 0)} rpm increments(10)
        {string.Join(",", entity.TorqueCurve)}
        final gear(11)
        {entity.FinalGearManual}
        final gear automatic(79)
        {entity.FinalGearAuto}
        engine minimum rpm(12)
        {entity.EngineMinRpm}
        engine redline in rpm(13)
        {entity.EngineMaxRpm}
        Maximum velocity of car [m/s](14)
        {entity.MaxVelocity}
        top speed cap [m/s](15)
        {entity.TopSpeed}
        front drive ratio(16)
        {entity.FrontDriveRatio}
        Uses Antilock Brake System(17)
        {(entity.Abs ? 1 : 0)}
        Maximum braking deceleration(18)
        {entity.MaxBrakeDecel}
        front bias brake ratio(19)
        {entity.FrontBrakeBias}
        gas increasing curve(20)
        {string.Join(",", entity.GasIncreaseCurve)}
        gas decreasing curve(21)
        {string.Join(",", entity.GasDecreaseCurve)}
        brake increasing curve(22)
        {string.Join(",", entity.BrakeIncreaseCurve)}
        brake decreasing curve(23)
        {string.Join(",", entity.BrakeDecreaseCurve)}
        wheel base(24)
        {entity.WheelBase}
        front grip bias(25)
        {entity.FrontGripBias}
        power steering (boolean)(26)
        {(entity.PowerSteering ? 1 : 0)}
        minimum steering acceleration(27)
        {entity.MinimumSteerAccel}
        turn in ramp(28)
        {entity.TurnInRamp}
        turn out ramp(29)
        {entity.TurnOutRamp}
        lateral acceleration grip multiplier(30)
        {entity.LateralAccGripMult}
        aerodynamic downforce multiplier(31)
        {entity.AeroDownMult}
        gas off factor(32)
        {entity.GasOffFactor}
        g transfer factor(33)
        {entity.GTransferFactor}
        turning circle radius(34)
        {entity.TurnCircleRadius}
        tire specs front(35)
        {entity.TireWidthFront},{entity.TireSidewallFront},{entity.TireRimFront}
        tire specs rear(36)
        {entity.TireWidthRear},{entity.TireSidewallRear},{entity.TireRimRear}
        tire wear(37)
        {entity.TireWear}
        Slide Multiplier(38)
        {entity.SlideMult}
        Spin Velocity Cap(39)
        {entity.SpinVelocityCap}
        Slide Velocity Cap(40)
        {entity.SlideVelocityCap}
        Slide Assistance Factor(41)
        {entity.SlideAssistanceFactor}
        Push Factor(42)
        {entity.PushFactor}
        Low Turn Factor (the lower the figure, the better the turn)(43)
        {entity.LowTurnFactor}
        High Turn Factor (the lower the figure, the better the turn)(44)
        {entity.HighTurnFactor}
        pitch roll factor(45)
        {entity.PitchRollFactor}
        road bumpiness factor(46)
        {entity.RoadBumpFactor}
        spoiler function type(47)
        {entity.SpoilerFunctionType}
        spoiler activation speed [m/s](48)
        {entity.SpoilerActivationSpeed}
        gradual turn cutoff(49)
        {entity.GradualTurnCutoff}
        medium turn cutoff(50)
        {entity.MediumTurnCutoff}
        sharp turn cutoff(51)
        {entity.SharpTurnCutoff}
        medium turn speed modifier(52)
        {entity.MediumTurnSpdMod}
        sharp turn speed modifier(53)
        {entity.SharpTurnSpdMod}
        extreme turn speed modifier(54)
        {entity.ExtremeTurnSpdMod}
        subdivide level(55)
        {entity.SubdivideLevel}
        camera arm(56)
        {entity.CameraArm}
        Body Damage(57)
        {entity.BodyDamage}
        Engine Damage(58)
        {entity.EngineDamage}
        Suspension Damage(59)
        {entity.SuspensionDamage}
        Engine Tuning(60)
        {entity.EngineTuning}
        Brake Balance(61)
        {entity.BrakeBalance}
        Steering TopSpeed(62)
        {entity.SteeringSpeed}
        Gear Rat Factor(63)
        {entity.GearRatFactor}
        Suspension Stiffness(64)
        {entity.SuspensionStiffness}
        Aero Factor(65)
        {entity.AeroFactor}
        Tire Factor(66)
        {entity.TireFactor}
        AI ACC0 acceleration table section(67)
        {string.Join(",", entity.AiCurve0)}
        AI ACC1 acceleration table section(68)
        {string.Join(",", entity.AiCurve1)}
        AI ACC2 acceleration table section(69)
        {string.Join(",", entity.AiCurve2)}
        AI ACC3 acceleration table section(70)
        {string.Join(",", entity.AiCurve3)}
        AI ACC4 acceleration table section(71)
        {string.Join(",", entity.AiCurve4)}
        AI ACC5 acceleration table section(72)
        {string.Join(",", entity.AiCurve5)}
        AI ACC6 acceleration table section(73)
        {string.Join(",", entity.AiCurve6)}
        AI ACC7 acceleration table section(74)
        {string.Join(",", entity.AiCurve7)}
        """));
    }
}
