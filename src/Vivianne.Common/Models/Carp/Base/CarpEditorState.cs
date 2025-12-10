using System;
using System.Collections.Generic;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models.Carp.Base;

/// <summary>
/// Implements a state for a Carp file.
/// </summary>
public class CarpEditorState<TCarPerf, TCarClass> : FileStateBase<TCarPerf>, ICarPerf where TCarPerf : CarPerf<TCarClass> where TCarClass : unmanaged, Enum
{
    private ObservableListWrap<double>? _aiCurve0;
    private ObservableListWrap<double>? _aiCurve1;
    private ObservableListWrap<double>? _aiCurve2;
    private ObservableListWrap<double>? _aiCurve3;
    private ObservableListWrap<double>? _aiCurve4;
    private ObservableListWrap<double>? _aiCurve5;
    private ObservableListWrap<double>? _aiCurve6;
    private ObservableListWrap<double>? _aiCurve7;
    private ObservableListWrap<double>? _brakeBlip;
    private ObservableListWrap<double>? _brakeDecreaseCurve;
    private ObservableListWrap<double>? _brakeIncreaseCurve;
    private ObservableListWrap<double>? _gasDecreaseCurve;
    private ObservableListWrap<double>? _gasIncreaseCurve;
    private ObservableListWrap<double>? _gearEfficiencyAuto;
    private ObservableListWrap<double>? _gearEfficiencyManual;
    private ObservableListWrap<double>? _gearRatioAuto;
    private ObservableListWrap<double>? _gearRatioManual;
    private ObservableListWrap<double>? _shiftBlip;
    private ObservableListWrap<double>? _torqueCurve;
    private ObservableListWrap<double>? _velocityToRpmAuto;
    private ObservableListWrap<double>? _velocityToRpmManual;

    /// <inheritdoc/>
    public bool Abs
    {
        get => File.Abs;
        set => Change(p => p.Abs, value);
    }

    /// <inheritdoc/>
    public double AeroDownMult
    {
        get => File.AeroDownMult;
        set => Change(p => p.AeroDownMult, value);
    }

    /// <inheritdoc/>
    public double AeroFactor
    {
        get => File.AeroFactor;
        set => Change(p => p.AeroFactor, value);
    }

    /// <inheritdoc/>
    public IList<double> AiCurve0 => _aiCurve0 ??= GetObservable(File.AiCurve0);

    /// <inheritdoc/>
    public IList<double> AiCurve1 => _aiCurve1 ??= GetObservable(File.AiCurve1);

    /// <inheritdoc/>
    public IList<double> AiCurve2 => _aiCurve2 ??= GetObservable(File.AiCurve2);

    /// <inheritdoc/>
    public IList<double> AiCurve3 => _aiCurve3 ??= GetObservable(File.AiCurve3);

    /// <inheritdoc/>
    public IList<double> AiCurve4 => _aiCurve4 ??= GetObservable(File.AiCurve4);

    /// <inheritdoc/>
    public IList<double> AiCurve5 => _aiCurve5 ??= GetObservable(File.AiCurve5);

    /// <inheritdoc/>
    public IList<double> AiCurve6 => _aiCurve6 ??= GetObservable(File.AiCurve6);

    /// <inheritdoc/>
    public IList<double> AiCurve7 => _aiCurve7 ??= GetObservable(File.AiCurve7);

    /// <inheritdoc/>
    public double BodyDamage
    {
        get => File.BodyDamage;
        set => Change(p => p.BodyDamage, value);
    }

    /// <inheritdoc/>
    public double BrakeBalance
    {
        get => File.BrakeBalance;
        set => Change(p => p.BrakeBalance, value);
    }

    /// <inheritdoc/>
    public IList<double> BrakeBlip => _brakeBlip ??= GetObservable(File.BrakeBlip);

    /// <inheritdoc/>
    public IList<double> BrakeDecreaseCurve => _brakeDecreaseCurve ??= GetObservable(File.BrakeDecreaseCurve);

