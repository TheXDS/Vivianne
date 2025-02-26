﻿using System.Collections.ObjectModel;

namespace TheXDS.Vivianne.Models.Carp;

/// <summary>
/// Describes Car performance data.
/// </summary>
public interface ICarPerf : ISerialNumberModel
{
    /// <summary>
    /// Gets or sets a value that indicates if the car is equipped with ABS.
    /// </summary>
    bool Abs { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the multiplier to be applied to the
    /// aerodynamic downforce generated by the car while in movement.
    /// </summary>
    double AeroDownMult { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// aerodynamic load of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    double AeroFactor { get; set; }

    /// <summary>
    /// Gets a collection of values that plot the first part of the AI
    /// acceleration curve.
    /// </summary>
    ICollection<double> AiCurve0 { get; }

    /// <summary>
    /// Gets a collection of values that plot the second part of the AI
    /// acceleration curve.
    /// </summary>
    ICollection<double> AiCurve1 { get; }

    /// <summary>
    /// Gets a collection of values that plot the third part of the AI
    /// acceleration curve.
    /// </summary>
    ICollection<double> AiCurve2 { get; }

    /// <summary>
    /// Gets a collection of values that plot the fourth part of the AI
    /// acceleration curve.
    /// </summary>
    ICollection<double> AiCurve3 { get; }

    /// <summary>
    /// Gets a collection of values that plot the fifth part of the AI
    /// acceleration curve.
    /// </summary>
    ICollection<double> AiCurve4 { get; }

    /// <summary>
    /// Gets a collection of values that plot the sixth part of the AI
    /// acceleration curve.
    /// </summary>
    ICollection<double> AiCurve5 { get; }

    /// <summary>
    /// Gets a collection of values that plot the seventh part of the AI
    /// acceleration curve.
    /// </summary>
    ICollection<double> AiCurve6 { get; }

    /// <summary>
    /// Gets a collection of values that plot the eighth part of the AI
    /// acceleration curve.
    /// </summary>
    ICollection<double> AiCurve7 { get; }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of body
    /// damage.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    double BodyDamage { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// brake balance on the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    double BrakeBalance { get; set; }

    /// <summary>
    /// Gets a collection of values that indicate the RPM blip to be applied
    /// when shifting gears down.
    /// </summary>
    /// <remarks>
    /// The number of elements in this array should idealy remain at 8.
    /// </remarks>
    ICollection<double> BrakeBlip { get; }

    /// <summary>
    /// Gets a collection of values taht indicates how the brakes decrease when
    /// released.
    /// </summary>
    ICollection<double> BrakeDecreaseCurve { get; }

    /// <summary>
    /// Gets a collection of values taht indicates how the brakes increase when
    /// applied.
    /// </summary>
    ICollection<double> BrakeIncreaseCurve { get; }

    double CameraArm { get; set; }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of
    /// engine damage.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    double EngineDamage { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the maximum engine RPM
    /// </summary>
    int EngineMaxRpm { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the idling engine RPM.
    /// </summary>
    int EngineMinRpm { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// engine on the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    double EngineTuning { get; set; }

    double ExtremeTurnSpdMod { get; set; }

    /// <summary>
    /// Gets a value that indicates what is the final gear ratio for the
    /// automatic transmission.
    /// </summary>
    /// <remarks>
    /// This value is merely informational, and is apparently not used for
    /// actual speed calculations.
    /// </remarks>
    double FinalGearAuto { get; set; }

    /// <summary>
    /// Gets a value that indicates what is the final gear ratio for the manual
    /// transmission.
    /// </summary>
    /// <remarks>
    /// This value is merely informational, and is apparently not used for
    /// actual speed calculations.
    /// </remarks>
    double FinalGearManual { get; set; }

    /// <summary>
    /// Gets or sets a value between 0.0 and 1.0 that indicates the braking
    /// bias to the front brakes.
    /// </summary>
    double FrontBrakeBias { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the front differential gear ratio.
    /// </summary>
    /// <remarks>
    /// This value is merely informational, and is apparently not used for
    /// actual speed calculations.
    /// </remarks>
    double FrontDriveRatio { get; set; }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 indicating the Bias in grip
    /// between the front and rear of the car.
    /// </summary>
    double FrontGripBias { get; set; }

    /// <summary>
    /// Gets a collection of values taht indicates how the gas decreases when
    /// released.
    /// </summary>
    ICollection<double> GasDecreaseCurve { get; }

    /// <summary>
    /// Gets a collection of values taht indicates how the gas increases when
    /// applied.
    /// </summary>
    ICollection<double> GasIncreaseCurve { get; }

    double GasOffFactor { get; set; }

    /// <summary>
    /// Gets a collection of values that indicates the gear efficiency
    /// multiplier for any given gear for the automatic transmission.
    /// </summary>
    ICollection<double> GearEfficiencyAuto { get; }

    /// <summary>
    /// Gets a collection of values that indicates the gear efficiency
    /// multiplier for any given gear for the manual transmission.
    /// </summary>
    ICollection<double> GearEfficiencyManual { get; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the gear
    /// ratios of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    double GearRatFactor { get; set; }

    /// <summary>
    /// Gets a collection of values that indicate gear ratio for any given gear
    /// for the automatic transmission.
    /// </summary>
    /// <remarks>
    /// These values are merely informational, and are apparently not used for
    /// actual speed calculations.
    /// </remarks>
    ICollection<double> GearRatioAuto { get; }

    /// <summary>
    /// Gets a collection of values that indicate gear ratio for any given gear
    /// for the manual transmission.
    /// </summary>
    /// <remarks>
    /// These values are merely informational, and are apparently not used for
    /// actual speed calculations.
    /// </remarks>
    ICollection<double> GearRatioManual { get; }

    /// <summary>
    /// Gets a value that indicates the number of game ticks of delay between
    /// gear shifts.
    /// </summary>
    int GearShiftDelay { get; set; }

    double GradualTurnCutoff { get; set; }

    double GTransferFactor { get; set; }

    double HighTurnFactor { get; set; }

    double LateralAccGripMult { get; set; }

    double LowTurnFactor { get; set; }

    /// <summary>
    /// Gets or sets the car mass in Kilograms.
    /// </summary>
    double Mass { get; set; }

    /// <summary>
    /// Gets a value that indicates the maximum deceleration while braking.
    /// </summary>
    double MaxBrakeDecel { get; set; }

    /// <summary>
    /// Gets the maximum velocity this car can reach, in meters per second.
    /// </summary>
    /// <remarks>
    /// Speed calculation uses this value alongside the current RPM and the
    /// velocity to RPM tables to calculate the actual speed.
    /// </remarks>
    double MaxVelocity { get; set; }

    double MediumTurnCutoff { get; set; }

    double MediumTurnSpdMod { get; set; }

    double MinimumSteerAccel { get; set; }

    /// <summary>
    /// Gets a value that indicates the number of defined gears for the
    /// automatic transmission.
    /// </summary>
    /// <remarks>
    /// Count includes reverse gear as well as neutral.
    /// </remarks>
    int NumberOfGearsAuto { get; set; }

    /// <summary>
    /// Gets a value that indicates the number of defined gears for the manual
    /// transmission.
    /// </summary>
    /// <remarks>
    /// Count includes reverse gear as well as neutral.
    /// </remarks>
    int NumberOfGearsManual { get; set; }

    double PitchRollFactor { get; set; }

    /// <summary>
    /// Gets a value that indicates whether or not this car is equipped with
    /// Power steering.
    /// </summary>
    bool PowerSteering { get; set; }

    double PushFactor { get; set; }

    double RoadBumpFactor { get; set; }
    
    double SharpTurnCutoff { get; set; }
    
    double SharpTurnSpdMod { get; set; }

    /// <summary>
    /// Gets a collection of values that indicate the RPM blip to be applied
    /// when shifting gears up.
    /// </summary>
    /// <remarks>
    /// The number of elements in this array should idealy remain at 8.
    /// </remarks>
    ICollection<double> ShiftBlip { get; }
    
    double SlideAssistanceFactor { get; set; }
    
    double SlideMult { get; set; }
    
    double SlideVelocityCap { get; set; }
    
    double SpinVelocityCap { get; set; }
    
    double SpoilerActivationSpeed { get; set; }
    
    int SpoilerFunctionType { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// steering responsiveness of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    double SteeringSpeed { get; set; }
    
    double SubdivideLevel { get; set; }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of
    /// suspension damage.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    double SuspensionDamage { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the
    /// suspension stiffness of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    double SuspensionStiffness { get; set; }

    /// <summary>
    /// Gets or sets a value that allows the player to tune up or down the tire
    /// grip of the car.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it will be modified in memory by
    /// NFS3 during normal gameplay, and changing it might have unknown
    /// consequences.
    /// </remarks>
    double TireFactor { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the front rim diameter.
    /// </summary>
    int TireRimFront { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the rear rim diameter.
    /// </summary>
    int TireRimRear { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the ratio between the front tire
    /// width and its sidewall.
    /// </summary>
    int TireSidewallFront { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the ratio between the rear tire
    /// width and its sidewall.
    /// </summary>
    int TireSidewallRear { get; set; }

    /// <summary>
    /// Gets or sets a value from 0.0 to 1.0 that indicates the amount of tire
    /// wear.
    /// </summary>
    /// <remarks>
    /// This value should remain at 0.0, as it goes unused by NFS3, and
    /// changing it might have unknown consequences.
    /// </remarks>
    double TireWear { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the width of the front tire in
    /// milimeters.
    /// </summary>
    int TireWidthFront { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the width of the rear tire in
    /// milimeters.
    /// </summary>
    int TireWidthRear { get; set; }

    /// <remarks>
    /// This value seems to be informational in nature, but should match the
    /// value in <see cref="MaxVelocity"/>
    /// </remarks>
    double TopSpeed { get; set; }

    /// <summary>
    /// Gets a collection of values plotting the engine's torque curve.
    /// </summary>
    ICollection<double> TorqueCurve { get; }
    
    double TurnCircleRadius { get; set; }
    
    double TurnInRamp { get; set; }
    
    double TurnOutRamp { get; set; }

    /// <summary>
    /// Gets a collection of values that indicate a factor to divide the RPM by
    /// to calculate the actual top speed in any given gear for the automatic
    /// transmission.
    /// </summary>
    ICollection<double> VelocityToRpmAuto { get; }

    /// <summary>
    /// Gets a collection of values that indicate a factor to divide the RPM by
    /// to calculate the actual top speed in any given gear for the manual
    /// transmission.
    /// </summary>
    ICollection<double> VelocityToRpmManual { get; }

    /// <summary>
    /// Gets or sets a value that indicates the wheel base for the car (the
    /// distance between front and rear tires).
    /// </summary>
    double WheelBase { get; set; }
}