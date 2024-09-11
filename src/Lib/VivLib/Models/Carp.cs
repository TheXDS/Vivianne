namespace TheXDS.Vivianne.Models;

/// <summary>
/// Describes Car performance data.
/// </summary>
public class Carp : ISerialNumberModel
{
    /// <summary>
    /// Gets or sets the car's serial number as set on the FeData.
    /// </summary>
    public ushort SerialNumber { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the Car class as set on the FeData.
    /// </summary>
    public Nfs3CarClass CarClass { get; set; }

    /// <summary>
    /// Gets or sets the car mass in Kilograms.
    /// </summary>
    public double Mass { get; set; }

    /// <summary>
    /// Gets a value that indicates the number of defined gears for the manual
    /// transmission.
    /// </summary>
    /// <remarks>
    /// Count includes reverse gear as well as neutral.
    /// </remarks>
    public int NumberOfGearsManual { get; set; }

    /// <summary>
    /// Gets a value that indicates the number of defined gears for the
    /// automatic transmission.
    /// </summary>
    /// <remarks>
    /// Count includes reverse gear as well as neutral.
    /// </remarks>
    public int NumberOfGearsAuto { get; set; }

    /// <summary>
    /// Gets a value that indicates the number of game ticks of delay between
    /// gear shifts.
    /// </summary>
    public int GearShiftDelay { get; set; }

    /// <summary>
    /// Gets a collection of values that indicate the RPM blip to be applied
    /// when shifting gears up.
    /// </summary>
    /// <remarks>
    /// The number of elements in this array should idealy remain at 8.
    /// </remarks>
    public ICollection<int> ShiftBlip { get; } = [];

    /// <summary>
    /// Gets a collection of values that indicate the RPM blip to be applied
    /// when shifting gears down.
    /// </summary>
    /// <remarks>
    /// The number of elements in this array should idealy remain at 8.
    /// </remarks>
    public ICollection<int> BrakeBlip { get; } = [];

    /// <summary>
    /// Gets a collection of values that indicate a factor to divide the RPM by
    /// to calculate the actual top speed in any given gear for the manual
    /// transmission.
    /// </summary>
    public ICollection<double> VelocityToRpmManual { get; } = [];

    /// <summary>
    /// Gets a collection of values that indicate a factor to divide the RPM by
    /// to calculate the actual top speed in any given gear for the automatic
    /// transmission.
    /// </summary>
    public ICollection<double> VelocityToRpmAuto { get; } = [];

    /// <summary>
    /// Gets a collection of values that indicate gear ratio for any given gear
    /// for the manual transmission.
    /// </summary>
    /// <remarks>
    /// These values are merely informational, and are apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public ICollection<double> GearRatioManual { get; } = [];

    /// <summary>
    /// Gets a collection of values that indicate gear ratio for any given gear
    /// for the automatic transmission.
    /// </summary>
    /// <remarks>
    /// These values are merely informational, and are apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public ICollection<double> GearRatioAuto { get; } = [];

    /// <summary>
    /// Gets a collection of values that indicates the gear efficiency
    /// multiplier for any given gear for the manual transmission.
    /// </summary>
    public ICollection<double> GearEfficiencyManual { get; } = [];

    /// <summary>
    /// Gets a collection of values that indicates the gear efficiency
    /// multiplier for any given gear for the automatic transmission.
    /// </summary>
    public ICollection<double> GearEfficiencyAuto { get; } = [];

    /// <summary>
    /// Gets a collection of values plotting the engine's torque curve.
    /// </summary>
    public ICollection<double> TorqueCurve { get; } = [];

    /// <summary>
    /// Gets a value that indicates what is the final gear ratio for the manual
    /// transmission.
    /// </summary>
    /// <remarks>
    /// This value is merely informational, and is apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public double FinalGearManual { get; set; }

    /// <summary>
    /// Gets a value that indicates what is the final gear ratio for the
    /// automatic transmission.
    /// </summary>
    /// <remarks>
    /// This value is merely informational, and is apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public double FinalGearAuto { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the idling engine RPM.
    /// </summary>
    public int EngineMinRpm { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the maximum engine RPM
    /// </summary>
    public int EngineMaxRpm { get; set; }

    /// <summary>
    /// Gets the maximum velocity this car can reach, in meters per second.
    /// </summary>
    /// <remarks>
    /// Speed calculation uses this value alongside the current RPM and the
    /// velocity to RPM tables to calculate the actual speed.
    /// </remarks>
    public double MaxVelocity { get; set; }

    /// <remarks>
    /// This value seems to be informational in nature, but should match the
    /// value in <see cref="MaxVelocity"/>
    /// </remarks>
    public double TopSpeed { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the front differential gear ratio.
    /// </summary>
    /// <remarks>
    /// This value is merely informational, and is apparently not used for
    /// actual speed calculations.
    /// </remarks>
    public double FrontDriveRatio { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if the car is equipped with ABS.
    /// </summary>
    public bool Abs { get; set; }

    /// <summary>
    /// Gets a value that indicates the maximum deceleration while braking.
    /// </summary>
    public double MaxBrakeDecel { get; set; }

    /// <summary>
    /// Gets or sets a value between 0.0 and 1.0 that indicates the braking
    /// bias to the front brakes.
    /// </summary>
    public double FrontBrakeBias { get; set; }

    /// <summary>
    /// Gets a collection of values taht indicates how the gas increases when
    /// applied.
    /// </summary>
    public ICollection<int> GasIncreaseCurve { get; } = [];

    /// <summary>
    /// Gets a collection of values taht indicates how the gas decreases when
    /// released.
    /// </summary>
    public ICollection<int> GasDecreaseCurve { get; } = [];

    /// <summary>
    /// Gets a collection of values taht indicates how the brakes increase when
    /// applied.
    /// </summary>
    public ICollection<double> BrakeIncreaseCurve { get; } = [];

    /// <summary>
    /// Gets a collection of values taht indicates how the brakes decrease when
    /// released.
    /// </summary>
    public ICollection<double> BrakeDecreaseCurve { get; } = [];

    /// <summary>
    /// Gets or sets a value that indicates the wheel base for the car (the
    /// distance between front and rear tires).
    /// </summary>
    public double WheelBase { get; set; }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 indicating the Bias in grip
    /// between the front and rear of the car.
    /// </summary>
    public double FrontGripBias { get; set; }

    /// <summary>
    /// Gets a value that indicates whether or not this car is equipped with
    /// Power steering.
    /// </summary>
    public bool PowerSteering { get; set; }
    public double MinimumSteerAccel { get; set; }
    public double TurnInRamp { get; set; }
    public double TurnOutRamp { get; set; }
    public double LateralAccGripMult { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the multiplier to be applied to the
    /// aerodynamic downforce generated by the car while in movement.
    /// </summary>
    public double AeroDownMult { get; set; }
    public double GasOffFactor { get; set; }
    public double GTransferFactor { get; set; }
    public double TurnCircleRadius { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the width of the front tire in
    /// milimeters.
    /// </summary>
    public int TireWidthFront { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the ratio between the front tire
    /// width and its sidewall.
    /// </summary>
    public int TireSidewallFront { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the front rim diameter.
    /// </summary>
    public int TireRimFront { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the width of the rear tire in
    /// milimeters.
    /// </summary>
    public int TireWidthRear { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the ratio between the rear tire
    /// width and its sidewall.
    /// </summary>
    public int TireSidewallRear { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the rear rim diameter.
    /// </summary>
    public int TireRimRear { get; set; }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of tire
    /// wear.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
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

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of body
    /// damage.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    public double BodyDamage { get; set; }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of
    /// engine damage.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    public double EngineDamage { get; set; }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of
    /// suspension damage.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    public double SuspensionDamage { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// engine on the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double EngineTuning { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// brake balance on the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double BrakeBalance { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// steering responsiveness of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double SteeringSpeed { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the gear
    /// ratios of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double GearRatFactor { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// suspension stiffness of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double SuspensionStiffness { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// aerodynamic load of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double AeroFactor { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the tire
    /// grip of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    public double TireFactor { get; set; }

    /// <summary>
    /// Gets a collection of values that plot the first part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve0 { get; } = [];

    /// <summary>
    /// Gets a collection of values that plot the second part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve1 { get; } = [];

    /// <summary>
    /// Gets a collection of values that plot the third part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve2 { get; } = [];

    /// <summary>
    /// Gets a collection of values that plot the fourth part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve3 { get; } = [];

    /// <summary>
    /// Gets a collection of values that plot the fifth part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve4 { get; } = [];

    /// <summary>
    /// Gets a collection of values that plot the sixth part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve5 { get; } = [];

    /// <summary>
    /// Gets a collection of values that plot the seventh part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve6 { get; } = [];

    /// <summary>
    /// Gets a collection of values that plot the eighth part of the AI
    /// acceleration curve.
    /// </summary>
    public ICollection<double> AiCurve7 { get; } = [];
}