    /// <inheritdoc/>
    public IList<double> BrakeIncreaseCurve => _brakeIncreaseCurve ??= GetObservable(File.BrakeIncreaseCurve);

    /// <inheritdoc/>
    public double CameraArm
    {
        get => File.CameraArm;
        set => Change(p => p.CameraArm, value);
    }

    /// <summary>
    /// Gets or sets the car class to which the car belongs.
    /// </summary>
    public TCarClass CarClass
    {
        get => File.CarClass;
        set => Change(p => p.CarClass, value);
    }

    /// <inheritdoc/>
    public double EngineDamage
    {
        get => File.EngineDamage;
        set => Change(p => p.EngineDamage, value);
    }

    /// <inheritdoc/>
    public int EngineMaxRpm
    {
        get => File.EngineMaxRpm;
        set => Change(p => p.EngineMaxRpm, value);
    }
    
    /// <inheritdoc/>
    public int EngineMinRpm
    {
        get => File.EngineMinRpm;
        set => Change(p => p.EngineMinRpm, value);
    }
    
    /// <inheritdoc/>
    public double EngineTuning
    {
        get => File.EngineTuning;
        set => Change(p => p.EngineTuning, value);
    }

    /// <inheritdoc/>
    public double ExtremeTurnSpdMod
    {
        get => File.ExtremeTurnSpdMod;
        set => Change(p => p.ExtremeTurnSpdMod, value);
    }

    /// <inheritdoc/>
    public double FinalGearAuto
    {
        get => File.FinalGearAuto;
        set => Change(p => p.FinalGearAuto, value);
    }

    /// <inheritdoc/>
    public double FinalGearManual
    {
        get => File.FinalGearManual;
        set => Change(p => p.FinalGearManual, value);
    }

    /// <inheritdoc/>
    public double FrontBrakeBias
    {
        get => File.FrontBrakeBias;
        set => Change(p => p.FrontBrakeBias, value);
    }

    /// <inheritdoc/>
    public double FrontDriveRatio
    {
        get => File.FrontDriveRatio;
        set => Change(p => p.FrontDriveRatio, value);
    }

    /// <inheritdoc/>
    public double FrontGripBias
    {
        get => File.FrontGripBias;
        set => Change(p => p.FrontGripBias, value);
    }

    /// <inheritdoc/>
    public IList<double> GasDecreaseCurve => _gasDecreaseCurve ??= GetObservable(File.GasDecreaseCurve);

    /// <inheritdoc/>
    public IList<double> GasIncreaseCurve => _gasIncreaseCurve ??= GetObservable(File.GasIncreaseCurve);

    /// <inheritdoc/>
    public double GasOffFactor
    {
        get => File.GasOffFactor;
        set => Change(p => p.GasOffFactor, value);
    }

    /// <inheritdoc/>
    public IList<double> GearEfficiencyAuto => _gearEfficiencyAuto ??= GetObservable(File.GearEfficiencyAuto);

    /// <inheritdoc/>
    public IList<double> GearEfficiencyManual => _gearEfficiencyManual ??= GetObservable(File.GearEfficiencyManual);

    /// <inheritdoc/>
    public double GearRatFactor
    {
        get => File.GearRatFactor;
        set => Change(p => p.GearRatFactor, value);
    }

    /// <inheritdoc/>
    public IList<double> GearRatioAuto => _gearRatioAuto ??= GetObservable(File.GearRatioAuto);

    /// <inheritdoc/>
    public IList<double> GearRatioManual => _gearRatioManual ??= GetObservable(File.GearRatioManual);

    /// <inheritdoc/>
    public int GearShiftDelay
    {
        get => File.GearShiftDelay;
        set => Change(p => p.GearShiftDelay, value);
    }

    /// <inheritdoc/>
    public double GradualTurnCutoff
    {
        get => File.GradualTurnCutoff;
        set => Change(p => p.GradualTurnCutoff, value);
    }

