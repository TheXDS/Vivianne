namespace TheXDS.Vivianne.Models.Carp.Nfs2;

public class CarPerf
{
    public bool IsCompressed { get; set; }

    public double Mass { get; set; }

    public int NumberOfGears { get; set; }

    public int GearShiftDelay { get; set; }

    public IList<double> GearEfficiency { get; init; } = [];

    public IList<double> VelocityToRpm { get; init; } = [];

    public IList<double> TorqueCurve { get; init; } = [];

    public int EngineMaxRpm { get; set; }

    public double MaxVelocity { get; set; }

    public double FrontDriveRatio { get; set; }

    public double MaxBrakeDecel { get; set; }

    public double FrontBrakeBias { get; set; }

    public IList<byte> GasIncreaseCurve { get; init; } = [];

    public IList<byte> GasDecreaseCurve { get; init; } = [];

    public IList<byte> BrakeIncreaseCurve { get; init; } = [];

    public IList<byte> BrakeDecreaseCurve { get; init; } = [];

    public float WheelBase { get; set; }

    public double FrontGripBias { get; set; }

    public double MaximumSteerAccel { get; set; }

    public double TurnInRamp { get; set; }

    public double TurnOutRamp { get; set; }

    public double LateralAccGripMult { get; set; }

    public double AeroDownMult { get; set; }

    public double GasOffFactor { get; set; }

    public double GTransferFactor { get; set; }

    public double SlideMult { get; set; }

    public double SpinVelocityCap { get; set; }

    public double SlideVelocityCap { get; set; }

    public double SlideAssistanceFactor { get; set; }

    public double PushFactor { get; set; }

    public double LowTurnFactor { get; set; }

    public double HighTurnFactor { get; set; }
}
