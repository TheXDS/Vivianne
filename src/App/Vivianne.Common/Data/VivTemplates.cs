using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Fsh.Nfs3;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.Serializers.Fsh.Blobs;

namespace TheXDS.Vivianne.Data;

internal static class VivTemplates
{
    public static IEnumerable<KeyValuePair<string, (string, Func<byte[]>)[]>> Get()
    {
        yield return new("DASH.qfs for NFS3", [("DASH.qfs", EmptyDashQfs)]);
        yield return new("Carp.txt (NFS3, based on NFS4 Ferrari F50)", [("carp.txt", EmptyCarp3)]);
        yield return new("Carp.txt (NFS4, based on NFS4 Ferrari F50)", [("carp.txt", EmptyCarp4)]);
        yield return new("FeData (NFS3, Full set)", FullFeDataSet(EmptyFeData3));
        yield return new("FeData (NFS4, Full set)", FullFeDataSet(EmptyFeData4));
    }

    private static (string, Func<byte[]>)[] FullFeDataSet(Func<byte[]> factory)
    {
        return [.. Models.Fe.FeDataBase.KnownExtensions.Select<string, (string, Func<byte[]>)>(p => ($"fedata{p}", factory))];
    }

    private static byte[] EmptyFeData3()
    {
        return ((ISerializer<Models.Fe.Nfs3.FeData>)new Serializers.Fe.Nfs3.FeDataSerializer()).Serialize(new()
        {
            VehicleClass = Models.Fe.Nfs3.CarClass.A,
            Seat = Models.Fe.DriverSeatPosition.Left,
            IsPolice = false,
            IsDlcCar = 1,
            AvailableToAi = true,
            CarAccel = 10,
            CarTopSpeed = 10,
            CarHandling = 10,
            CarBraking = 10,
            CarId = "XXXX",
            SerialNumber = (ushort)new Random().Next(ushort.MaxValue),
            IsBonus = false,
            StringEntries = 40,
        });
    }

    private static byte[] EmptyFeData4()
    {
        return ((ISerializer<Models.Fe.Nfs4.FeData>)new Serializers.Fe.Nfs4.FeDataSerializer()).Serialize(new()
        {
            PoliceFlag = Models.Fe.Nfs4.PursuitFlag.No,
            VehicleClass = Models.Fe.Nfs4.CarClass.AAA,
            Upgradable = true,
            Convertible = false,
            DefaultCompare = new()
            {
                Acceleration = 10,
                TopSpeed = 10,
                Handling = 10,
                Braking = 10,
                Overall = 10,
                Price = 30000
            },
            CompareUpg1 = new()
            {
                Acceleration = 12,
                TopSpeed = 12,
                Handling = 12,
                Braking = 12,
                Overall = 12,
                Price = 10000
            },
            CompareUpg2 = new()
            {
                Acceleration = 14,
                TopSpeed = 14,
                Handling = 14,
                Braking = 14,
                Overall = 14,
                Price = 20000
            },
            CompareUpg3 = new()
            {
                Acceleration = 16,
                TopSpeed = 16,
                Handling = 16,
                Braking = 16,
                Overall = 16,
                Price = 30000
            },
            CarId = "XXXX",
            SerialNumber = (ushort)new Random().Next(ushort.MaxValue),
            IsBonus = false,
            StringEntries = 41,
        });
    }

    private static byte[] EmptyDashQfs()
    {
        var cabinBlob = new FshBlob()
        {
            Magic = FshBlobFormat.Argb32,
            Footer = ((ISerializer<GaugeData>)new GaugeDataSerializer()).Serialize(new())
        };
        cabinBlob.ReplaceWith(new Image<Rgba32>(640, 480));
        var steerBlob = new FshBlob() { Magic = FshBlobFormat.Argb32, XRotation = 128, YRotation = 128, XPosition = 192, YPosition = 352 };
        steerBlob.ReplaceWith(new Image<Rgba32>(256, 256));

        var fsh = new FshFile();
        fsh.Entries.Add("0000", cabinBlob);
        fsh.Entries.Add("0001", steerBlob);
        return QfsCodec.Compress(((ISerializer<FshFile>)new FshSerializer()).Serialize(fsh));
    }

    private static byte[] EmptyCarp3()
    {
        return ((ISerializer<Models.Carp.Nfs3.CarPerf>)new Serializers.Carp.Nfs3.CarpSerializer()).Serialize(new()
        {
            SerialNumber = (ushort)new Random().Next(ushort.MaxValue),
            CarClass = Models.Fe.Nfs3.CarClass.A,
            Mass = 1350,
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
        });
    }

    private static byte[] EmptyCarp4()
    {
        return ((ISerializer<Models.Carp.Nfs4.CarPerf>)new Serializers.Carp.Nfs4.CarpSerializer()).Serialize(new()
        {
            SerialNumber = (ushort)new Random().Next(ushort.MaxValue),
            CarClass = Models.Fe.Nfs4.CarClass.AAA,
            Mass = 1350,
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
        });
    }
}