    /// <inheritdoc/>
    public double GTransferFactor
    {
        get => File.GTransferFactor;
        set => Change(p => p.GTransferFactor, value);
    }

    /// <inheritdoc/>
    public double HighTurnFactor
    {
        get => File.HighTurnFactor;
        set => Change(p => p.HighTurnFactor, value);
    }

    /// <inheritdoc/>
    public double LateralAccGripMult
    {
        get => File.LateralAccGripMult;
        set => Change(p => p.LateralAccGripMult, value);
    }

    /// <inheritdoc/>
    public double LowTurnFactor
    {
        get => File.LowTurnFactor;
        set => Change(p => p.LowTurnFactor, value);
    }

    /// <inheritdoc/>
    public double Mass
    {
        get => File.Mass;
        set => Change(p => p.Mass, value);
    }

    /// <inheritdoc/>
    public double MaxBrakeDecel
    {
        get => File.MaxBrakeDecel;
        set => Change(p => p.MaxBrakeDecel, value);
    }

    /// <inheritdoc/>
    public double MaxVelocity
    {
        get => File.MaxVelocity;
        set => Change(p => p.MaxVelocity, value);
    }

    /// <inheritdoc/>
    public double MediumTurnCutoff
    {
        get => File.MediumTurnCutoff;
        set => Change(p => p.MediumTurnCutoff, value);
    }

    /// <inheritdoc/>
    public double MediumTurnSpdMod
    {
        get => File.MediumTurnSpdMod;
        set => Change(p => p.MediumTurnSpdMod, value);
    }

    /// <inheritdoc/>
    public double MinimumSteerAccel
    {
        get => File.MinimumSteerAccel;
        set => Change(p => p.MinimumSteerAccel, value);
    }

    /// <inheritdoc/>
    public int NumberOfGearsAuto
    {
        get => File.NumberOfGearsAuto;
        set => Change(p => p.NumberOfGearsAuto, value);
    }

    /// <inheritdoc/>
    public int NumberOfGearsManual
    {
        get => File.NumberOfGearsManual;
        set => Change(p => p.NumberOfGearsManual, value);
    }

    /// <inheritdoc/>
    public double PitchRollFactor
    {
        get => File.PitchRollFactor;
        set => Change(p => p.PitchRollFactor, value);
    }

    /// <inheritdoc/>
    public bool PowerSteering
    {
        get => File.PowerSteering;
        set => Change(p => p.PowerSteering, value);
    }

    /// <inheritdoc/>
    public double PushFactor
    {
        get => File.PushFactor;
        set => Change(p => p.PushFactor, value);
    }

    /// <inheritdoc/>
    public double RoadBumpFactor
    {
        get => File.RoadBumpFactor;
        set => Change(p => p.RoadBumpFactor, value);
    }

    /// <inheritdoc/>
    public double SharpTurnCutoff
    {
        get => File.SharpTurnCutoff;
        set => Change(p => p.SharpTurnCutoff, value);
    }

    /// <inheritdoc/>
    public double SharpTurnSpdMod
    {
        get => File.SharpTurnSpdMod;
        set => Change(p => p.SharpTurnSpdMod, value);
    }

    /// <inheritdoc/>
    public IList<double> ShiftBlip => _shiftBlip ??= GetObservable(File.ShiftBlip);

    /// <inheritdoc/>
    public double SlideAssistanceFactor
    {
        get => File.SlideAssistanceFactor;
        set => Change(p => p.SlideAssistanceFactor, value);
    }

    /// <inheritdoc/>
    public double SlideMult
    {
        get => File.SlideMult;
        set => Change(p => p.SlideMult, value);
    }

    /// <inheritdoc/>
    public double SlideVelocityCap
    {
        get => File.SlideVelocityCap;
        set => Change(p => p.SlideVelocityCap, value);
    }

