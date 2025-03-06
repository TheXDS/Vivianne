using System.Text;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Carp;

namespace TheXDS.Vivianne.Serializers.Carp;

/// <summary>
/// Implements a serializer that can read and write <see cref="Carp"/>
/// entities.
/// </summary>
public class CarpSerializerBase<TCarClass, TFile> : ISerializer<TFile> where TCarClass : unmanaged, Enum where TFile : ICarPerf, ICarClass<TCarClass>, new()
{
    private static int TryInt(string value)
    {
        return int.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var s) ? s : 0;
    }

    private static double TryDouble(string value)
    {
        return double.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var s) ? s : 0.0;
    }

    private static IEnumerable<T> TryArray<T>(Dictionary<int, string> dic, int key, Func<string, T> parse)
    {
        return dic[key].Split(",").Where(p => !p.IsEmpty()).Select(parse);
    }

    /// <summary>
    /// Tries to read an <see cref="int"/> value from the Carp file parsed
    /// lines.
    /// </summary>
    /// <param name="dic">
    /// Dictionary containing the parsed Carp file lines.
    /// </param>
    /// <param name="key">Key of the value to get.</param>
    /// <returns>
    /// The value obtained from the dictinoary of parsed Carp lines, or
    /// <c><see langword="default"/></c> if either the value did not exist or
    /// if it was unable to be parsed as an <see cref="int"/> value.
    /// </returns>
    protected static int TryIntKey(Dictionary<int, string> dic, int key)
    {
        return TryInt(dic[key]);
    }

    /// <summary>
    /// Tries to read an <see cref="double"/> value from the Carp file parsed
    /// lines.
    /// </summary>
    /// <param name="dic">
    /// Dictionary containing the parsed Carp file lines.
    /// </param>
    /// <param name="key">Key of the value to get.</param>
    /// <returns>
    /// The value obtained from the dictinoary of parsed Carp lines, or
    /// <c><see langword="default"/></c> if either the value did not exist or
    /// if it was unable to be parsed as an <see cref="double"/> value.
    /// </returns>
    protected static double TryDoubleKey(Dictionary<int, string> dic, int key)
    {
        return TryDouble(dic[key]);
    }

    /// <summary>
    /// Tries to read a collection of <see cref="int"/> values from the Carp
    /// file parsed lines.
    /// </summary>
    /// <param name="dic">
    /// Dictionary containing the parsed Carp file lines.
    /// </param>
    /// <param name="key">Key of the value to get.</param>
    /// <returns>
    /// The array of values obtained from the dictinoary of parsed Carp lines,
    /// or <c><see langword="default"/></c> if either the value did not exist
    /// or if it was unable to be parsed as an <see cref="int"/> array.
    /// </returns>
    protected static IEnumerable<int> TryIntArray(Dictionary<int, string> dic, int key)
    {
        return TryArray(dic, key, TryInt);
    }

    /// <summary>
    /// Tries to read a collection of <see cref="double"/> values from the Carp
    /// file parsed lines.
    /// </summary>
    /// <param name="dic">
    /// Dictionary containing the parsed Carp file lines.
    /// </param>
    /// <param name="key">Key of the value to get.</param>
    /// <returns>
    /// The array of values obtained from the dictinoary of parsed Carp lines,
    /// or <c><see langword="default"/></c> if either the value did not exist
    /// or if it was unable to be parsed as an <see cref="double"/> array.
    /// </returns>
    protected static IEnumerable<double> TryDoubleArray(Dictionary<int, string> dic, int key)
    {
        return TryArray(dic, key, TryDouble);
    }

    private static int GetKey(string line)
    {
        return int.TryParse(line.Split('(')[^1].ChopEnd(")"), out var k) ? k : -1;
    }

    /// <summary>
    /// When overriden in a derived class, allows for deserialization of
    /// additional properties to be performed.
    /// </summary>
    /// <param name="carp">Object being deserialized.</param>
    /// <param name="fields">
    /// Dictionary of values that were read and parsed from the Carp file.
    /// </param>
    protected virtual void ReadProps(TFile carp, Dictionary<int, string> fields) { }

    /// <inheritdoc/>
    public TFile Deserialize(Stream stream)
    {
        Dictionary<int, string> dic = [];
        using (var r = new StreamReader(stream))
        {
            while (!r.EndOfStream)
            {
                var key = GetKey(r.ReadLine() ?? string.Empty);
                var value = r.ReadLine() ?? string.Empty;
                dic[key] = value;
            }
        }
        var carp = new TFile
        {
            SerialNumber = (ushort)TryIntKey(dic, 0),
            CarClass = Enum.Parse<TCarClass>(TryIntKey(dic, 1).ToString()),
            Mass = TryDoubleKey(dic, 2),
            NumberOfGearsManual = TryIntKey(dic, 3),
            NumberOfGearsAuto = TryIntKey(dic, 75),
            GearShiftDelay = TryIntKey(dic, 4),
            FinalGearManual = TryDoubleKey(dic, 11),
            FinalGearAuto = TryDoubleKey(dic, 79),
            EngineMinRpm = TryIntKey(dic, 12),
            EngineMaxRpm = TryIntKey(dic, 13),
            MaxVelocity = TryDoubleKey(dic, 14),
            TopSpeed = TryDoubleKey(dic, 15),
            FrontDriveRatio = TryDoubleKey(dic, 16),
            Abs = TryIntKey(dic, 17) != 0,
            MaxBrakeDecel = TryDoubleKey(dic, 18),
            FrontBrakeBias = TryDoubleKey(dic, 19),
            WheelBase = TryDoubleKey(dic, 24),
            FrontGripBias = TryDoubleKey(dic, 25),
            PowerSteering = TryIntKey(dic, 26) != 0,
            MinimumSteerAccel = TryDoubleKey(dic, 27),
            TurnInRamp = TryDoubleKey(dic, 28),
            TurnOutRamp = TryDoubleKey(dic, 29),
            LateralAccGripMult = TryDoubleKey(dic, 30),
            AeroDownMult = TryDoubleKey(dic, 31),
            GasOffFactor = TryDoubleKey(dic, 32),
            GTransferFactor = TryDoubleKey(dic, 33),
            TurnCircleRadius = TryDoubleKey(dic, 34),
            TireWear = TryDoubleKey(dic, 37),
            SlideMult = TryDoubleKey(dic, 38),
            SpinVelocityCap = TryDoubleKey(dic, 39),
            SlideVelocityCap = TryDoubleKey(dic, 40),
            SlideAssistanceFactor = TryDoubleKey(dic, 41),
            PushFactor = TryDoubleKey(dic, 42),
            LowTurnFactor = TryDoubleKey(dic, 43),
            HighTurnFactor = TryDoubleKey(dic, 44),
            PitchRollFactor = TryDoubleKey(dic, 45),
            RoadBumpFactor = TryDoubleKey(dic, 46),
            SpoilerFunctionType = TryIntKey(dic, 47),
            SpoilerActivationSpeed = TryDoubleKey(dic, 48),
            GradualTurnCutoff = TryDoubleKey(dic, 49),
            MediumTurnCutoff = TryDoubleKey(dic, 50),
            SharpTurnCutoff = TryDoubleKey(dic, 51),
            MediumTurnSpdMod = TryDoubleKey(dic, 52),
            SharpTurnSpdMod = TryDoubleKey(dic, 53),
            ExtremeTurnSpdMod = TryDoubleKey(dic, 54),
            SubdivideLevel = TryDoubleKey(dic, 55),
            CameraArm = TryDoubleKey(dic, 56),
            BodyDamage = TryDoubleKey(dic, 57),
            EngineDamage = TryDoubleKey(dic, 58),
            SuspensionDamage = TryDoubleKey(dic, 59),
            EngineTuning = TryDoubleKey(dic, 60),
            BrakeBalance = TryDoubleKey(dic, 61),
            SteeringSpeed = TryDoubleKey(dic, 62),
            GearRatFactor = TryDoubleKey(dic, 63),
            SuspensionStiffness = TryDoubleKey(dic, 64),
            AeroFactor = TryDoubleKey(dic, 65),
            TireFactor = TryDoubleKey(dic, 66),
        };

        carp.ShiftBlip.AddRange(TryDoubleArray(dic, 5));
        carp.BrakeBlip.AddRange(TryDoubleArray(dic, 6));
        carp.VelocityToRpmManual.AddRange(TryDoubleArray(dic, 7));
        carp.VelocityToRpmAuto.AddRange(TryDoubleArray(dic, 76));
        carp.GearRatioManual.AddRange(TryDoubleArray(dic, 8));
        carp.GearRatioAuto.AddRange(TryDoubleArray(dic, 77));
        carp.GearEfficiencyManual.AddRange(TryDoubleArray(dic, 9));
        carp.GearEfficiencyAuto.AddRange(TryDoubleArray(dic, 78));
        carp.TorqueCurve.AddRange(TryDoubleArray(dic, 10));
        carp.GasIncreaseCurve.AddRange(TryDoubleArray(dic, 20));
        carp.GasDecreaseCurve.AddRange(TryDoubleArray(dic, 21));
        carp.BrakeIncreaseCurve.AddRange(TryDoubleArray(dic, 22));
        carp.BrakeDecreaseCurve.AddRange(TryDoubleArray(dic, 23));
        carp.AiCurve0.AddRange(TryDoubleArray(dic, 67));
        carp.AiCurve1.AddRange(TryDoubleArray(dic, 68));
        carp.AiCurve2.AddRange(TryDoubleArray(dic, 69));
        carp.AiCurve3.AddRange(TryDoubleArray(dic, 70));
        carp.AiCurve4.AddRange(TryDoubleArray(dic, 71));
        carp.AiCurve5.AddRange(TryDoubleArray(dic, 72));
        carp.AiCurve6.AddRange(TryDoubleArray(dic, 73));
        carp.AiCurve7.AddRange(TryDoubleArray(dic, 74));
        var tirespecs = TryIntArray(dic, 35).ToArray();
        if (tirespecs.Length == 3)
        {
            carp.TireWidthFront = tirespecs[0];
            carp.TireSidewallFront = tirespecs[1];
            carp.TireRimFront = tirespecs[2];
        }
        tirespecs = TryIntArray(dic, 36).ToArray();
        if (tirespecs.Length == 3)
        {
            carp.TireWidthRear = tirespecs[0];
            carp.TireSidewallRear = tirespecs[1];
            carp.TireRimRear = tirespecs[2];
        }
        ReadProps(carp, dic);
        return carp;
    }

    /// <inheritdoc/>
    public virtual void SerializeTo(TFile entity, Stream stream)
    {
        stream.WriteBytes(Encoding.Latin1.GetBytes($"""
        Serial Number(0)
        {entity.SerialNumber}
        Car Classification(1)
        {Convert.ToInt32(entity.CarClass)}
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
        {GetExtraProps(entity)}
        """));
    }

    /// <summary>
    /// When overriden in a derived class, allows the serializer to write any
    /// additional properties defined in the Carp model.
    /// </summary>
    /// <param name="entity">Entity being serialized.</param>
    /// <returns>
    /// A string with the additional contents of the serialized Carp file.
    /// </returns>
    protected virtual string GetExtraProps(TFile entity) => string.Empty;
}
