using System.Collections.Generic;
using System.Collections.ObjectModel;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Vivianne.Models;

public partial class CarpEditorState : EditorViewModelStateBase
{
    public int SerialNumber
    {
        get => _SerialNumber;
        set => Change(ref _SerialNumber, value);
    }
    public Nfs3CarClass CarClass
    {
        get => _CarClass;
        set => Change(ref _CarClass, value);
    }
    public double Mass
    {
        get => _Mass;
        set => Change(ref _Mass, value);
    }
    public int NumberOfGearsManual
    {
        get => _NumberOfGearsManual;
        set => Change(ref _NumberOfGearsManual, value);
    }
    public int NumberOfGearsAuto
    {
        get => _NumberOfGearsAuto;
        set => Change(ref _NumberOfGearsAuto, value);
    }
    public int GearShiftDelay
    {
        get => _GearShiftDelay;
        set => Change(ref _GearShiftDelay, value);
    }
    public ICollection<int> ShiftBlip { get; } = new ObservableCollection<int>();
    public ICollection<int> BrakeBlip { get; } = new ObservableCollection<int>();
    public ICollection<double> VelocityToRpmManual { get; } = new ObservableCollection<double>();
    public ICollection<double> VelocityToRpmAuto { get; } = new ObservableCollection<double>();
    public ICollection<double> GearRatioManual { get; } = new ObservableCollection<double>();
    public ICollection<double> GearRatioAuto { get; } = new ObservableCollection<double>();
    public ICollection<double> GearEfficiencyManual { get; } = new ObservableCollection<double>();
    public ICollection<double> GearEfficiencyAuto { get; } = new ObservableCollection<double>();
    public ICollection<double> TorqueCurve { get; } = new ObservableCollection<double>();
    public double FinalGearManual
    {
        get => _FinalGearManual;
        set => Change(ref _FinalGearManual, value);
    }
    public double FinalGearAuto
    {
        get => _FinalGearAuto;
        set => Change(ref _FinalGearAuto, value);
    }
    public int EngineMinRpm
    {
        get => _EngineMinRpm;
        set => Change(ref _EngineMinRpm, value);
    }
    public int EngineMaxRpm
    {
        get => _EngineMaxRpm;
        set => Change(ref _EngineMaxRpm, value);
    }
    public double MaxVelocity
    {
        get => _MaxVelocity;
        set => Change(ref _MaxVelocity, value);
    }
    public double TopSpeed
    {
        get => _TopSpeed;
        set => Change(ref _TopSpeed, value);
    }
    public double FrontDriveRatio
    {
        get => _FrontDriveRatio;
        set => Change(ref _FrontDriveRatio, value);
    }
    public bool Abs
    {
        get => _Abs;
        set => Change(ref _Abs, value);
    }
    public double MaxBrakeDecel
    {
        get => _MaxBrakeDecel;
        set => Change(ref _MaxBrakeDecel, value);
    }
    public double FrontBrakeBias
    {
        get => _FrontBrakeBias;
        set => Change(ref _FrontBrakeBias, value);
    }
    public ICollection<int> GasIncreaseCurve { get; } = new ObservableCollection<int>();
    public ICollection<int> GasDecreaseCurve { get; } = new ObservableCollection<int>();
    public ICollection<double> BrakeIncreaseCurve { get; } = new ObservableCollection<double>();
    public ICollection<double> BrakeDecreaseCurve { get; } = new ObservableCollection<double>();
    public double WheelBase
    {
        get => _WheelBase;
        set => Change(ref _WheelBase, value);
    }
    public double FrontGripBias
    {
        get => _FrontGripBias;
        set => Change(ref _FrontGripBias, value);
    }
    public bool PowerSteering
    {
        get => _PowerSteering;
        set => Change(ref _PowerSteering, value);
    }
    public double MinimumSteerAccel
    {
        get => _MinimumSteerAccel;
        set => Change(ref _MinimumSteerAccel, value);
    }
    public double TurnInRamp
    {
        get => _TurnInRamp;
        set => Change(ref _TurnInRamp, value);
    }
    public double TurnOutRamp
    {
        get => _TurnOutRamp;
        set => Change(ref _TurnOutRamp, value);
    }
    public double LateralAccGripMult
    {
        get => _LateralAccGripMult;
        set => Change(ref _LateralAccGripMult, value);
    }
    public double AeroDownMult
    {
        get => _AeroDownMult;
        set => Change(ref _AeroDownMult, value);
    }
    public double GasOffFactor
    {
        get => _GasOffFactor;
        set => Change(ref _GasOffFactor, value);
    }
    public double GTransferFactor
    {
        get => _GTransferFactor;
        set => Change(ref _GTransferFactor, value);
    }
    public double TurnCircleRadius
    {
        get => _TurnCircleRadius;
        set => Change(ref _TurnCircleRadius, value);
    }
    public int TireWidthFront
    {
        get => _TireWidthFront;
        set => Change(ref _TireWidthFront, value);
    }
    public int TireSidewallFront
    {
        get => _TireSidewallFront;
        set => Change(ref _TireSidewallFront, value);
    }
    public int TireRimFront
    {
        get => _TireRimFront;
        set => Change(ref _TireRimFront, value);
    }
    public int TireWidthRear
    {
        get => _TireWidthRear;
        set => Change(ref _TireWidthRear, value);
    }
    public int TireSidewallRear
    {
        get => _TireSidewallRear;
        set => Change(ref _TireSidewallRear, value);
    }
    public int TireRimRear
    {
        get => _TireRimRear;
        set => Change(ref _TireRimRear, value);
    }
    public double TireWear
    {
        get => _TireWear;
        set => Change(ref _TireWear, value);
    }
    public double SlideMult
    {
        get => _SlideMult;
        set => Change(ref _SlideMult, value);
    }
    public double SpinVelocityCap
    {
        get => _SpinVelocityCap;
        set => Change(ref _SpinVelocityCap, value);
    }
    public double SlideVelocityCap
    {
        get => _SlideVelocityCap;
        set => Change(ref _SlideVelocityCap, value);
    }
    public double SlideAssistanceFactor
    {
        get => _SlideAssistanceFactor;
        set => Change(ref _SlideAssistanceFactor, value);
    }
    public double PushFactor
    {
        get => _PushFactor;
        set => Change(ref _PushFactor, value);
    }
    public double LowTurnFactor
    {
        get => _LowTurnFactor;
        set => Change(ref _LowTurnFactor, value);
    }
    public double HighTurnFactor
    {
        get => _HighTurnFactor;
        set => Change(ref _HighTurnFactor, value);
    }
    public double PitchRollFactor
    {
        get => _PitchRollFactor;
        set => Change(ref _PitchRollFactor, value);
    }
    public double RoadBumpFactor
    {
        get => _RoadBumpFactor;
        set => Change(ref _RoadBumpFactor, value);
    }
    public int SpoilerFunctionType
    {
        get => _SpoilerFunctionType;
        set => Change(ref _SpoilerFunctionType, value);
    }
    public double SpoilerActivationSpeed
    {
        get => _SpoilerActivationSpeed;
        set => Change(ref _SpoilerActivationSpeed, value);
    }
    public double GradualTurnCutoff
    {
        get => _GradualTurnCutoff;
        set => Change(ref _GradualTurnCutoff, value);
    }
    public double MediumTurnCutoff
    {
        get => _MediumTurnCutoff;
        set => Change(ref _MediumTurnCutoff, value);
    }
    public double SharpTurnCutoff
    {
        get => _SharpTurnCutoff;
        set => Change(ref _SharpTurnCutoff, value);
    }
    public double MediumTurnSpdMod
    {
        get => _MediumTurnSpdMod;
        set => Change(ref _MediumTurnSpdMod, value);
    }
    public double SharpTurnSpdMod
    {
        get => _SharpTurnSpdMod;
        set => Change(ref _SharpTurnSpdMod, value);
    }
    public double ExtremeTurnSpdMod
    {
        get => _ExtremeTurnSpdMod;
        set => Change(ref _ExtremeTurnSpdMod, value);
    }
    public double SubdivideLevel
    {
        get => _SubdivideLevel;
        set => Change(ref _SubdivideLevel, value);
    }
    public double CameraArm
    {
        get => _CameraArm;
        set => Change(ref _CameraArm, value);
    }
    public double BodyDamage
    {
        get => _BodyDamage;
        set => Change(ref _BodyDamage, value);
    }
    public double EngineDamage
    {
        get => _EngineDamage;
        set => Change(ref _EngineDamage, value);
    }
    public double SuspensionDamage
    {
        get => _SuspensionDamage;
        set => Change(ref _SuspensionDamage, value);
    }
    public double EngineTuning
    {
        get => _EngineTuning;
        set => Change(ref _EngineTuning, value);
    }
    public double BrakeBalance
    {
        get => _BrakeBalance;
        set => Change(ref _BrakeBalance, value);
    }
    public double SteeringSpeed
    {
        get => _SteeringSpeed;
        set => Change(ref _SteeringSpeed, value);
    }
    public double GearRatFactor
    {
        get => _GearRatFactor;
        set => Change(ref _GearRatFactor, value);
    }
    public double SuspensionStiffness
    {
        get => _SuspensionStiffness;
        set => Change(ref _SuspensionStiffness, value);
    }
    public double AeroFactor
    {
        get => _AeroFactor;
        set => Change(ref _AeroFactor, value);
    }
    public double TireFactor
    {
        get => _TireFactor;
        set => Change(ref _TireFactor, value);
    }
    public ICollection<double> AiCurve0 { get; } = new ObservableCollection<double>();
    public ICollection<double> AiCurve1 { get; } = new ObservableCollection<double>();
    public ICollection<double> AiCurve2 { get; } = new ObservableCollection<double>();
    public ICollection<double> AiCurve3 { get; } = new ObservableCollection<double>();
    public ICollection<double> AiCurve4 { get; } = new ObservableCollection<double>();
    public ICollection<double> AiCurve5 { get; } = new ObservableCollection<double>();
    public ICollection<double> AiCurve6 { get; } = new ObservableCollection<double>();
    public ICollection<double> AiCurve7 { get; } = new ObservableCollection<double>();
}
