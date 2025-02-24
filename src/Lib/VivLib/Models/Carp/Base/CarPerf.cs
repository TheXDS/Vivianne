using System.Collections.ObjectModel;

namespace TheXDS.Vivianne.Models.Carp.Base;

/// <summary>
/// Describes Car performance data.
/// </summary>
public class CarPerf<TCarClass> : ICarPerf, ICarClass<TCarClass> where TCarClass : unmanaged, Enum
{
    /// <summary>
    /// Gets or sets the car's serial number as set on the FeData.
    /// </summary>
    public ushort SerialNumber { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the Car class as set on the FeData.
    /// </summary>
    public TCarClass CarClass { get; set; }

    /// <inheritdoc/>
    public double Mass { get; set; }

    /// <inheritdoc/>
    public int NumberOfGearsManual { get; set; }

    /// <inheritdoc/>
    public int NumberOfGearsAuto { get; set; }

    /// <inheritdoc/>
    public int GearShiftDelay { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<double> ShiftBlip { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> BrakeBlip { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> VelocityToRpmManual { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> VelocityToRpmAuto { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> GearRatioManual { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> GearRatioAuto { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> GearEfficiencyManual { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> GearEfficiencyAuto { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> TorqueCurve { get; } = [];

    /// <inheritdoc/>
    public double FinalGearManual { get; set; }

    /// <inheritdoc/>
    public double FinalGearAuto { get; set; }

    /// <inheritdoc/>
    public int EngineMinRpm { get; set; }

    /// <inheritdoc/>
    public int EngineMaxRpm { get; set; }

    /// <inheritdoc/>
    public double MaxVelocity { get; set; }

    /// <inheritdoc/>
    public double TopSpeed { get; set; }

    /// <inheritdoc/>
    public double FrontDriveRatio { get; set; }

    /// <inheritdoc/>
    public bool Abs { get; set; }

    /// <inheritdoc/>
    public double MaxBrakeDecel { get; set; }

    /// <inheritdoc/>
    public double FrontBrakeBias { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<double> GasIncreaseCurve { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> GasDecreaseCurve { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> BrakeIncreaseCurve { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> BrakeDecreaseCurve { get; } = [];

    /// <inheritdoc/>
    public double WheelBase { get; set; }

    /// <inheritdoc/>
    public double FrontGripBias { get; set; }

    /// <inheritdoc/>
    public bool PowerSteering { get; set; }

    /// <inheritdoc/>
    public double MinimumSteerAccel { get; set; }

    /// <inheritdoc/>
    public double TurnInRamp { get; set; }

    /// <inheritdoc/>
    public double TurnOutRamp { get; set; }

    /// <inheritdoc/>
    public double LateralAccGripMult { get; set; }

    /// <inheritdoc/>
    public double AeroDownMult { get; set; }

    /// <inheritdoc/>
    public double GasOffFactor { get; set; }

    /// <inheritdoc/>
    public double GTransferFactor { get; set; }

    /// <inheritdoc/>
    public double TurnCircleRadius { get; set; }

    /// <inheritdoc/>
    public int TireWidthFront { get; set; }

    /// <inheritdoc/>
    public int TireSidewallFront { get; set; }

    /// <inheritdoc/>
    public int TireRimFront { get; set; }

    /// <inheritdoc/>
    public int TireWidthRear { get; set; }

    /// <inheritdoc/>
    public int TireSidewallRear { get; set; }

    /// <inheritdoc/>
    public int TireRimRear { get; set; }

    /// <inheritdoc/>
    public double TireWear { get; set; }

    /// <inheritdoc/>
    public double SlideMult { get; set; }

    /// <inheritdoc/>
    public double SpinVelocityCap { get; set; }

    /// <inheritdoc/>
    public double SlideVelocityCap { get; set; }

    /// <inheritdoc/>
    public double SlideAssistanceFactor { get; set; }

    /// <inheritdoc/>
    public double PushFactor { get; set; }

    /// <inheritdoc/>
    public double LowTurnFactor { get; set; }

    /// <inheritdoc/>
    public double HighTurnFactor { get; set; }

    /// <inheritdoc/>
    public double PitchRollFactor { get; set; }

    /// <inheritdoc/>
    public double RoadBumpFactor { get; set; }

    /// <inheritdoc/>
    public int SpoilerFunctionType { get; set; }

    /// <inheritdoc/>
    public double SpoilerActivationSpeed { get; set; }

    /// <inheritdoc/>
    public double GradualTurnCutoff { get; set; }

    /// <inheritdoc/>
    public double MediumTurnCutoff { get; set; }

    /// <inheritdoc/>
    public double SharpTurnCutoff { get; set; }

    /// <inheritdoc/>
    public double MediumTurnSpdMod { get; set; }

    /// <inheritdoc/>
    public double SharpTurnSpdMod { get; set; }

    /// <inheritdoc/>
    public double ExtremeTurnSpdMod { get; set; }

    /// <inheritdoc/>
    public double SubdivideLevel { get; set; }

    /// <inheritdoc/>
    public double CameraArm { get; set; }

    /// <inheritdoc/>
    public double BodyDamage { get; set; }

    /// <inheritdoc/>
    public double EngineDamage { get; set; }

    /// <inheritdoc/>
    public double SuspensionDamage { get; set; }

    /// <inheritdoc/>
    public double EngineTuning { get; set; }

    /// <inheritdoc/>
    public double BrakeBalance { get; set; }

    /// <inheritdoc/>
    public double SteeringSpeed { get; set; }

    /// <inheritdoc/>
    public double GearRatFactor { get; set; }

    /// <inheritdoc/>
    public double SuspensionStiffness { get; set; }

    /// <inheritdoc/>
    public double AeroFactor { get; set; }

    /// <inheritdoc/>
    public double TireFactor { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<double> AiCurve0 { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> AiCurve1 { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> AiCurve2 { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> AiCurve3 { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> AiCurve4 { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> AiCurve5 { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> AiCurve6 { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<double> AiCurve7 { get; } = [];
}
