using System.Globalization;
using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Carp.Nfs2;

namespace TheXDS.Vivianne.Serializers.Carp.Nfs2;

/// <summary>
/// Implements a Carp serializer for NFS2 cars.
/// </summary>
public class CarpSerializer : IMarshalSerializer<CarPerf, CarpSerializer.CarpData>
{
    CarPerf IMarshalSerializer<CarPerf, CarpData>.Convert(CarpData entity)
    {
        return new()
        {
            Mass = entity.Mass,
            NumberOfGears = entity.NumberOfGears,
            GearShiftDelay = entity.GearShiftDelay,
            GearEfficiency = [.. entity.GearEfficiency],
            VelocityToRpm = [.. entity.VelocityToRpm],
            TorqueCurve = [.. entity.TorqueCurve],
            EngineMaxRpm = entity.EngineMaxRpm,
            MaxVelocity = entity.MaxVelocity,
            FrontDriveRatio = entity.FrontDriveRatio,
            MaxBrakeDecel = entity.MaxBrakeDecel,
            FrontBrakeBias = entity.FrontBrakeBias,
            GasIncreaseCurve = [.. entity.GasIncreaseCurve],
            GasDecreaseCurve = [.. entity.GasDecreaseCurve],
            BrakeIncreaseCurve = [.. entity.BrakeIncreaseCurve],
            BrakeDecreaseCurve = [.. entity.BrakeDecreaseCurve],
            WheelBase = entity.WheelBase,
            FrontGripBias = entity.FrontGripBias,
            MaximumSteerAccel = entity.MaximumSteerAccel,
            TurnInRamp = entity.TurnInRamp,
            TurnOutRamp = entity.TurnOutRamp,
            LateralAccGripMult = entity.LateralAccGripMult,
            AeroDownMult = entity.AeroDownMult,
            GasOffFactor = entity.GasOffFactor,
            GTransferFactor = entity.GTransferFactor,
            SlideMult = entity.SlideMult,
            SpinVelocityCap = entity.SpinVelocityCap,
            SlideVelocityCap = entity.SlideVelocityCap,
            SlideAssistanceFactor = entity.SlideAssistanceFactor,
            PushFactor = entity.PushFactor,
            LowTurnFactor = entity.LowTurnFactor,
            HighTurnFactor = entity.HighTurnFactor            
        };
    }

    CarpData IMarshalSerializer<CarPerf, CarpData>.Convert(CarPerf entity)
    {
        return new()
        {
            Mass = entity.Mass,
            NumberOfGears = entity.NumberOfGears,
            GearShiftDelay = entity.GearShiftDelay,
            GearEfficiency = [.. entity.GearEfficiency],
            VelocityToRpm = [.. entity.VelocityToRpm],
            TorqueCurve = [.. entity.TorqueCurve],
            EngineMaxRpm = entity.EngineMaxRpm,
            MaxVelocity = entity.MaxVelocity,
            FrontDriveRatio = entity.FrontDriveRatio,
            MaxBrakeDecel = entity.MaxBrakeDecel,
            FrontBrakeBias = entity.FrontBrakeBias,
            GasIncreaseCurve = [.. entity.GasIncreaseCurve],
            GasDecreaseCurve = [.. entity.GasDecreaseCurve],
            BrakeIncreaseCurve = [.. entity.BrakeIncreaseCurve],
            BrakeDecreaseCurve = [.. entity.BrakeDecreaseCurve],
            WheelBase = entity.WheelBase,
            FrontGripBias = entity.FrontGripBias,
            MaximumSteerAccel = entity.MaximumSteerAccel,
            TurnInRamp = entity.TurnInRamp,
            TurnOutRamp = entity.TurnOutRamp,
            LateralAccGripMult = entity.LateralAccGripMult,
            AeroDownMult = entity.AeroDownMult,
            GasOffFactor = entity.GasOffFactor,
            GTransferFactor = entity.GTransferFactor,
            SlideMult = entity.SlideMult,
            SpinVelocityCap = entity.SpinVelocityCap,
            SlideVelocityCap = entity.SlideVelocityCap,
            SlideAssistanceFactor = entity.SlideAssistanceFactor,
            PushFactor = entity.PushFactor,
            LowTurnFactor = entity.LowTurnFactor,
            HighTurnFactor = entity.HighTurnFactor
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    private struct FixedPointDecimal32
    {
        public ushort Fraction;
        public short Integer;

        public static implicit operator float(FixedPointDecimal32 x)
        {
            return x.Integer + float.Parse($"0.{x.Fraction}", CultureInfo.InvariantCulture);
        }

        public static implicit operator double(FixedPointDecimal32 x)
        {
            return x.Integer + double.Parse($"0.{x.Fraction}", CultureInfo.InvariantCulture);
        }

        public static implicit operator FixedPointDecimal32(float x)
        {
            return (double)x;
        }

        public static implicit operator FixedPointDecimal32(double x)
        {
            return new FixedPointDecimal32()
            {
                Integer = (short)Math.Floor(x),
                Fraction = ushort.Parse((x - Math.Floor(x)).ToString(CultureInfo.InvariantCulture).ChopStart("0."), CultureInfo.InvariantCulture)
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    private struct CarpData
    {
        public FixedPointDecimal32 Mass;
        public int NumberOfGears;
        public int GearShiftDelay;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public FixedPointDecimal32[] VelocityToRpm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public FixedPointDecimal32[] GearEfficiency;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public FixedPointDecimal32[] TorqueCurve;
        public int EngineMaxRpm;
        public FixedPointDecimal32 MaxVelocity;
        public FixedPointDecimal32 FrontDriveRatio;
        public FixedPointDecimal32 MaxBrakeDecel;
        public FixedPointDecimal32 FrontBrakeBias;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] GasIncreaseCurve;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] GasDecreaseCurve;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] BrakeIncreaseCurve;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] BrakeDecreaseCurve;
        public FixedPointDecimal32 WheelBase;
        public FixedPointDecimal32 FrontGripBias;
        public FixedPointDecimal32 MaximumSteerAccel;
        public FixedPointDecimal32 TurnInRamp;
        public FixedPointDecimal32 TurnOutRamp;
        public FixedPointDecimal32 LateralAccGripMult;
        public FixedPointDecimal32 AeroDownMult;
        public FixedPointDecimal32 GasOffFactor;
        public FixedPointDecimal32 GTransferFactor;
        public FixedPointDecimal32 SlideMult;
        public FixedPointDecimal32 SpinVelocityCap;
        public FixedPointDecimal32 SlideVelocityCap;
        public FixedPointDecimal32 SlideAssistanceFactor;
        public FixedPointDecimal32 PushFactor;
        public FixedPointDecimal32 LowTurnFactor;
        public FixedPointDecimal32 HighTurnFactor;
    }
}
