#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Carp;

namespace TheXDS.Vivianne.Serializers.Base;

public abstract class CarpSerializerTestsBase<TSerializer, TFile>(string testResourceName, Action<TFile> additionalPropsSetter)
    : SerializerTestsBase<TSerializer, TFile>(testResourceName, GetDefaultFile(additionalPropsSetter))
    where TSerializer : ISerializer<TFile>, new() where TFile : ICarPerf, new()
{
    private static TFile GetDefaultFile(Action<TFile> additionalPropsSetter)
    {
        var file = new TFile()
        {
            SerialNumber = 1,
            Mass = 1350.0,
            NumberOfGearsManual = 8,
            NumberOfGearsAuto = 8,
            GearShiftDelay = 6,
            ShiftBlip = { 0, 0, 125, 115, 90, 80, 75, 55 },
            BrakeBlip = { 0, 0, 115, 100, 85, 75, 60, 50 },
            VelocityToRpmManual = { -273.905090, 0.000000, 360.599213, 233.674225, 176.509521, 131.028549, 111.536697, 92.044846 },
            VelocityToRpmAuto = { -273.905090, 0.000000, 360.599213, 233.674225, 176.509521, 141.857346, 115.868217, 92.044846 },
            GearRatioManual = { 2.529412, 0.000000, 3.330000, 2.157895, 1.630000, 1.310000, 1.070000, 0.850000 },
            GearRatioAuto = { 2.529412, 0.000000, 2.933333, 2.157895, 1.630000, 1.310000, 1.070000, 0.850000 },
            GearEfficiencyManual = { 0.800000, 0.000000, 0.770000, 0.790000, 0.820000, 0.840000, 0.850000, 1.150000 },
            GearEfficiencyAuto = { 0.800000, 0.000000, 0.710000, 0.710000, 0.740000, 0.740000, 0.800000, 1.150000 },
            TorqueCurve = {
            000.000000, 200.000000, 240.000000, 275.000000, 295.000000, 310.000000, 330.000000,
            350.000000, 380.000000, 420.000000, 440.000000, 450.000000, 465.000000, 471.000000,
            465.000000, 470.000000, 465.000000, 430.000000, 400.000000, 360.000000, 000.000000 },
            FinalGearManual = 3.800000,
            FinalGearAuto = 3.800000,
            EngineMinRpm = 1335,
            EngineMaxRpm = 8900,
            MaxVelocity = 90.303543,
            TopSpeed = 90.303543,
            FrontDriveRatio = 0.000000,
            Abs = true,
            MaxBrakeDecel = 10.850000,
            FrontBrakeBias = 0.529400,
            GasIncreaseCurve = { 16, 16, 16, 16, 16, 16, 16, 16 },
            GasDecreaseCurve = { 32, 32, 32, 32, 32, 32, 32, 32 },
            BrakeIncreaseCurve = { 64.000000, 32.000000, 16.000000, 8.000000, 4.000000, 2.000000, 1.000000, 1.000000 },
            BrakeDecreaseCurve = { 32.000000, 32.000000, 32.000000, 32.000000, 32.000000, 32.000000, 32.000000, 32.000000 },
            WheelBase = 2.580000,
            FrontGripBias = 0.420000,
            PowerSteering = true,
            MinimumSteerAccel = 18.250000,
            TurnInRamp = 16.000000,
            TurnOutRamp = 32.000000,
            LateralAccGripMult = 3.610000,
            AeroDownMult = 0.002368,
            GasOffFactor = 0.465000,
            GTransferFactor = 0.462000,
            TurnCircleRadius = 6.300000,
            TireWidthFront = 245,
            TireSidewallFront = 35,
            TireRimFront = 18,
            TireWidthRear = 335,
            TireSidewallRear = 35,
            TireRimRear = 18,
            TireWear = 0.000000,
            SlideMult = 1.099991,
            SpinVelocityCap = 0.299988,
            SlideVelocityCap = 0.299988,
            SlideAssistanceFactor = 150,
            PushFactor = 12000,
            LowTurnFactor = 0.023987,
            HighTurnFactor = 0.028000,
            PitchRollFactor = 0.920000,
            RoadBumpFactor = 0.910000,
            SpoilerFunctionType = 8,
            SpoilerActivationSpeed = 0.000000,
            GradualTurnCutoff = 110,
            MediumTurnCutoff = 160,
            SharpTurnCutoff = 180,
            MediumTurnSpdMod = 0.970000,
            SharpTurnSpdMod = 0.950000,
            ExtremeTurnSpdMod = 0.930000,
            SubdivideLevel = 3,
            CameraArm = 0.250000,
            BodyDamage = 0.000000,
            EngineDamage = 0.000000,
            SuspensionDamage = 0.000000,
            EngineTuning = 1.000000,
            BrakeBalance = 0.000000,
            SteeringSpeed = 1.000000,
            GearRatFactor = 1.000000,
            SuspensionStiffness = 1.114755,
            AeroFactor = 1.000000,
            TireFactor = 1.000000,
            AiCurve0 = { 4.266667, 6.349520, 7.737358, 8.536504, 8.955383, 9.161016, 9.224277, 9.157882, 9.052475, 9.028872, 9.089978, 9.139469, 9.096681, 9.015980 },
            AiCurve1 = { 9.009326, 9.113148, 9.310430, 9.504269, 9.512443, 9.131333, 8.316323, 7.362495, 6.644360, 6.281654, 6.185064, 6.216635, 6.278254, 6.316365 },
            AiCurve2 = { 6.292465, 6.171501, 5.896525, 5.472954, 5.072002, 4.847321, 4.760395, 4.711891, 4.667427, 4.630373, 4.604499, 4.588714, 4.548230, 4.393620 },
            AiCurve3 = { 4.094394, 3.763382, 3.514253, 3.351206, 3.247369, 3.205148, 3.212960, 3.218600, 3.184813, 3.129700, 3.083073, 3.045386, 3.004708, 2.939694 },
            AiCurve4 = { 2.808251, 2.604247, 2.396461, 2.251089, 2.168125, 2.114827, 2.067707, 2.019555, 1.972794, 1.922291, 1.852626, 1.778252, 1.746975, 1.770979 },
            AiCurve5 = { 1.809858, 1.827267, 1.815285, 1.777023, 1.717956, 1.646383, 1.569493, 1.491768, 1.415381, 1.339153, 1.260314, 1.177396, 1.090600, 1.000228 },
            AiCurve6 = { 0.905302, 0.803205, 0.690751, 0.566915, 0.434530, 0.299065, 0.140351, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000 },
            AiCurve7 = { 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000 }
        };
        additionalPropsSetter.Invoke(file);
        return file;
    }

    protected override void TestParsedFile(TFile expected, TFile actual)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.SerialNumber, Is.EqualTo(expected.SerialNumber));
            Assert.That(actual.Mass, Is.EqualTo(expected.Mass));
            Assert.That(actual.NumberOfGearsManual, Is.EqualTo(expected.NumberOfGearsManual));
            Assert.That(actual.NumberOfGearsAuto, Is.EqualTo(expected.NumberOfGearsAuto));
            Assert.That(actual.GearShiftDelay, Is.EqualTo(expected.GearShiftDelay));
            Assert.That(actual.ShiftBlip, Is.EquivalentTo(expected.ShiftBlip));
            Assert.That(actual.BrakeBlip, Is.EquivalentTo(expected.BrakeBlip));
            Assert.That(actual.VelocityToRpmManual, Is.EquivalentTo(expected.VelocityToRpmManual));
            Assert.That(actual.VelocityToRpmAuto, Is.EquivalentTo(expected.VelocityToRpmAuto));
            Assert.That(actual.GearRatioManual, Is.EquivalentTo(expected.GearRatioManual));
            Assert.That(actual.GearRatioAuto, Is.EquivalentTo(expected.GearRatioAuto));
            Assert.That(actual.GearEfficiencyManual, Is.EquivalentTo(expected.GearEfficiencyManual));
            Assert.That(actual.GearEfficiencyAuto, Is.EquivalentTo(expected.GearEfficiencyAuto));
            Assert.That(actual.TorqueCurve, Is.EquivalentTo(expected.TorqueCurve));
            Assert.That(actual.FinalGearManual, Is.EqualTo(expected.FinalGearManual));
            Assert.That(actual.FinalGearAuto, Is.EqualTo(expected.FinalGearAuto));
            Assert.That(actual.EngineMinRpm, Is.EqualTo(expected.EngineMinRpm));
            Assert.That(actual.EngineMaxRpm, Is.EqualTo(expected.EngineMaxRpm));
            Assert.That(actual.MaxVelocity, Is.EqualTo(expected.MaxVelocity));
            Assert.That(actual.TopSpeed, Is.EqualTo(expected.TopSpeed));
            Assert.That(actual.FrontDriveRatio, Is.EqualTo(expected.FrontDriveRatio));
            Assert.That(actual.Abs, Is.EqualTo(expected.Abs));
            Assert.That(actual.MaxBrakeDecel, Is.EqualTo(expected.MaxBrakeDecel));
            Assert.That(actual.FrontBrakeBias, Is.EqualTo(expected.FrontBrakeBias));
            Assert.That(actual.GasIncreaseCurve, Is.EquivalentTo(expected.GasIncreaseCurve));
            Assert.That(actual.GasDecreaseCurve, Is.EquivalentTo(expected.GasDecreaseCurve));
            Assert.That(actual.BrakeIncreaseCurve, Is.EquivalentTo(expected.BrakeIncreaseCurve));
            Assert.That(actual.BrakeDecreaseCurve, Is.EquivalentTo(expected.BrakeDecreaseCurve));
            Assert.That(actual.WheelBase, Is.EqualTo(expected.WheelBase));
            Assert.That(actual.FrontGripBias, Is.EqualTo(expected.FrontGripBias));
            Assert.That(actual.PowerSteering, Is.EqualTo(expected.PowerSteering));
            Assert.That(actual.MinimumSteerAccel, Is.EqualTo(expected.MinimumSteerAccel));
            Assert.That(actual.TurnInRamp, Is.EqualTo(expected.TurnInRamp));
            Assert.That(actual.TurnOutRamp, Is.EqualTo(expected.TurnOutRamp));
            Assert.That(actual.LateralAccGripMult, Is.EqualTo(expected.LateralAccGripMult));
            Assert.That(actual.AeroDownMult, Is.EqualTo(expected.AeroDownMult));
            Assert.That(actual.GasOffFactor, Is.EqualTo(expected.GasOffFactor));
            Assert.That(actual.GTransferFactor, Is.EqualTo(expected.GTransferFactor));
            Assert.That(actual.TurnCircleRadius, Is.EqualTo(expected.TurnCircleRadius));
            Assert.That(actual.TireWidthFront, Is.EqualTo(expected.TireWidthFront));
            Assert.That(actual.TireSidewallFront, Is.EqualTo(expected.TireSidewallFront));
            Assert.That(actual.TireRimFront, Is.EqualTo(expected.TireRimFront));
            Assert.That(actual.TireWidthRear, Is.EqualTo(expected.TireWidthRear));
            Assert.That(actual.TireSidewallRear, Is.EqualTo(expected.TireSidewallRear));
            Assert.That(actual.TireRimRear, Is.EqualTo(expected.TireRimRear));
            Assert.That(actual.TireWear, Is.EqualTo(expected.TireWear));
            Assert.That(actual.SlideMult, Is.EqualTo(expected.SlideMult));
            Assert.That(actual.SpinVelocityCap, Is.EqualTo(expected.SpinVelocityCap));
            Assert.That(actual.SlideVelocityCap, Is.EqualTo(expected.SlideVelocityCap));
            Assert.That(actual.SlideAssistanceFactor, Is.EqualTo(expected.SlideAssistanceFactor));
            Assert.That(actual.PushFactor, Is.EqualTo(expected.PushFactor));
            Assert.That(actual.LowTurnFactor, Is.EqualTo(expected.LowTurnFactor));
            Assert.That(actual.HighTurnFactor, Is.EqualTo(expected.HighTurnFactor));
            Assert.That(actual.PitchRollFactor, Is.EqualTo(expected.PitchRollFactor));
            Assert.That(actual.RoadBumpFactor, Is.EqualTo(expected.RoadBumpFactor));
            Assert.That(actual.SpoilerFunctionType, Is.EqualTo(expected.SpoilerFunctionType));
            Assert.That(actual.SpoilerActivationSpeed, Is.EqualTo(expected.SpoilerActivationSpeed));
            Assert.That(actual.GradualTurnCutoff, Is.EqualTo(expected.GradualTurnCutoff));
            Assert.That(actual.MediumTurnCutoff, Is.EqualTo(expected.MediumTurnCutoff));
            Assert.That(actual.SharpTurnCutoff, Is.EqualTo(expected.SharpTurnCutoff));
            Assert.That(actual.MediumTurnSpdMod, Is.EqualTo(expected.MediumTurnSpdMod));
            Assert.That(actual.SharpTurnSpdMod, Is.EqualTo(expected.SharpTurnSpdMod));
            Assert.That(actual.ExtremeTurnSpdMod, Is.EqualTo(expected.ExtremeTurnSpdMod));
            Assert.That(actual.SubdivideLevel, Is.EqualTo(expected.SubdivideLevel));
            Assert.That(actual.CameraArm, Is.EqualTo(expected.CameraArm));
            Assert.That(actual.BodyDamage, Is.EqualTo(expected.BodyDamage));
            Assert.That(actual.EngineDamage, Is.EqualTo(expected.EngineDamage));
            Assert.That(actual.SuspensionDamage, Is.EqualTo(expected.SuspensionDamage));
            Assert.That(actual.EngineTuning, Is.EqualTo(expected.EngineTuning));
            Assert.That(actual.BrakeBalance, Is.EqualTo(expected.BrakeBalance));
            Assert.That(actual.SteeringSpeed, Is.EqualTo(expected.SteeringSpeed));
            Assert.That(actual.GearRatFactor, Is.EqualTo(expected.GearRatFactor));
            Assert.That(actual.SuspensionStiffness, Is.EqualTo(expected.SuspensionStiffness));
            Assert.That(actual.AeroFactor, Is.EqualTo(expected.AeroFactor));
            Assert.That(actual.TireFactor, Is.EqualTo(expected.TireFactor));
            Assert.That(actual.AiCurve0, Is.EquivalentTo(expected.AiCurve0));
            Assert.That(actual.AiCurve1, Is.EquivalentTo(expected.AiCurve1));
            Assert.That(actual.AiCurve2, Is.EquivalentTo(expected.AiCurve2));
            Assert.That(actual.AiCurve3, Is.EquivalentTo(expected.AiCurve3));
            Assert.That(actual.AiCurve4, Is.EquivalentTo(expected.AiCurve4));
            Assert.That(actual.AiCurve5, Is.EquivalentTo(expected.AiCurve5));
            Assert.That(actual.AiCurve6, Is.EquivalentTo(expected.AiCurve6));
            Assert.That(actual.AiCurve7, Is.EquivalentTo(expected.AiCurve7));
        });
    }
}