    /// <inheritdoc/>
    public double SpinVelocityCap
    {
        get => File.SpinVelocityCap;
        set => Change(p => p.SpinVelocityCap, value);
    }

    /// <inheritdoc/>
    public double SpoilerActivationSpeed
    {
        get => File.SpoilerActivationSpeed;
        set => Change(p => p.SpoilerActivationSpeed, value);
    }

    /// <inheritdoc/>
    public int SpoilerFunctionType
    {
        get => File.SpoilerFunctionType;
        set => Change(p => p.SpoilerFunctionType, value);
    }

    /// <inheritdoc/>
    public double SteeringSpeed
    {
        get => File.SteeringSpeed;
        set => Change(p => p.SteeringSpeed, value);
    }

    /// <inheritdoc/>
    public double SubdivideLevel
    {
        get => File.SubdivideLevel;
        set => Change(p => p.SubdivideLevel, value);
    }

    /// <inheritdoc/>
    public double SuspensionDamage
    {
        get => File.SuspensionDamage;
        set => Change(p => p.SuspensionDamage, value);
    }

    /// <inheritdoc/>
    public double SuspensionStiffness
    {
        get => File.SuspensionStiffness;
        set => Change(p => p.SuspensionStiffness, value);
    }

    /// <inheritdoc/>
    public double TireFactor
    {
        get => File.TireFactor;
        set => Change(p => p.TireFactor, value);
    }

    /// <inheritdoc/>
    public int TireRimFront
    {
        get => File.TireRimFront;
        set => Change(p => p.TireRimFront, value);
    }

    /// <inheritdoc/>
    public int TireRimRear
    {
        get => File.TireRimRear;
        set => Change(p => p.TireRimRear, value);
    }

    /// <inheritdoc/>
    public int TireSidewallFront
    {
        get => File.TireSidewallFront;
        set => Change(p => p.TireSidewallFront, value);
    }

    /// <inheritdoc/>
    public int TireSidewallRear
    {
        get => File.TireSidewallRear;
        set => Change(p => p.TireSidewallRear, value);
    }

    /// <inheritdoc/>
    public double TireWear
    {
        get => File.TireWear;
        set => Change(p => p.TireWear, value);
    }

    /// <inheritdoc/>
    public int TireWidthFront
    {
        get => File.TireWidthFront;
        set => Change(p => p.TireWidthFront, value);
    }

    /// <inheritdoc/>
    public int TireWidthRear
    {
        get => File.TireWidthRear;
        set => Change(p => p.TireWidthRear, value);
    }

    /// <inheritdoc/>
    public double TopSpeed
    {
        get => File.TopSpeed;
        set => Change(p => p.TopSpeed, value);
    }

    /// <inheritdoc/>
    public IList<double> TorqueCurve => _torqueCurve ??= GetObservable(File.TorqueCurve);

    /// <inheritdoc/>
    public double TurnCircleRadius
    {
        get => File.TurnCircleRadius;
        set => Change(p => p.TurnCircleRadius, value);
    }

    /// <inheritdoc/>
    public double TurnInRamp
    {
        get => File.TurnInRamp;
        set => Change(p => p.TurnInRamp, value);
    }

    /// <inheritdoc/>
    public double TurnOutRamp
    {
        get => File.TurnOutRamp;
        set => Change(p => p.TurnOutRamp, value);
    }

    /// <inheritdoc/>
    public IList<double> VelocityToRpmAuto => _velocityToRpmAuto ??= GetObservable(File.VelocityToRpmAuto);

    /// <inheritdoc/>
    public IList<double> VelocityToRpmManual => _velocityToRpmManual ??= GetObservable(File.VelocityToRpmManual);

    /// <inheritdoc/>
    public double WheelBase
    {
        get => File.WheelBase;
        set => Change(p => p.WheelBase, value);
    }

    /// <inheritdoc/>
    public ushort SerialNumber
    {
        get => File.SerialNumber;
        set => Change(p => p.SerialNumber, value);
    }
}
