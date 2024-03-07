namespace TheXDS.Vivianne.Models;

/// <summary>
/// Contains the car performance data.
/// </summary>
public class Carp
{
    public class Gearbox
    {
        public ushort ShiftBlip { get; set; }

        public ushort BrakeBlip { get; set; }

        /// <summary>
        /// Gets or sets the velocity to RPM ratio.
        /// </summary>
        /// <remarks>
        /// Actual speed in M/s = max RPM / this value.
        /// </remarks>
        public double VelocityToRpm { get; set; }

        public double GearRatio { get; set; }

        public double GearEfficiency { get; set; }
    }

    /// <summary>
    /// Gets or sets the car's serial number.
    /// </summary>
    public ushort SerialNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the car class.
    /// </summary>
    public byte CarClassification { get; set; }
    
    /// <summary>
    /// Gets or sets the car mass, in KG.
    /// </summary>
    public double Mass { get; set; }

    /// <summary>
    /// Gets or sets the gear shift delay, in ticks.
    /// </summary>
    public byte GearShiftDelay { get; set; }

    public Gearbox[] Manual {  get; set; } = [];
    
    public Gearbox[] Automatic { get; } = [];

    public double[] TorqueCurve { get; } = [];

    public double FinalGearRatio { get; set; }

    public double FinalGearAuto { get; set; }

    public ushort MinRpm { get; set; }
    public ushort MaxRpm { get; set; }

    /// <summary>
    /// Max car speed in M/s.
    /// </summary>
    public double MaxVelocity { get; set; }

    /// <summary>
    /// Top speed cap for the car in M/s. Generally matches <see cref="MaxVelocity"/>.
    /// </summary>
    public double SpeedCap { get; set; }

    public double FrontDriveRatio { get; set; }

    public bool AntiLockBrakes { get; set; }

    public double MaxBrakeDecel { get; set; }
    public double FrontBrakeBias { get; set; }
    public int[] GasIncreaseCurve { get; } = [];
    public int[] GasDecreaseCurve { get; } = [];
    public double[] BrakeIncreaseCurve { get; } = [];
    public double[] BrakeDecreaseCurve { get; } = [];
    public double WheelBase { get; set; }
    public double FrontGripBias { get; set; }
    public bool PowerSteering { get; set; }
    public double MinSteerAccel { get; set; }
    public double TurnInRamp { get; set; }
    public double TurnOutRamp { get; set; }
    public double LateralAccelerationGripMult { get; set; }
    public double AeroDownforceMult { get; set; }
    public double GasOffFactor { get; set; }
    public double GTransferFactor { get; set; }
    public double TurningRadius { get; set; }
    public (ushort Width, byte Ratio, byte Rim) TireSpecFront { get; set; }
    public (ushort Width, byte Ratio, byte Rim) TireSpecRear { get; set; }
    public double TireWear { get; set; }
    public double SlideMultiplier { get; set; }
    public double SpinVelocityCap { get; set; }
    public double SlideVelocityCap { get; set; }
    public double SlideAssistanceFactor { get; set; }
    public int PushFactor { get; set; }
    public double LowTurnFactor { get; set; }
    public double HighTurnFactor { get; set; }
    public double PitchRollFactor { get; set; }
    public double RoadBumpFactor { get; set; }
    public byte SpoilerFuncType { get; set; }
    public double SpoilerActSpeed { get; set; }
    public int GradualTurnCutoff { get; set; }
    public int MediumTurnCutoff { get; set; }
    public int SharpTurnCutoff { get; set; }
    public double MediumTurnSpeedModifier { get; set; }
    public double SharpTurnSpeedModifier { get; set; }
    public double ExtremeTurnSpeedModifier { get; set; }
    public int SubdivideLevel { get; set; }
    public double CameraArm { get; set; }
    public double BodyDamage { get; set; }
    public double EngineDamage { get; set; }
    public double SuspensionDamage { get; set; }
    public double EngineTunning { get; set; }
    public double BrakeBalance { get; set; }
    public double SteeringSPeed { get; set; }
    public double GearRatFactor { get; set; }
    public double SuspensionStiffness { get; set; }
    public double AeroFactor { get; set; }
    public double TireFactor { get; set; }
    public double[,] AiAccTable { get; set; }
}
