using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// State that describes the Car's performance data.
/// </summary>
public partial class CarpEditorState : EditorViewModelStateBase
{
    /// <summary>
    /// Gets or sets the car's serial number.
    /// </summary>
    public ushort SerialNumber
    {
        get => _SerialNumber;
        set => Change(ref _SerialNumber, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the Car class as set on the FeData.
    /// </summary>
    public Nfs3CarClass CarClass
    {
        get => _CarClass;
        set => Change(ref _CarClass, value);
    }

    /// <summary>
    /// Gets or sets the car mass in Kilograms.
    /// </summary>
    public double Mass
    {
        get => _Mass;
        set => Change(ref _Mass, value);
    }

    /// <summary>
    /// Gets a value that indicates the number of defined gears for the manual
    /// transmission.
    /// </summary>
    /// <remarks>
    /// Count includes reverse gear as well as neutral.
    /// </remarks>
    public int NumberOfGearsManual
    {
        get => _NumberOfGearsManual;
        set => Change(ref _NumberOfGearsManual, value);
    }

    /// <summary>
    /// Gets a value that indicates the number of defined gears for the
    /// automatic transmission.
    /// </summary>
    /// <remarks>
    /// Count includes reverse gear as well as neutral.
    /// </remarks>
    public int NumberOfGearsAuto
    {
        get => _NumberOfGearsAuto;
        set => Change(ref _NumberOfGearsAuto, value);
    }

    /// <summary>
    /// Gets a value that indicates the number of game ticks of delay between
    /// gear shifts.
    /// </summary>
    public int GearShiftDelay
    {
        get => _GearShiftDelay;
        set => Change(ref _GearShiftDelay, value);
    }

    /// <summary>
    /// Gets a collection of values that indicate the RPM blip to be applied
    /// when shifting gears up.
    /// </summary>
    /// <remarks>
    /// The number of elements in this array should idealy remain at 8.
    /// </remarks>
    public ICollection<double> ShiftBlip { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that indicate the RPM blip to be applied
    /// when shifting gears down.
    /// </summary>
    /// <remarks>
    /// The number of elements in this array should idealy remain at 8.
    /// </remarks>
    public ICollection<double> BrakeBlip { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that indicate a factor to divide the RPM by
    /// to calculate the actual top speed in any given gear for the manual
    /// transmission.
    /// </summary>
    public ICollection<double> VelocityToRpmManual { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that indicate a factor to divide the RPM by
    /// to calculate the actual top speed in any given gear for the automatic
    /// transmission.
    /// </summary>
    public ICollection<double> VelocityToRpmAuto { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that indicate gear ratio for any given gear
    /// for the manual transmission.
    /// </summary>
    /// <remarks>
    /// These values are merely informational, and are apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public ICollection<double> GearRatioManual { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that indicate gear ratio for any given gear
    /// for the automatic transmission.
    /// </summary>
    /// <remarks>
    /// These values are merely informational, and are apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public ICollection<double> GearRatioAuto { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that indicates the gear efficiency
    /// multiplier for any given gear for the manual transmission.
    /// </summary>
    public ICollection<double> GearEfficiencyManual { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that indicates the gear efficiency
    /// multiplier for any given gear for the automatic transmission.
    /// </summary>
    public ICollection<double> GearEfficiencyAuto { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values plotting the engine's torque curve.
    /// </summary>
    public ICollection<double> TorqueCurve { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a value that indicates what is the final gear ratio for the manual
    /// transmission.
    /// </summary>
    /// <remarks>
    /// This value is merely informational, and is apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public double FinalGearManual
    {
        get => _FinalGearManual;
        set => Change(ref _FinalGearManual, value);
    }

    /// <summary>
    /// Gets a value that indicates what is the final gear ratio for the
    /// automatic transmission.
    /// </summary>
    /// <remarks>
    /// This value is merely informational, and is apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public double FinalGearAuto
    {
        get => _FinalGearAuto;
        set => Change(ref _FinalGearAuto, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the idling engine RPM.
    /// </summary>
    public int EngineMinRpm
    {
        get => _EngineMinRpm;
        set => Change(ref _EngineMinRpm, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the maximum engine RPM
    /// </summary>
    public int EngineMaxRpm
    {
        get => _EngineMaxRpm;
        set => Change(ref _EngineMaxRpm, value);
    }

    /// <summary>
    /// Gets the maximum velocity this car can reach, in meters per second.
    /// </summary>
    /// <remarks>
    /// Speed calculation uses this value alongside the current RPM and the
    /// velocity to RPM tables to calculate the actual speed.
    /// </remarks>
    public double MaxVelocity
    {
        get => _MaxVelocity;
        set => Change(ref _MaxVelocity, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the top speed of the car in meters
    /// per second.
    /// </summary>
    /// <remarks>
    /// This value seems to be informational in nature, but should match the
    /// value in <see cref="MaxVelocity"/>
    /// </remarks>
    public double TopSpeed
    {
        get => _TopSpeed;
        set => Change(ref _TopSpeed, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the front differential gear ratio.
    /// </summary>
    /// <remarks>
    /// This value is merely informational, and is apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public double FrontDriveRatio
    {
        get => _FrontDriveRatio;
        set => Change(ref _FrontDriveRatio, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates if the car is equipped with ABS.
    /// </summary>
    public bool Abs
    {
        get => _Abs;
        set => Change(ref _Abs, value);
    }

    /// <summary>
    /// Gets a value that indicates the maximum deceleration while braking.
    /// </summary>
    public double MaxBrakeDecel
    {
        get => _MaxBrakeDecel;
        set => Change(ref _MaxBrakeDecel, value);
    }

    /// <summary>
    /// Gets or sets a value between 0.0 and 1.0 that indicates the braking
    /// bias to the front brakes.
    /// </summary>
    public double FrontBrakeBias
    {
        get => _FrontBrakeBias;
        set => Change(ref _FrontBrakeBias, value);
    }

    /// <summary>
    /// Gets a collection of values taht indicates how the gas increases when
    /// applied.
    /// </summary>
    public ICollection<double> GasIncreaseCurve { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values taht indicates how the gas decreases when
    /// released.
    /// </summary>
    public ICollection<double> GasDecreaseCurve { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values taht indicates how the brakes increase when
    /// applied.
    /// </summary>
    public ICollection<double> BrakeIncreaseCurve { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values taht indicates how the brakes decrease when
    /// released.
    /// </summary>
    public ICollection<double> BrakeDecreaseCurve { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets or sets a value that indicates the wheel base for the car (the
    /// distance between front and rear tires).
    /// </summary>
    public double WheelBase
    {
        get => _WheelBase;
        set => Change(ref _WheelBase, value);
    }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 indicating the Bias in grip
    /// between the front and rear of the car.
    /// </summary>
    public double FrontGripBias
    {
        get => _FrontGripBias;
        set => Change(ref _FrontGripBias, value);
    }

    /// <summary>
    /// Gets a value that indicates whether or not this car is equipped with
    /// Power steering.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value that indicates the tire width for the front
    /// wheels in millimeters.
    /// </summary>
    public int TireWidthFront
    {
        get => _TireWidthFront;
        set => Change(ref _TireWidthFront, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the proportion of sidewall/width
    /// for the front tires expressed as an integer percentage.
    /// </summary>
    public int TireSidewallFront
    {
        get => _TireSidewallFront;
        set => Change(ref _TireSidewallFront, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the rim diameter for the front
    /// wheels in inches.
    /// </summary>
    public int TireRimFront
    {
        get => _TireRimFront;
        set => Change(ref _TireRimFront, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the tire width for the rear
    /// wheels in millimeters.
    /// </summary>
    public int TireWidthRear
    {
        get => _TireWidthRear;
        set => Change(ref _TireWidthRear, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the proportion of sidewall/width
    /// for the rear tires expressed as an integer percentage.
    /// </summary>
    public int TireSidewallRear
    {
        get => _TireSidewallRear;
        set => Change(ref _TireSidewallRear, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the rim diameter for the rear
    /// wheels in inches.
    /// </summary>
    public int TireRimRear
    {
        get => _TireRimRear;
        set => Change(ref _TireRimRear, value);
    }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of tire
    /// wear.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
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

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of body
    /// damage.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    public double BodyDamage
    {
        get => _BodyDamage;
        set => Change(ref _BodyDamage, value);
    }
    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of
    /// engine damage.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    public double EngineDamage
    {
        get => _EngineDamage;
        set => Change(ref _EngineDamage, value);
    }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of
    /// suspension damage.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    public double SuspensionDamage
    {
        get => _SuspensionDamage;
        set => Change(ref _SuspensionDamage, value);
    }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// engine on the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double EngineTuning
    {
        get => _EngineTuning;
        set => Change(ref _EngineTuning, value);
    }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// brake balance on the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double BrakeBalance
    {
        get => _BrakeBalance;
        set => Change(ref _BrakeBalance, value);
    }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// steering responsiveness of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double SteeringSpeed
    {
        get => _SteeringSpeed;
        set => Change(ref _SteeringSpeed, value);
    }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the gear
    /// ratios of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double GearRatFactor
    {
        get => _GearRatFactor;
        set => Change(ref _GearRatFactor, value);
    }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// suspension stiffness of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double SuspensionStiffness
    {
        get => _SuspensionStiffness;
        set => Change(ref _SuspensionStiffness, value);
    }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// aerodynamic load of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double AeroFactor
    {
        get => _AeroFactor;
        set => Change(ref _AeroFactor, value);
    }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the tire
    /// grip of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double TireFactor
    {
        get => _TireFactor;
        set => Change(ref _TireFactor, value);
    }

    /// <summary>
    /// Gets a collection of values that plot the first part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve0 { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that plot the second part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve1 { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that plot the third part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve2 { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that plot the fourth part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve3 { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that plot the fifth part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve4 { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that plot the sixth part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve5 { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that plot the seventh part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve6 { get; } = new ObservableCollection<double>();

    /// <summary>
    /// Gets a collection of values that plot the eighth part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve7 { get; } = new ObservableCollection<double>();
}
