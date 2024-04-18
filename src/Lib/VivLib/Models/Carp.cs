namespace TheXDS.Vivianne.Models;

public class Carp
{
    public int SerialNumber { get; set; }
    public Nfs3CarClass CarClass { get; set; }
    public double Mass { get; set; }
    public int NumberOfGearsManual { get; set; }
    public int NumberOfGearsAuto { get; set; }
    public int GearShiftDelay { get; set; }
    public ICollection<int> ShiftBlip { get; } = [];
    public ICollection<int> BrakeBlip { get; } = [];
    public ICollection<double> VelocityToRpmManual { get; } = [];
    public ICollection<double> VelocityToRpmAuto { get; } = [];
    public ICollection<double> GearRatioManual { get; } = [];
    public ICollection<double> GearRatioAuto { get; } = [];
    public ICollection<double> GearEfficiencyManual { get; } = [];
    public ICollection<double> GearEfficiencyAuto { get; } = [];
    public ICollection<double> TorqueCurve { get; } = [];
    public double FinalGearManual { get; set; }
    public double FinalGearAuto { get; set; }
    public int EngineMinRpm { get; set; }
    public int EngineMaxRpm { get; set; }
    public double MaxVelocity { get; set; }
    public double TopSpeed { get; set; }
    public double FrontDriveRatio { get; set; }
    public bool Abs { get; set; }
    public double MaxBrakeDecel { get; set; }
    public double FrontBrakeBias { get; set; }
    public ICollection<int> GasIncreaseCurve { get; } = [];
    public ICollection<int> GasDecreaseCurve { get; } = [];
    public ICollection<double> BrakeIncreaseCurve { get; } = [];
    public ICollection<double> BrakeDecreaseCurve { get; } = [];
    public double WheelBase { get; set; }
    public double FrontGripBias { get; set; }
    public bool PowerSteering { get; set; }
    public double MinimumSteerAccel { get; set; }
    public double TurnInRamp { get; set; }
    public double TurnOutRamp { get; set; }
    public double LateralAccGripMult { get; set; }
    public double AeroDownMult { get; set; }
    public double GasOffFactor { get; set; }
    public double GTransferFactor { get; set; }
    public double TurnCircleRadius { get; set; }
    public int TireWidthFront { get; set; }
    public int TireSidewallFront { get; set; }
    public int TireRimFront { get; set; }
    public int TireWidthRear { get; set; }
    public int TireSidewallRear { get; set; }
    public int TireRimRear { get; set; }
    public double TireWear { get; set; }
    public double SlideMult { get; set; }
    public double SpinVelocityCap { get; set; }
    public double SlideVelocityCap { get; set; }
    public double SlideAssistanceFactor { get; set; }
    public double PushFactor { get; set; }
    public double LowTurnFactor { get; set; }
    public double HighTurnFactor { get; set; }
    public double PitchRollFactor { get; set; }
    public double RoadBumpFactor { get; set; }
    public int SpoilerFunctionType { get; set; }
    public double SpoilerActivationSpeed { get; set; }
    public double GradualTurnCutoff { get; set; }
    public double MediumTurnCutoff { get; set; }
    public double SharpTurnCutoff { get; set; }
    public double MediumTurnSpdMod { get; set; }
    public double SharpTurnSpdMod { get; set; }
    public double ExtremeTurnSpdMod { get; set; }
    public double SubdivideLevel { get; set; }
    public double CameraArm { get; set; }
    public double BodyDamage { get; set; }
    public double EngineDamage { get; set; }
    public double SuspensionDamage { get; set; }
    public double EngineTuning { get; set; }
    public double BrakeBalance { get; set; }
    public double SteeringSpeed { get; set; }
    public double GearRatFactor { get; set; }
    public double SuspensionStiffness { get; set; }
    public double AeroFactor { get; set; }
    public double TireFactor { get; set; }
    public ICollection<double> AiCurve0 { get; } = [];
    public ICollection<double> AiCurve1 { get; } = [];
    public ICollection<double> AiCurve2 { get; } = [];
    public ICollection<double> AiCurve3 { get; } = [];
    public ICollection<double> AiCurve4 { get; } = [];
    public ICollection<double> AiCurve5 { get; } = [];
    public ICollection<double> AiCurve6 { get; } = [];
    public ICollection<double> AiCurve7 { get; } = [];
}
